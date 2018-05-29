// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Provides a type matching the provided members.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Reflection;

	/// <summary>
	/// Provides a type matching the provided members.
	/// </summary>
	[ContractClass(typeof(RuntimeTypeProviderContracts))]
	public interface IRuntimeTypeProvider
	{
		/// <summary>
		/// Gets the <see cref="Type"/> matching the provided members.
		/// </summary>
		/// <param name="sourceType">The <see cref="Type"/> to generate the runtime type from.</param>
		/// <param name="properties">The <see cref="MemberInfo"/> to use to generate properties.</param>
		/// <returns>A <see cref="Type"/> mathing the provided properties.</returns>
		Type Get(Type sourceType, IEnumerable<MemberInfo> properties);
	}

	[ContractClassFor(typeof(IRuntimeTypeProvider))]
	internal abstract class RuntimeTypeProviderContracts : IRuntimeTypeProvider
	{
		public Type Get(Type sourceType, IEnumerable<MemberInfo> properties)
		{
			CustomContract.Requires<ArgumentNullException>(sourceType != null);
			CustomContract.Requires<ArgumentNullException>(properties != null);

			throw new NotImplementedException();
		}
	}
}