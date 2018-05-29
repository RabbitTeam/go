// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringContainsMethodWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringContainsMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class StringContainsMethodWriter : IMethodCallWriter
	{
		public bool CanHandle(MethodCallExpression expression)
		{
			CustomContract.Assert(expression.Method != null);

			return expression.Method.DeclaringType == typeof(string)
				   && expression.Method.Name == "Contains";
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			CustomContract.Assert(expression.Arguments != null);
			CustomContract.Assume(expression.Arguments.Count > 0);

			var argumentExpression = expression.Arguments[0];
			var obj = expression.Object;

			CustomContract.Assume(obj != null);
			CustomContract.Assume(argumentExpression != null);

			return string.Format(
				"substringof({0}, {1})", 
				expressionWriter(argumentExpression), 
				expressionWriter(obj));
		}
	}
}