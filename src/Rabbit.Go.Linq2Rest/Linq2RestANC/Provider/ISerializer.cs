// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for an object serializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;

	/// <summary>
	/// Defines the public interface for an object serializer.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[ContractClass(typeof(SerializerContracts<>))]
	public interface ISerializer<T>
	{
		/// <summary>
		/// Deserializes a single item.
		/// </summary>
		/// <param name="input">The serialized item.</param>
		/// <returns>An instance of the serialized item.</returns>
		T Deserialize(Stream input);

		/// <summary>
		/// Deserializes a list of items.
		/// </summary>
		/// <param name="input">The serialized items.</param>
		/// <returns>An list of the serialized items.</returns>
		IEnumerable<T> DeserializeList(Stream input);

		/// <summary>
		/// Serializes the passed item into a <see cref="Stream"/>.
		/// </summary>
		/// <param name="item">The item to serialize.</param>
		/// <returns>A <see cref="Stream"/> representation of the item.</returns>
		Stream Serialize(T item);
	}

	[ContractClassFor(typeof(ISerializer<>))]
	internal abstract class SerializerContracts<T> : ISerializer<T>
	{
		/// <summary>
		/// Deserializes a single item.
		/// </summary>
		/// <param name="input">The serialized item.</param>
		/// <returns>An instance of the serialized item.</returns>
		public T Deserialize(Stream input)
		{
			CustomContract.Requires<ArgumentNullException>(input != null);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Deserializes a list of items.
		/// </summary>
		/// <param name="input">The serialized items.</param>
		/// <returns>An list of the serialized items.</returns>
		public IEnumerable<T> DeserializeList(Stream input)
		{
			CustomContract.Requires<ArgumentNullException>(input != null);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Serializes the passed item into a <see cref="Stream"/>.
		/// </summary>
		/// <param name="item">The item to serialize.</param>
		/// <returns>A <see cref="Stream"/> representation of the item.</returns>
		public Stream Serialize(T item)
		{
			CustomContract.Requires<ArgumentNullException>(!ReferenceEquals(item, null));

			throw new NotImplementedException();
		}
	}
}