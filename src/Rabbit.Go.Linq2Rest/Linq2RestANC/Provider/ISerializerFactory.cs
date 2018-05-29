// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializerFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for a factory of <see cref="ISerializer{T}" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// Defines the public interface for a factory of <see cref="ISerializer{T}"/>.
	/// </summary>
	[ContractClass(typeof(SerializerFactoryContracts))]
	public interface ISerializerFactory
	{
		/// <summary>
		/// Creates an instance of an <see cref="ISerializer{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type for the serializer.</typeparam>
		/// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
		ISerializer<T> Create<T>();

		/// <summary>
		/// Creates an instance of an <see cref="ISerializer{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type for the serializer.</typeparam>
		/// <typeparam name="TSource">The item type to provide alias metadata for the serializer.</typeparam>
		/// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
		ISerializer<T> Create<T, TSource>();
	}

	[ContractClassFor(typeof(ISerializerFactory))]
	internal abstract class SerializerFactoryContracts : ISerializerFactory
	{
		public ISerializer<T> Create<T>()
		{
			CustomContract.Ensures(CustomContract.Result<ISerializer<T>>() != null);
			throw new NotImplementedException();
		}

		public ISerializer<T> Create<T, TSource>()
		{
			CustomContract.Ensures(CustomContract.Result<ISerializer<T>>() != null);
			throw new NotImplementedException();
		}
	}
}