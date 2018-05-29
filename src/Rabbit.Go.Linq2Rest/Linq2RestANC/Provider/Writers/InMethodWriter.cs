using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Linq2Rest.Provider.Writers
{
    internal class InMethodWriter : IMethodCallWriter
    {
        #region Implementation of IMethodCallWriter

        public bool CanHandle(MethodCallExpression expression)
        {
            CustomContract.Assert(expression.Method != null);

            return expression.Method.DeclaringType == typeof(Enumerable) && expression.Method.Name == "Contains";
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            CustomContract.Assume(expression.Arguments.Count > 1);

            var array = (MemberExpression)expression.Arguments[0];
            var property = expression.Arguments[1];

            var field = (FieldInfo)array.Member;
            var constantExpression = (ConstantExpression)array.Expression;

            var values = (IEnumerable)field.GetValue(constantExpression.Value);
            var str = string.Join(" ", values.Cast<object>());

            var propertyName = expressionWriter(property);

            return $"in({propertyName}, {str})";
        }

        #endregion Implementation of IMethodCallWriter
    }
}