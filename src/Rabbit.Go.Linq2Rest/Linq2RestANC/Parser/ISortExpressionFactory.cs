// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISortExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for a SortExpressionFactory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the public interface for a SortExpressionFactory.
	/// </summary>
	public interface ISortExpressionFactory
	{
		/// <summary>
		/// Creates an enumeration of sort descriptions from its string representation.
		/// </summary>
		/// <param name="filter">The string representation of the sort descriptions.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item to sort.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> if the passed sort descriptions are valid, otherwise null.</returns>
		IEnumerable<SortDescription<T>> Create<T>(string filter);
	}
}