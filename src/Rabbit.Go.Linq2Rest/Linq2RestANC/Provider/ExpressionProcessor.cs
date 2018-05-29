// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionProcessor.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExpressionProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;

    internal class ExpressionProcessor : IExpressionProcessor
    {
        private readonly IExpressionWriter _writer;
        private readonly IMemberNameResolver _memberNameResolver;

        public ExpressionProcessor(IExpressionWriter writer, IMemberNameResolver memberNameResolver)
        {
            CustomContract.Requires(writer != null);
            CustomContract.Requires(memberNameResolver != null);

            _writer = writer;
            _memberNameResolver = memberNameResolver;
        }

        public object ProcessMethodCall<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IEnumerable<T>> resultLoader, Func<Type, ParameterBuilder, IEnumerable> intermediateResultLoader)
        {
            if (methodCall == null)
            {
                return null;
            }

            var method = methodCall.Method.Name;

            switch (method)
            {
                case "First":
                case "FirstOrDefault":
                    builder.TakeParameter = "1";
                    return methodCall.Arguments.Count >= 2
                                ? GetMethodResult(methodCall, builder, resultLoader, intermediateResultLoader)
                                : GetResult(methodCall, builder, resultLoader, intermediateResultLoader);

                case "Single":
                case "SingleOrDefault":
                case "Last":
                case "LastOrDefault":
                case "Count":
                case "LongCount":
                    return methodCall.Arguments.Count >= 2
                            ? GetMethodResult(methodCall, builder, resultLoader, intermediateResultLoader)
                            : GetResult(methodCall, builder, resultLoader, intermediateResultLoader);
                case "Where":
                    CustomContract.Assume(methodCall.Arguments.Count >= 2);
                    {
                        var result = ProcessMethodCall(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
                        if (result != null)
                        {
                            return InvokeEager(methodCall, result);
                        }

                        var newFilter = _writer.Write(methodCall.Arguments[1], builder.SourceType);

                        builder.FilterParameter = string.IsNullOrWhiteSpace(builder.FilterParameter)
                                                    ? newFilter
                                                    : string.Format("{0} and {1}", builder.FilterParameter, newFilter);
                    }

                    break;
                case "Select":
                    CustomContract.Assume(methodCall.Arguments.Count >= 2);
                    {
                        var result = ProcessMethodCall(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
                        if (result != null)
                        {
                            return InvokeEager(methodCall, result);
                        }

                        if (!string.IsNullOrWhiteSpace(builder.SelectParameter))
                        {
                            return ExecuteMethod(methodCall, builder, resultLoader, intermediateResultLoader);
                        }

                        var unaryExpression = methodCall.Arguments[1] as UnaryExpression;
                        if (unaryExpression != null)
                        {
                            var lambdaExpression = unaryExpression.Operand as LambdaExpression;
                            if (lambdaExpression != null)
                            {
                                var sourceType = builder.SourceType;
                                return ResolveProjection(builder, lambdaExpression, sourceType);
                            }
                        }
                    }

                    break;
                case "OrderBy":
                case "ThenBy":
                    CustomContract.Assume(methodCall.Arguments.Count >= 2);
                    {
                        var methodCallExpression = methodCall.Arguments[0] as MethodCallExpression;
                        var result = ProcessMethodCall(methodCallExpression, builder, resultLoader, intermediateResultLoader);
                        if (result != null)
                        {
                            return InvokeEager(methodCall, result);
                        }

                        var sourceType = builder.SourceType;
                        var sortProperty = methodCall.Arguments[1];
                        var item = _writer.Write(sortProperty, sourceType);
                        builder.OrderByParameter.Add(item);
                    }

                    break;
                case "OrderByDescending":
                case "ThenByDescending":
                    CustomContract.Assume(methodCall.Arguments.Count >= 2);
                    {
                        var result = ProcessMethodCall(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
                        if (result != null)
                        {
                            return InvokeEager(methodCall, result);
                        }

                        var visit = _writer.Write(methodCall.Arguments[1], builder.SourceType);
                        builder.OrderByParameter.Add(visit + " desc");
                    }

                    break;
                case "Take":
                    CustomContract.Assume(methodCall.Arguments.Count >= 2);
                    {
                        var result = ProcessMethodCall(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
                        if (result != null)
                        {
                            return InvokeEager(methodCall, result);
                        }

                        builder.TakeParameter = _writer.Write(methodCall.Arguments[1], builder.SourceType);
                    }

                    break;
                case "Skip":
                    CustomContract.Assume(methodCall.Arguments.Count >= 2);
                    {
                        var result = ProcessMethodCall(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
                        if (result != null)
                        {
                            return InvokeEager(methodCall, result);
                        }

                        builder.SkipParameter = _writer.Write(methodCall.Arguments[1], builder.SourceType);
                    }

                    break;
                case "Expand":
                    CustomContract.Assume(methodCall.Arguments.Count >= 2);
                    {
                        var result = ProcessMethodCall(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
                        if (result != null)
                        {
                            return InvokeEager(methodCall, result);
                        }

                        var expression = methodCall.Arguments[1];

                        CustomContract.Assume(expression != null);

                        var objectMember = Expression.Convert(expression, typeof(object));
                        var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

                        builder.ExpandParameter = getterLambda().ToString();
                    }

                    break;
                default:
                    return ExecuteMethod(methodCall, builder, resultLoader, intermediateResultLoader);
            }

            return null;
        }

        private static object InvokeEager(MethodCallExpression methodCall, object source)
        {
            CustomContract.Requires(source != null);
            CustomContract.Requires(methodCall != null);

            var results = source as IEnumerable;

            CustomContract.Assume(results != null);

            var parameters = ResolveInvocationParameters(results, methodCall);
            return methodCall.Method.Invoke(null, parameters);
        }

        private static object[] ResolveInvocationParameters(IEnumerable results, MethodCallExpression methodCall)
        {
            CustomContract.Requires(results != null);
            CustomContract.Requires(methodCall != null);

            var parameters = new object[] { results.AsQueryable() }
                .Concat(methodCall.Arguments.Where((x, i) => i > 0).Select(GetExpressionValue))
                .ToArray();
            return parameters;
        }

        private static object GetExpressionValue(Expression expression)
        {
            if (expression is UnaryExpression)
            {
                return (expression as UnaryExpression).Operand;
            }

            if (expression is ConstantExpression)
            {
                return (expression as ConstantExpression).Value;
            }

            return null;
        }

        private object ResolveProjection(ParameterBuilder builder, LambdaExpression lambdaExpression, Type sourceType)
        {
            CustomContract.Requires(lambdaExpression != null);

            var selectFunction = lambdaExpression.Body as NewExpression;

            if (selectFunction != null)
            {
                var properties = sourceType.GetProperties();
                var members = selectFunction.Members
                    .Select(x => properties.FirstOrDefault(y => y.Name == x.Name) ?? x)
                                         .Select(x => _memberNameResolver.ResolveName(x))
                                         .ToArray();
                var args = selectFunction.Arguments.OfType<MemberExpression>()
                                         .Select(x => properties.FirstOrDefault(y => y.Name == x.Member.Name) ?? x.Member)
                                         .Select(x => _memberNameResolver.ResolveName(x))
                                         .ToArray();
                if (members.Intersect(args).Count() != members.Length)
                {
                    throw new InvalidOperationException("Projection into new member names is not supported.");
                }

                builder.SelectParameter = string.Join(",", args);
            }

            var propertyExpression = lambdaExpression.Body as MemberExpression;
            if (propertyExpression != null)
            {
                builder.SelectParameter = string.IsNullOrWhiteSpace(builder.SelectParameter)
                    ? _memberNameResolver.ResolveName(propertyExpression.Member)
                    : builder.SelectParameter + "," + _memberNameResolver.ResolveName(propertyExpression.Member);
            }

            return null;
        }

        private object GetMethodResult<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IEnumerable<T>> resultLoader, Func<Type, ParameterBuilder, IEnumerable> intermediateResultLoader)
        {
            CustomContract.Requires(methodCall != null);
            CustomContract.Requires(builder != null);
            CustomContract.Requires(resultLoader != null);
            CustomContract.Requires(intermediateResultLoader != null);
            CustomContract.Assume(methodCall.Arguments.Count >= 2);

            ProcessMethodCall(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);

            var processResult = _writer.Write(methodCall.Arguments[1], builder.SourceType);
            var currentParameter = string.IsNullOrWhiteSpace(builder.FilterParameter)
                                    ? processResult
                                    : string.Format("({0}) and ({1})", builder.FilterParameter, processResult);
            builder.FilterParameter = currentParameter;

            var genericArguments = methodCall.Method.GetGenericArguments();
            var queryableMethods = typeof(Queryable).GetMethods();

            CustomContract.Assume(queryableMethods.Any());

            var nonGenericMethod = queryableMethods
                .Single(x => x.Name == methodCall.Method.Name && x.GetParameters().Length == 1);

            CustomContract.Assume(nonGenericMethod != null);

            var method = nonGenericMethod.MakeGenericMethod(genericArguments);

            var list = resultLoader(builder);

            CustomContract.Assume(list != null);

            var queryable = list.AsQueryable();
            var parameters = new object[] { queryable };
            var result = method.Invoke(null, parameters);
            return result ?? default(T);
        }

        private object GetResult<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IEnumerable<T>> resultLoader, Func<Type, ParameterBuilder, IEnumerable> intermediateResultLoader)
        {
            CustomContract.Requires(builder != null);
            CustomContract.Requires(methodCall != null);
            CustomContract.Requires(resultLoader != null);
            CustomContract.Requires(intermediateResultLoader != null);

            CustomContract.Assume(methodCall.Arguments.Count >= 1);

            var methodCallExpression = methodCall.Arguments[0] as MethodCallExpression;
            ProcessMethodCall(methodCallExpression, builder, resultLoader, intermediateResultLoader);

            var results = resultLoader(builder);

            CustomContract.Assume(results != null);

            var parameters = ResolveInvocationParameters(results, methodCall);
            var final = methodCall.Method.Invoke(null, parameters);
            return final;
        }

        private object ExecuteMethod<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IEnumerable<T>> resultLoader, Func<Type, ParameterBuilder, IEnumerable> intermediateResultLoader)
        {
            CustomContract.Requires(methodCall != null);
            CustomContract.Requires(resultLoader != null);
            CustomContract.Requires(intermediateResultLoader != null);
            CustomContract.Requires(builder != null);
            CustomContract.Assume(methodCall.Arguments.Count >= 2);

            var innerMethod = methodCall.Arguments[0] as MethodCallExpression;

            if (innerMethod == null)
            {
                return null;
            }

            var result = ProcessMethodCall(innerMethod, builder, resultLoader, intermediateResultLoader);
            if (result != null)
            {
                return InvokeEager(innerMethod, result);
            }

            var genericArgument = innerMethod.Method.ReturnType.GetGenericArguments()[0];

            var type = typeof(T);
            var list = type != genericArgument
             ? intermediateResultLoader(genericArgument, builder)
             : resultLoader(builder);

            CustomContract.Assume(list != null);

            var arguments = ResolveInvocationParameters(list, methodCall);

            return methodCall.Method.Invoke(null, arguments);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            CustomContract.Invariant(_writer != null);
        }
    }
}