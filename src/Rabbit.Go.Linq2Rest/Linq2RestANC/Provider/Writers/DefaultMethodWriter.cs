// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMethodWriter.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class DefaultMethodWriter : IMethodCallWriter
	{
		private readonly ParameterValueWriter _valueWriter;

		public DefaultMethodWriter(ParameterValueWriter valueWriter)
		{
			CustomContract.Requires(valueWriter != null);

			_valueWriter = valueWriter;
		}

		public bool CanHandle(MethodCallExpression expression)
		{
			return true;
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			return _valueWriter.Write(GetValue(expression));
		}

		private static object GetValue(Expression input)
		{
			CustomContract.Requires(input != null);

			var objectMember = Expression.Convert(input, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

			return getterLambda();
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_valueWriter != null);
		}
	}
}