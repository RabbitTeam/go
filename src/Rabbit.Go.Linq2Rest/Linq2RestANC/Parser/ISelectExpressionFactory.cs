// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISelectExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for a SelectExpressionFactory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
	using System;
	using System.Linq.Expressions;

	/// <summary>
	/// Defines the public interface for a SelectExpressionFactory.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to create expression for.</typeparam>
	public interface ISelectExpressionFactory<T>
	{
		/// <summary>
		/// Creates a select expression.
		/// </summary>
		/// <param name="selection">The properties to select.</param>
		/// <returns>An instance of a <see cref="Func{T1,TResult}"/>.</returns>
		Expression<Func<T, object>> Create(string selection);
	}
}