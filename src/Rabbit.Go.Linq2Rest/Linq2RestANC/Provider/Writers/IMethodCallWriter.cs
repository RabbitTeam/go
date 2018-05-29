// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodCallWriter.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IMethodCallWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	[ContractClass(typeof(MethodCallWriterContracts))]
	internal interface IMethodCallWriter
	{
		bool CanHandle(MethodCallExpression expression);

		string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter);
	}

	[ContractClassFor(typeof(IMethodCallWriter))]
	internal abstract class MethodCallWriterContracts : IMethodCallWriter
	{
		public bool CanHandle(MethodCallExpression expression)
		{
			CustomContract.Requires(expression != null);

			throw new NotImplementedException();
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			CustomContract.Requires(expression != null);
			CustomContract.Requires(expressionWriter != null);
			CustomContract.Ensures(CustomContract.Result<string>() != null);
			
			throw new NotImplementedException();
		}
	}
}