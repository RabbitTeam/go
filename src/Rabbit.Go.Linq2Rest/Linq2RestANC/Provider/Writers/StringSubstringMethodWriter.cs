// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringSubstringMethodWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringSubstringMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class StringSubstringMethodWriter : IMethodCallWriter
	{
		public bool CanHandle(MethodCallExpression expression)
		{
			CustomContract.Assert(expression.Method != null);

			return expression.Method.DeclaringType == typeof(string)
				   && expression.Method.Name == "Substring";
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			CustomContract.Assert(expression.Arguments != null);
			CustomContract.Assume(expression.Arguments.Count > 0);

			var obj = expression.Object;

			CustomContract.Assume(obj != null);

			if (expression.Arguments.Count == 1)
			{
				var argumentExpression = expression.Arguments[0];

				CustomContract.Assume(argumentExpression != null);

				return string.Format(
					"substring({0}, {1})", expressionWriter(obj), expressionWriter(argumentExpression));
			}

			var firstArgument = expression.Arguments[0];
			var secondArgument = expression.Arguments[1];

			CustomContract.Assume(firstArgument != null);
			CustomContract.Assume(secondArgument != null);

			return string.Format(
				"substring({0}, {1}, {2})", 
				expressionWriter(obj), 
				expressionWriter(firstArgument), 
				expressionWriter(secondArgument));
		}
	}
}