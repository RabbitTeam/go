using Linq2Rest.Provider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rabbit.Go.Linq2Rest
{
    internal class GoSerializerFactory : ISerializerFactory
    {
        public static ISerializerFactory SerializerFactory { get; } = new GoSerializerFactory();
        private static readonly ConcurrentDictionary<Stream, object> Results = new ConcurrentDictionary<Stream, object>();

        public static void SetResult(Stream stream, object result)
        {
            Results.TryAdd(stream, result);
        }

        #region Implementation of ISerializerFactory

        /// <summary>
        /// Creates an instance of an <see cref="ISerializer{T}"/>.
        /// </summary>
        /// <typeparam name="T">The item type for the serializer.</typeparam>
        /// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
        public ISerializer<T> Create<T>()
        {
            return new JsonSerializer<T>();
        }

        /// <summary>
        /// Creates an instance of an <see cref="ISerializer{T}"/>.
        /// </summary>
        /// <typeparam name="T">The item type for the serializer.</typeparam>
        /// <typeparam name="TSource">The item type to provide alias metadata for the serializer.</typeparam>
        /// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
        public ISerializer<T> Create<T, TSource>()
        {
            return new JsonSerializer<T>();
        }

        #endregion Implementation of ISerializerFactory

        public class JsonSerializer<T> : ISerializer<T>
        {
            #region Implementation of ISerializer<T>

            /// <inheritdoc/>
            /// <summary>
            /// Deserializes a single item.
            /// </summary>
            /// <param name="input">The serialized item.</param>
            /// <returns>An instance of the serialized item.</returns>
            public T Deserialize(Stream input)
            {
                if (input == Stream.Null)
                    return default(T);
                return Results.TryRemove(input, out var value) ? (T)value : default(T);
            }

            /// <summary>
            /// Deserializes a list of items.
            /// </summary>
            /// <param name="input">The serialized items.</param>
            /// <returns>An list of the serialized items.</returns>
            public IEnumerable<T> DeserializeList(Stream input)
            {
                if (input == Stream.Null)
                    return Enumerable.Empty<T>();

                return Results.TryRemove(input, out var value) ? (IEnumerable<T>)value : Enumerable.Empty<T>();
            }

            /// <summary>
            /// Serializes the passed item into a <see cref="Stream"/>.
            /// </summary>
            /// <param name="item">The item to serialize.</param>
            /// <returns>A <see cref="Stream"/> representation of the item.</returns>
            public Stream Serialize(T item)
            {
                throw new NotImplementedException();
            }

            #endregion Implementation of ISerializer<T>
        }
    }
}