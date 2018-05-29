// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathMethodWriter.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MathMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	[ContractClass(typeof(MathMethodWriterContracts))]
	internal abstract class MathMethodWriter : IMethodCallWriter
	{
		protected abstract string MethodName { get; }

		public abstract bool CanHandle(MethodCallExpression expression);

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			CustomContract.Assume(expression.Arguments.Count > 0);

			var mathArgument = expression.Arguments[0];

			CustomContract.Assume(mathArgument != null);

			return string.Format("{0}({1})", MethodName, expressionWriter(mathArgument));
		}
	}

	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Contract class.")]
	[ContractClassFor(typeof(MathMethodWriter))]
	internal abstract class MathMethodWriterContracts : MathMethodWriter
	{
		[Pure]
		protected override string MethodName
		{
			get
			{
				CustomContract.Ensures(!string.IsNullOrWhiteSpace(CustomContract.Result<string>()));
				throw new NotImplementedException();
			}
		}
	}
}