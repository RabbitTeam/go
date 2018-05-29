// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortExpressionFactory.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SortExpressionFactory?
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines the SortExpressionFactory?
    /// </summary>
    public class SortExpressionFactory : ISortExpressionFactory
    {
        private readonly IMemberNameResolver _nameResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortExpressionFactory"/> class.
        /// </summary>
        /// <param name="nameResolver">The <see cref="IMemberNameResolver"/> for name resolution.</param>
        public SortExpressionFactory(IMemberNameResolver nameResolver)
        {
            _nameResolver = nameResolver;
        }

        /// <summary>
        /// Creates an enumeration of sort descriptions from its string representation.
        /// </summary>
        /// <param name="filter">The string representation of the sort descriptions.</param>
        /// <typeparam name="T">The <see cref="Type"/> of item to sort.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> if the passed sort descriptions are valid, otherwise null.</returns>
        public IEnumerable<SortDescription<T>> Create<T>(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return new SortDescription<T>[0];
            }

            var parameterExpression = Expression.Parameter(typeof(T), "x");

            var sortTokens = filter.Split(',');

            return from sortToken in sortTokens
                   select sortToken.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                into sort
                   let property = GetSortExpression<T>(sort, parameterExpression)
                   where property != null
                   let direction = sort.LastOrDefault() == "desc" ? SortDirection.Descending : SortDirection.Ascending
                   select new SortDescription<T>(property, direction);
        }

        private Expression GetSortExpression<T>(IReadOnlyList<string> tokens, ParameterExpression parameterExpression)
        {
            if (tokens.Count <= 2)
                return GetPropertyLambdaExpression<T>(tokens.First(), parameterExpression);

            var left = GetPropertyExpression<T>(tokens[0], parameterExpression);
            var right = GetPropertyExpression<T>(tokens[2], parameterExpression);

            return GetCalculateExpression<T>(tokens[1], left.expression, right.expression, parameterExpression);
        }

        private Expression GetCalculateExpression<T>(string op, Expression left, Expression right, ParameterExpression parameterExpression)
        {
            var leftType = left.Type;
            var rightType = right.Type;

            TypeCode GetSimpleTypeCode(Type type)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return TypeCode.Int64;

                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return TypeCode.Decimal;

                    case TypeCode.DateTime:
                        return TypeCode.DateTime;

                    case TypeCode.String:
                        return TypeCode.String;
                }
                throw new NotSupportedException($"not supported type: {type}");
            }

            Expression GetConvertExpression(Expression expression, TypeCode typeCode)
            {
                var type = expression.Type;
                if (Type.GetTypeCode(type) == typeCode)
                    return expression;

                switch (typeCode)
                {
                    case TypeCode.Int64:
                        return Expression.Convert(expression, typeof(long));

                    case TypeCode.Decimal:
                        return Expression.Convert(expression, typeof(decimal));

                    case TypeCode.String:
                        return Expression.Call(expression, nameof(ToString), new Type[0]);

                    default:
                        return expression;
                }
            }

            var leftTypeCode = GetSimpleTypeCode(leftType);
            var rightTypeCode = GetSimpleTypeCode(rightType);

            bool AnyCode(TypeCode typeCode)
            {
                return leftTypeCode == typeCode || rightTypeCode == typeCode;
            }

            bool TrySet(TypeCode typeCode)
            {
                if (!AnyCode(typeCode))
                    return false;

                left = GetConvertExpression(left, typeCode);
                right = GetConvertExpression(right, typeCode);
                return true;
            }

            var returnType = leftType;

            if (leftType != rightType)
            {
                if (TrySet(TypeCode.String))
                    returnType = typeof(string);
                if (TrySet(TypeCode.Int64))
                    returnType = typeof(long);
                if (TrySet(TypeCode.Decimal))
                    returnType = typeof(decimal);
            }
            Expression result = null;
            switch (op.ToLower())
            {
                case "add":
                    result = Expression.Add(left, right);
                    break;

                case "sub":
                    result = Expression.Subtract(left, right);
                    break;

                case "mul":
                    result = Expression.Multiply(left, right);
                    break;

                case "div":
                    result = Expression.Divide(left, right);
                    break;

                case "mod":
                    result = Expression.Modulo(left, right);
                    break;
            }

            var funcType = typeof(Func<,>).MakeGenericType(typeof(T), returnType);

            return Expression.Lambda(funcType, result, parameterExpression);
        }

        private (Expression expression, Type memberType) GetPropertyExpression<T>(string propertyToken, ParameterExpression parameter)
        {
            if (string.IsNullOrWhiteSpace(propertyToken))
            {
                return (null, null);
            }

            var parentType = typeof(T);

            var propertyChain = propertyToken.Split('/');
            var result = _nameResolver.CreateMemberExpression(parameter, propertyChain, parentType, null);
            var propertyExpression = result.Item2;
            parentType = result.Item1;

            if (propertyExpression == null)
            {
                throw new FormatException(propertyToken + " is not recognized as a valid property");
            }

            return (propertyExpression, parentType);
        }

        private Expression GetPropertyLambdaExpression<T>(string propertyToken, ParameterExpression parameter)
        {
            (Expression propertyExpression, Type type) = GetPropertyExpression<T>(propertyToken, parameter);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(T), type);

            return Expression.Lambda(funcType, propertyExpression, parameter);
        }
    }
}