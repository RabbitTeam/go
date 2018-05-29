// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringReplaceMethodWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringReplaceMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class StringReplaceMethodWriter : IMethodCallWriter
	{
		public bool CanHandle(MethodCallExpression expression)
		{
			CustomContract.Assert(expression.Method != null);

			return expression.Method.DeclaringType == typeof(string)
				   && expression.Method.Name == "Replace";
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			CustomContract.Assert(expression.Arguments != null);
			CustomContract.Assume(expression.Arguments.Count > 1);

			var firstArgument = expression.Arguments[0];
			var secondArgument = expression.Arguments[1];
			var obj = expression.Object;

			CustomContract.Assume(firstArgument != null);
			CustomContract.Assume(secondArgument != null);
			CustomContract.Assume(obj != null);

			return string.Format(
				"replace({0}, {1}, {2})", 
				expressionWriter(obj), 
				expressionWriter(firstArgument), 
				expressionWriter(secondArgument));
		}
	}
}