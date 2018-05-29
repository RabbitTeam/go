// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueExpressionFactory.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IValueExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	/// <summary>
	/// Custom value expression factory.
	/// </summary>
	[ContractClass(typeof(ValueExpressionFactoryContracts))]
	public interface IValueExpressionFactory
	{
		/// <summary>
		/// Returns a value indicating whether the factory can handle a give <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to check.</param>
		/// <returns><code>true</code> if the <see cref="Type"/> can be handled, otherwise <code>false</code>.</returns>
		bool Handles(Type type);

	    /// <summary>
	    /// Converts the passed OData style value to an <see cref="Expression"/>.
	    /// </summary>
	    /// <param name="token">The value token to convert.</param>
	    /// <param name="type">The value type.</param>
	    /// <returns>The value as a <see cref="ConstantExpression"/>.</returns>
	    ConstantExpression Convert(string token, Type type);
	}

	[ContractClassFor(typeof(IValueExpressionFactory))]
	internal abstract class ValueExpressionFactoryContracts : IValueExpressionFactory
	{
		public bool Handles(Type type)
		{
			CustomContract.Requires(type != null);
			throw new NotImplementedException();
		}

		public ConstantExpression Convert(string token, Type type)
		{
			CustomContract.Requires(token != null);
			CustomContract.Ensures(CustomContract.Result<ConstantExpression>() != null);
			throw new NotImplementedException();
		}
	}
}