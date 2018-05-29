// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterExpressionFactory.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FilterExpressionFactory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
    using Linq2Rest.Parser.Readers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the FilterExpressionFactory.
    /// </summary>
    public class FilterExpressionFactory : IFilterExpressionFactory
    {
        private static readonly Regex StringRx = new Regex(@"^[""'](.*?)[""']$", RegexOptions.Compiled);
        private static readonly Regex NegateRx = new Regex(@"^-[^\d]*", RegexOptions.Compiled);
        private readonly IMemberNameResolver _memberNameResolver;
        private readonly ParameterValueReader _valueReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExpressionFactory"/> class.
        /// </summary>
        /// <param name="memberNameResolver">An <see cref="IMemberNameResolver"/> for name resolution.</param>
        /// <param name="expressionFactories">The custom <see cref="IValueExpressionFactory"/> to use for value conversion.</param>
        public FilterExpressionFactory(IMemberNameResolver memberNameResolver, IEnumerable<IValueExpressionFactory> expressionFactories)
        {
            CustomContract.Requires<ArgumentNullException>(memberNameResolver != null);
            CustomContract.Requires<ArgumentNullException>(expressionFactories != null);

            _valueReader = new ParameterValueReader(expressionFactories);
            _memberNameResolver = memberNameResolver;
        }

        /// <summary>
        /// Creates a filter expression from its string representation.
        /// </summary>
        /// <param name="filter">The string representation of the filter.</param>
        /// <typeparam name="T">The <see cref="Type"/> of item to filter.</typeparam>
        /// <returns>An <see cref="Expression{TDelegate}"/> if the passed filter is valid, otherwise null.</returns>
        public Expression<Func<T, bool>> Create<T>(string filter)
        {
            return Create<T>(filter, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Creates a filter expression from its string representation.
        /// </summary>
        /// <param name="filter">The string representation of the filter.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> to use when reading the filter.</param>
        /// <typeparam name="T">The <see cref="Type"/> of item to filter.</typeparam>
        /// <returns>An <see cref="Expression{TDelegate}"/> if the passed filter is valid, otherwise null.</returns>
        public Expression<Func<T, bool>> Create<T>(string filter, IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return x => true;
            }

            var parameter = Expression.Parameter(typeof(T), "x");

            var expression = CreateExpression<T>(filter, parameter, new List<ParameterExpression>(), null, formatProvider);

            if (expression != null)
            {
                return Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            throw new InvalidOperationException("Could not create valid expression from: " + filter);
        }

        private static Type GetFunctionParameterType(string operation)
        {
            CustomContract.Requires(operation != null);

            switch (operation.ToLowerInvariant())
            {
                case "substring":
                    return typeof(int);

                default:
                    return null;
            }
        }

        private static Expression GetOperation(string token, Expression left, Expression right)
        {
            CustomContract.Requires(token != null);
            CustomContract.Requires(right != null);

            return left == null ? GetRightOperation(token, right) : GetLeftRightOperation(token, left, right);
        }

        private static Expression GetLeftRightOperation(string token, Expression left, Expression right)
        {
            CustomContract.Requires(token != null);
            CustomContract.Requires(left != null);
            CustomContract.Requires(right != null);

            switch (token.ToLowerInvariant())
            {
                case "eq":
                    if (left.Type.IsEnum() && left.Type.GetCustomAttributes(typeof(FlagsAttribute), true).Any())
                    {
                        var underlyingType = Enum.GetUnderlyingType(left.Type);
                        var leftValue = Expression.Convert(left, underlyingType);
                        var rightValue = Expression.Convert(right, underlyingType);
                        var andExpression = Expression.And(leftValue, rightValue);
                        return Expression.Equal(andExpression, rightValue);
                    }

                    return Expression.Equal(left, right);

                case "ne":
                    return Expression.NotEqual(left, right);

                case "gt":
                    return Expression.GreaterThan(left, right);

                case "ge":
                    return Expression.GreaterThanOrEqual(left, right);

                case "lt":
                    return Expression.LessThan(left, right);

                case "le":
                    return Expression.LessThanOrEqual(left, right);

                case "and":
                    return Expression.AndAlso(left, right);

                case "or":
                    return Expression.OrElse(left, right);

                case "add":
                    return Expression.Add(left, right);

                case "sub":
                    return Expression.Subtract(left, right);

                case "mul":
                    return Expression.Multiply(left, right);

                case "div":
                    return Expression.Divide(left, right);

                case "mod":
                    return Expression.Modulo(left, right);
            }

            throw new InvalidOperationException("Could not understand operation: " + token);
        }

        private static Expression GetRightOperation(string token, Expression right)
        {
            CustomContract.Requires(token != null);
            CustomContract.Requires(right != null);

            Expression result = null;
            switch (token.ToLowerInvariant())
            {
                case "not":
                    result = right.Type == typeof(bool) ? Expression.Not(right) : null;
                    break;
            }

            if (result == null)
            {
                throw new InvalidOperationException(string.Format("Could not create valid expression from: {0} {1}", token, right));
            }

            return result;
        }

        private static Expression GetFunction(string function, Expression left, Expression right, ParameterExpression sourceParameter, ICollection<ParameterExpression> lambdaParameters)
        {
            CustomContract.Requires(function != null);
            CustomContract.Requires(left != null);

            switch (function.ToLowerInvariant())
            {
                case "substringof":
                    return Expression.Call(right, MethodProvider.ContainsMethod, new[] { left });

                case "endswith":
                    return Expression.Call(left, MethodProvider.EndsWithMethod, new[] { right });

                case "startswith":
                    return Expression.Call(left, MethodProvider.StartsWithMethod, new[] { right });

                case "length":
                    return Expression.Property(left, MethodProvider.LengthProperty);

                case "indexof":
                    return Expression.Call(left, MethodProvider.IndexOfMethod, new[] { right });

                case "substring":
                    return Expression.Call(left, MethodProvider.SubstringMethod, new[] { right });

                case "tolower":
                    return Expression.Call(left, MethodProvider.ToLowerMethod);

                case "toupper":
                    return Expression.Call(left, MethodProvider.ToUpperMethod);

                case "trim":
                    return Expression.Call(left, MethodProvider.TrimMethod);

                case "hour":
                    return Expression.Property(left, MethodProvider.HourProperty);

                case "minute":
                    return Expression.Property(left, MethodProvider.MinuteProperty);

                case "second":
                    return Expression.Property(left, MethodProvider.SecondProperty);

                case "day":
                    return Expression.Property(left, MethodProvider.DayProperty);

                case "month":
                    return Expression.Property(left, MethodProvider.MonthProperty);

                case "year":
                    return Expression.Property(left, MethodProvider.YearProperty);

                case "round":
                    return Expression.Call(left.Type == typeof(double) ? MethodProvider.DoubleRoundMethod : MethodProvider.DecimalRoundMethod, left);

                case "floor":
                    return Expression.Call(left.Type == typeof(double) ? MethodProvider.DoubleFloorMethod : MethodProvider.DecimalFloorMethod, left);

                case "ceiling":
                    return Expression.Call(left.Type == typeof(double) ? MethodProvider.DoubleCeilingMethod : MethodProvider.DecimalCeilingMethod, left);

                case "any":
                case "all":
                    {
                        CustomContract.Assume(right != null);
                        CustomContract.Assume(!string.IsNullOrEmpty(function));

                        return CreateAnyAllExpression(
                                                      left,
                                                      right,
                                                      sourceParameter,
                                                      lambdaParameters,
                                                      MethodProvider.GetAnyAllMethod(function.Capitalize(), left.Type));
                    }

                default:
                    return null;
            }
        }

        private static Expression CreateAnyAllExpression(
            Expression left,
            Expression right,
            ParameterExpression sourceParameter,
            IEnumerable<ParameterExpression> lambdaParameters,
            MethodInfo anyAllMethod)
        {
            CustomContract.Requires(left != null);
            CustomContract.Requires(right != null);

            var genericFunc = typeof(Func<,>)
                .MakeGenericType(
                                 MethodProvider.GetIEnumerableImpl(left.Type).GetGenericArguments()[0],
                                 typeof(bool));

            var filteredParameters = new ParameterVisitor()
                .GetParameters(right)
                .Where(p => p.Name != sourceParameter.Name)
                .ToArray();
            if (!filteredParameters.Any())
            {
                filteredParameters = lambdaParameters.ToArray();
            }

            return Expression.Call(
                                   anyAllMethod,
                                   left,
                                   Expression.Lambda(genericFunc, right, filteredParameters));
        }

        private static Type GetNonNullableType(Type type)
        {
            CustomContract.Requires(type != null);

            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? type.GetGenericArguments()[0]
                    : type;
        }

        private static bool SupportsNegate(Type type)
        {
            CustomContract.Requires(type != null);

            type = GetNonNullableType(type);
            if (!type.IsEnum())
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return true;
                }
            }

            return false;
        }

        private Expression GetBooleanExpression(string filter, IFormatProvider formatProvider)
        {
            CustomContract.Requires(filter != null);

            var booleanExpression = _valueReader.Read(typeof(bool), filter, formatProvider) as ConstantExpression;
            return booleanExpression != null && booleanExpression.Value != null
                ? booleanExpression
                : null;
        }

        private Expression GetParameterExpression(string filter, Type type, IFormatProvider formatProvider)
        {
            CustomContract.Requires(filter != null);

            return type != null
                ? _valueReader.Read(type, filter, formatProvider)
                : GetBooleanExpression(filter, formatProvider);
        }

        private Type GetExpressionType<T>(TokenSet set, ParameterExpression parameter, ICollection<ParameterExpression> lambdaParameters)
        {
            CustomContract.Requires(parameter != null);
            CustomContract.Requires(lambdaParameters != null);

            if (set == null)
            {
                return null;
            }

            if (Regex.IsMatch(set.Left, @"^\(.*\)$") && set.Operation.IsCombinationOperation())
            {
                return null;
            }

            if (set.Left.IsFunction())
            {
                var functionName = set.Left.GetFunctionName();
                if (!string.IsNullOrWhiteSpace(functionName))
                {
                    return functionName.GetFunctionType();
                }
            }

            var property = GetPropertyExpression<T>(set.Left, parameter, lambdaParameters) ?? GetPropertyExpression<T>(set.Right, parameter, lambdaParameters);
            if (property != null)
            {
                return property.Type;
            }

            var type = GetExpressionType<T>(set.Left.GetArithmeticToken(), parameter, lambdaParameters);

            return type ?? GetExpressionType<T>(set.Right.GetArithmeticToken(), parameter, lambdaParameters);
        }

        private Expression GetPropertyExpression<T>(string propertyToken, ParameterExpression parameter, ICollection<ParameterExpression> lambdaParameters)
        {
            CustomContract.Requires(propertyToken != null);
            CustomContract.Requires(parameter != null);
            CustomContract.Requires(lambdaParameters != null);

            if (string.IsNullOrWhiteSpace(propertyToken))
            {
                return null;
            }

            if (!propertyToken.IsImpliedBoolean())
            {
                var token = propertyToken.GetTokens().FirstOrDefault();
                if (token != null)
                {
                    return GetPropertyExpression<T>(token.Left, parameter, lambdaParameters) ?? GetPropertyExpression<T>(token.Right, parameter, lambdaParameters);
                }
            }

            var parentType = parameter.Type;
            Expression propertyExpression = null;

            var propertyChain = propertyToken.Split('/');

            CustomContract.Assert(propertyChain != null);

            if (propertyChain.Any() && lambdaParameters.Any(p => p.Name == propertyChain.First()))
            {
                var lambdaParameter = lambdaParameters.First(p => p.Name == propertyChain.First());

                CustomContract.Assume(lambdaParameter != null);

                parentType = lambdaParameter.Type;
                propertyExpression = lambdaParameter;
            }

            propertyExpression = _memberNameResolver.CreateMemberExpression(parameter, propertyChain, parentType, propertyExpression).Item2;

            return propertyExpression;
        }

        private Expression CreateExpression<T>(string filter, ParameterExpression sourceParameter, ICollection<ParameterExpression> lambdaParameters, Type type, IFormatProvider formatProvider)
        {
            CustomContract.Requires(filter != null);
            CustomContract.Requires(sourceParameter != null);
            CustomContract.Requires(lambdaParameters != null);

            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }

            var tokens = filter.GetTokens();

            if (tokens.Any())
            {
                return GetTokenExpression<T>(sourceParameter, lambdaParameters, type, formatProvider, tokens);
            }

            if (string.Equals(filter, "null", StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Constant(null);
            }

            var stringMatch = StringRx.Match(filter);

            if (stringMatch.Success)
            {
                return Expression.Constant(stringMatch.Groups[1].Value.Replace("''", "'"), typeof(string));
            }

            if (NegateRx.IsMatch(filter))
            {
                var negateExpression = CreateExpression<T>(
                    filter.Substring(1),
                    sourceParameter,
                    lambdaParameters,
                    type,
                    formatProvider);

                CustomContract.Assert(negateExpression != null);

                if (SupportsNegate(negateExpression.Type))
                {
                    return Expression.Negate(negateExpression);
                }

                throw new InvalidOperationException("Cannot negate " + negateExpression);
            }

            var expression = GetAnyAllFunctionExpression<T>(filter, sourceParameter, lambdaParameters, formatProvider)
                ?? GetContainsFunctionExpression<T>(filter, sourceParameter, lambdaParameters, formatProvider)
                ?? GetPropertyExpression<T>(filter, sourceParameter, lambdaParameters)
                ?? GetArithmeticExpression<T>(filter, sourceParameter, lambdaParameters, type, formatProvider)
                ?? GetFunctionExpression<T>(filter, sourceParameter, lambdaParameters, type, formatProvider)
                ?? GetParameterExpression(filter, type, formatProvider)
                ?? GetBooleanExpression(filter, formatProvider);

            if (expression == null)
            {
                throw new InvalidOperationException("Could not create expression from: " + filter);
            }

            return expression;
        }

        private Expression GetTokenExpression<T>(ParameterExpression parameter, ICollection<ParameterExpression> lambdaParameters, Type type, IFormatProvider formatProvider, ICollection<TokenSet> tokens)
        {
            CustomContract.Requires(tokens != null);
            CustomContract.Requires(parameter != null);
            CustomContract.Requires(lambdaParameters != null);

            string combiner = null;
            Expression existing = null;
            foreach (var tokenSet in tokens)
            {
                if (string.IsNullOrWhiteSpace(tokenSet.Left))
                {
                    if (string.Equals(tokenSet.Operation, "not", StringComparison.OrdinalIgnoreCase))
                    {
                        var right = CreateExpression<T>(
                                                        tokenSet.Right,
                                                        parameter,
                                                        lambdaParameters,
                                                        type ?? GetExpressionType<T>(tokenSet, parameter, lambdaParameters),
                                                        formatProvider);

                        return right == null
                                ? null
                                : GetOperation(tokenSet.Operation, null, right);
                    }

                    combiner = tokenSet.Operation;
                }
                else
                {
                    var left = CreateExpression<T>(
                                                   tokenSet.Left,
                                                   parameter,
                                                   lambdaParameters,
                                                   type ?? GetExpressionType<T>(tokenSet, parameter, lambdaParameters),
                                                   formatProvider);
                    if (left == null)
                    {
                        return null;
                    }

                    var rightExpressionType = tokenSet.Operation == "and" ? null : left.Type;
                    var right = CreateExpression<T>(tokenSet.Right, parameter, lambdaParameters, rightExpressionType, formatProvider);

                    if (existing != null && !string.IsNullOrWhiteSpace(combiner))
                    {
                        var current = right == null ? null : GetOperation(tokenSet.Operation, left, right);
                        existing = GetOperation(combiner, existing, current ?? left);
                    }
                    else if (right != null)
                    {
                        existing = GetOperation(tokenSet.Operation, left, right);
                    }
                }
            }

            return existing;
        }

        private Expression GetArithmeticExpression<T>(string filter, ParameterExpression parameter, ICollection<ParameterExpression> lambdaParameters, Type type, IFormatProvider formatProvider)
        {
            CustomContract.Requires(filter != null);
            CustomContract.Requires(parameter != null);
            CustomContract.Requires(lambdaParameters != null);

            var arithmeticToken = filter.GetArithmeticToken();
            if (arithmeticToken == null)
            {
                return null;
            }

            var type1 = type ?? GetExpressionType<T>(arithmeticToken, parameter, lambdaParameters);
            var leftExpression = CreateExpression<T>(arithmeticToken.Left, parameter, lambdaParameters, type1, formatProvider);
            var rightExpression = CreateExpression<T>(arithmeticToken.Right, parameter, lambdaParameters, type1, formatProvider);

            return leftExpression == null || rightExpression == null
                    ? null
                    : GetLeftRightOperation(arithmeticToken.Operation, leftExpression, rightExpression);
        }

        private Expression GetContainsFunctionExpression<T>(string filter, ParameterExpression sourceParameter, ICollection<ParameterExpression> lambdaParameters, IFormatProvider formatProvider)
        {
            CustomContract.Requires(filter != null);
            CustomContract.Requires(sourceParameter != null);
            CustomContract.Requires(lambdaParameters != null);

            var functionTokens = filter.GetInFunctionTokens();
            if (functionTokens == null)
                return null;

            var arrayValues = functionTokens.Right?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //数组没有任何值则直接返回表达式 false
            if (arrayValues == null || !arrayValues.Any())
                return Expression.IsTrue(Expression.Constant(false));

            var propertyExpression = GetPropertyExpression<T>(functionTokens.Left, sourceParameter, lambdaParameters);
            var leftType = propertyExpression.Type;
            var left = CreateExpression<T>(
                functionTokens.Left,
                sourceParameter,
                lambdaParameters,
                leftType,
                formatProvider);

            var parseMethod = leftType.GetMethod("Parse", new[] { typeof(string) });

            object values = arrayValues.Select(v => parseMethod.Invoke(null, new object[] { v }));

            values = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(leftType).Invoke(null, new[] { values });

            var valuesExpression = Expression.Constant(values);

            var containsExpression = Expression.Call(typeof(Enumerable), "Contains", new[] { leftType }, valuesExpression, left);
            return containsExpression;
        }

        private Expression GetAnyAllFunctionExpression<T>(string filter, ParameterExpression sourceParameter, ICollection<ParameterExpression> lambdaParameters, IFormatProvider formatProvider)
        {
            CustomContract.Requires(filter != null);
            CustomContract.Requires(sourceParameter != null);
            CustomContract.Requires(lambdaParameters != null);

            var functionTokens = filter.GetAnyAllFunctionTokens();
            if (functionTokens == null)
            {
                return null;
            }

            var propertyExpression = GetPropertyExpression<T>(functionTokens.Left, sourceParameter, lambdaParameters);
            var leftType = propertyExpression.Type;
            var left = CreateExpression<T>(
                functionTokens.Left,
                sourceParameter,
                lambdaParameters,
                leftType,
                formatProvider);

            // Create a new ParameterExpression from the lambda parameter and add to a collection to pass around
            var parameterName = functionTokens.Right.Substring(0, functionTokens.Right.IndexOf(":", StringComparison.CurrentCultureIgnoreCase)).Trim();
            var lambdaParameter =
                Expression.Parameter(MethodProvider.GetIEnumerableImpl(leftType).GetGenericArguments()[0], parameterName);
            lambdaParameters.Add(lambdaParameter);
            var lambdaFilter = functionTokens.Right.Substring(functionTokens.Right.IndexOf(":", StringComparison.CurrentCultureIgnoreCase) + 1).Trim();
            var lambdaType = GetFunctionParameterType(functionTokens.Operation)
                             ?? (left != null ? left.Type : null);

            var isLambdaAnyAllFunction = lambdaFilter.GetAnyAllFunctionTokens() != null;
            var right = isLambdaAnyAllFunction
                ? GetAnyAllFunctionExpression<T>(lambdaFilter, lambdaParameter, lambdaParameters, formatProvider)
                : CreateExpression<T>(lambdaFilter, sourceParameter, lambdaParameters, lambdaType, formatProvider);

            return left == null
                ? null
                : GetFunction(functionTokens.Operation, left, right, sourceParameter, lambdaParameters);
        }

        private Expression GetFunctionExpression<T>(string filter, ParameterExpression sourceParameter, ICollection<ParameterExpression> lambdaParameters, Type type, IFormatProvider formatProvider)
        {
            CustomContract.Requires(filter != null);
            CustomContract.Requires(sourceParameter != null);
            CustomContract.Requires(lambdaParameters != null);

            var functionTokens = filter.GetFunctionTokens();
            if (functionTokens == null)
            {
                return null;
            }

            var left = CreateExpression<T>(
                functionTokens.Left,
                sourceParameter,
                lambdaParameters,
                type ?? GetExpressionType<T>(functionTokens, sourceParameter, lambdaParameters),
                formatProvider);

            var right = left == null
                            ? null
                            : CreateExpression<T>(
                                functionTokens.Right,
                                sourceParameter,
                                lambdaParameters,
                                GetFunctionParameterType(functionTokens.Operation) ?? left.Type,
                                formatProvider);

            return left == null
                ? null
                : GetFunction(functionTokens.Operation, left, right, sourceParameter, lambdaParameters);
        }

        /// <summary>
        /// Used to get the ParameterExpressions used in an Expression so that Expression.Call will have the correct number of parameters supplied.
        /// </summary>
        private class ParameterVisitor : ExpressionVisitor
        {
            private static readonly string[] AnyAllMethodNames = { "Any", "All" };
            private List<ParameterExpression> _parameters;

            public IEnumerable<ParameterExpression> GetParameters(Expression expr)
            {
                CustomContract.Requires(expr != null);
                CustomContract.Ensures(CustomContract.Result<IEnumerable<ParameterExpression>>() != null);

                _parameters = new List<ParameterExpression>();
                Visit(expr);
                return _parameters;
            }

            public override Expression Visit(Expression node)
            {
                CustomContract.Assume(node != null);

                if (node.NodeType == ExpressionType.Call && AnyAllMethodNames.Contains(((MethodCallExpression)node).Method.Name))
                {
                    // Skip the second parameter of the Any/All as this has already been covered
                    return base.Visit(((MethodCallExpression)node).Arguments.First());
                }

                return base.Visit(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                CustomContract.Assume(node != null);

                if (node.NodeType == ExpressionType.AndAlso)
                {
                    Visit(node.Left);
                    Visit(node.Right);
                    return node;
                }

                return base.VisitBinary(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                CustomContract.Assume(node != null);
                CustomContract.Assume(_parameters != null);

                if (!_parameters.Contains(node))
                {
                    _parameters.Add(node);
                }

                return base.VisitParameter(node);
            }
        }
    }
}