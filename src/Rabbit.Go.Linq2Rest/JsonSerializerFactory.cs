using Linq2Rest.Provider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rabbit.Go.Linq2Rest
{
    internal class GoSerializerFactory : ISerializerFactory
    {
        public static ISerializerFactory SerializerFactory { get; } = new GoSerializerFactory();

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
                return Deserialize<T>(input);
            }

            /// <summary>
            /// Deserializes a list of items.
            /// </summary>
            /// <param name="input">The serialized items.</param>
            /// <returns>An list of the serialized items.</returns>
            public IEnumerable<T> DeserializeList(Stream input)
            {
                return Deserialize<T[]>(input) ?? Enumerable.Empty<T>();
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

            #region Private Method

            private static TType Deserialize<TType>(Stream input)
            {
                if (input == Stream.Null)
                    return default(TType);

                using (var reader = new StreamReader(input, Encoding.UTF8))
                {
                    var json = string.Empty;
                    try
                    {
                        json = reader.ReadToEnd();

                        return JsonConvert.DeserializeObject<TType>(json);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException($"反序列化为：{typeof(TType).Name} 时发生了错误，json：{json}", e);
                    }
                }
            }

            #endregion Private Method
        }
    }
}