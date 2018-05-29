// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Diagnostics.Contracts;

#if NETFX_CORE
	using System.Reflection;
#endif

	/// <summary>
	/// Interface for handling writing values to OData format.
	/// </summary>
	[ContractClass(typeof(ValueWriterContracts))]
	public interface IValueWriter
	{
		/// <summary>
		/// Get whether a <see cref="Type"/> is handled.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to check.</param>
		/// <returns></returns>
		bool Handles(Type type);

		/// <summary>
		/// Writes the passed value to an OData style string representation.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <returns>The OData style representation of the passed value.</returns>
		string Write(object value);
	}

	[ContractClassFor(typeof(IValueWriter))]
	internal abstract class ValueWriterContracts : IValueWriter
	{
		public bool Handles(Type type)
		{
			CustomContract.Requires(type != null);

			throw new NotImplementedException();
		}

		public string Write(object value)
		{
			CustomContract.Requires(value != null);
			CustomContract.Ensures(CustomContract.Result<string>() != null);

			throw new NotImplementedException();
		}
	}
}