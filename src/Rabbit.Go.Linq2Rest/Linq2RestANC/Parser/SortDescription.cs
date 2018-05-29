// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortDescription.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines a sort description.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	/// <summary>
	/// Defines a sort description.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to sort.</typeparam>
	public class SortDescription<T>
	{
		private readonly SortDirection _direction;
		private readonly Expression _keySelector;

		/// <summary>
		/// Initializes a new instance of the <see cref="SortDescription{T}"/> class.
		/// </summary>
		/// <param name="keySelector">The function to select the sort key.</param>
		/// <param name="direction">The sort direction.</param>
		public SortDescription(Expression keySelector, SortDirection direction)
		{
			CustomContract.Requires<ArgumentNullException>(keySelector != null);

			_keySelector = keySelector;
			_direction = direction;
		}

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		public SortDirection Direction
		{
			get { return _direction; }
		}

		/// <summary>
		/// Gets the key to sort by.
		/// </summary>
		public Expression KeySelector
		{
			get { return _keySelector; }
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_keySelector != null);
		}
	}
}