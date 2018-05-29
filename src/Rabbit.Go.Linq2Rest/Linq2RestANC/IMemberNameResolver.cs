// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberNameResolver.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for a resolver of <see cref="MemberInfo" /> name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Reflection;

	/// <summary>
	/// Defines the public interface for a resolver of <see cref="MemberInfo"/> name.
	/// </summary>
	[ContractClass(typeof(MemberNameResolverContracts))]
	public interface IMemberNameResolver
	{
		/// <summary>
		/// Returns the resolved name for the <see cref="MemberInfo"/>.
		/// </summary>
		/// <param name="member">The <see cref="MemberInfo"/> to resolve the name of.</param>
		/// <returns>The resolved name.</returns>
		string ResolveName(MemberInfo member);

		/// <summary>
		/// Returns the resolved <see cref="MemberInfo"/> for an alias.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> the alias relates to.</param>
		/// <param name="alias">The name of the alias.</param>
		/// <returns>The <see cref="MemberInfo"/> which is aliased.</returns>
		MemberInfo ResolveAlias(Type type, string alias);
	}

	[ContractClassFor(typeof(IMemberNameResolver))]
	internal abstract class MemberNameResolverContracts : IMemberNameResolver
	{
		[Pure]
		public string ResolveName(MemberInfo member)
		{
			CustomContract.Requires<ArgumentNullException>(member != null);
			CustomContract.Ensures(CustomContract.Result<string>() != null);

			throw new NotImplementedException();
		}

		[Pure]
		public MemberInfo ResolveAlias(Type type, string alias)
		{
			CustomContract.Requires<ArgumentNullException>(type != null);
			CustomContract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(alias));
			throw new NotImplementedException();
		}
	}
}