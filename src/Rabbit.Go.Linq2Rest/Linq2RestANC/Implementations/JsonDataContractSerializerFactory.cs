// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonDataContractSerializerFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the JsonDataContractSerializer factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Implementations
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization.Json;
	using Linq2Rest.Provider;

	/// <summary>
	/// Defines the JsonDataContractSerializer factory.
	/// </summary>
	public class JsonDataContractSerializerFactory : ISerializerFactory
	{
		private readonly IEnumerable<Type> _knownTypes;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonDataContractSerializerFactory"/> class.
		/// </summary>
		/// <param name="knownTypes">A number of known types for serialization resolution.</param>
		public JsonDataContractSerializerFactory(IEnumerable<Type> knownTypes)
		{
			CustomContract.Requires<ArgumentNullException>(knownTypes != null);

			_knownTypes = knownTypes;
		}

		/// <summary>
		/// Creates an instance of an <see cref="ISerializer{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type for the serializer.</typeparam>
		/// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
		public ISerializer<T> Create<T>()
		{
			return new JsonDataContractSerializer<T>(_knownTypes);
		}

		/// <summary>
		/// Creates an instance of an <see cref="ISerializer{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type for the serializer.</typeparam>
		/// <typeparam name="TSource">The item type to provide alias metadata for the serializer.</typeparam>
		/// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
		public ISerializer<T> Create<T, TSource>()
		{
			return Create<T>();
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_knownTypes != null);
		}

		private class JsonDataContractSerializer<T> : ISerializer<T>
		{
			private readonly DataContractJsonSerializer _listSerializer;
			private readonly DataContractJsonSerializer _serializer;

			public JsonDataContractSerializer(IEnumerable<Type> knownTypes)
			{
				CustomContract.Requires(knownTypes != null);

				var array = knownTypes.ToArray();
				_serializer = new DataContractJsonSerializer(typeof(T), array);
				_listSerializer = new DataContractJsonSerializer(typeof(List<T>), array);
			}

			public T Deserialize(Stream input)
			{
				var result = (T)_serializer.ReadObject(input);

				return result;
			}

			public IEnumerable<T> DeserializeList(Stream input)
			{
				var result = (List<T>)_listSerializer.ReadObject(input);

				return result;
			}

			public Stream Serialize(T item)
			{
				var stream = new MemoryStream();
				_serializer.WriteObject(stream, item);
				stream.Flush();
				stream.Position = 0;

				return stream;
			}

			[ContractInvariantMethod]
			private void Invariants()
			{
				CustomContract.Invariant(_serializer != null);
				CustomContract.Invariant(_listSerializer != null);
			}
		}
	}
}