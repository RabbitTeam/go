﻿using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Rabbit.Go.Features;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rabbit.Go.Abstractions
{
    public class HeaderDictionary : IHeaderDictionary
    {
#if NETSTANDARD1_3
        private static readonly string[] EmptyKeys = Array.Empty<string>();
        private static readonly StringValues[] EmptyValues = Array.Empty<StringValues>();
#else
        private static readonly string[] EmptyKeys = new string[0];
        private static readonly StringValues[] EmptyValues = new StringValues[0];
#endif
        private static readonly Enumerator EmptyEnumerator = new Enumerator();

        // Pre-box
        private static readonly IEnumerator<KeyValuePair<string, StringValues>> EmptyIEnumeratorType = EmptyEnumerator;

        private static readonly IEnumerator EmptyIEnumerator = EmptyEnumerator;

        public HeaderDictionary()
        {
        }

        public HeaderDictionary(Dictionary<string, StringValues> store)
        {
            Store = store;
        }

        public HeaderDictionary(int capacity)
        {
            Store = new Dictionary<string, StringValues>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        private Dictionary<string, StringValues> Store { get; set; }

        /// <summary>
        /// Get or sets the associated value from the collection as a single string.
        /// </summary>
        /// <param name="key">The header name.</param>
        /// <returns>
        /// the associated value from the collection as a StringValues or StringValues.Empty if the
        /// key is not present.
        /// </returns>
        public StringValues this[string key]
        {
            get
            {
                if (Store == null)
                {
                    return StringValues.Empty;
                }

                StringValues value;
                if (TryGetValue(key, out value))
                {
                    return value;
                }
                return StringValues.Empty;
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (StringValues.IsNullOrEmpty(value))
                {
                    Store?.Remove(key);
                }
                else
                {
                    if (Store == null)
                    {
                        Store = new Dictionary<string, StringValues>(1, StringComparer.OrdinalIgnoreCase);
                    }

                    Store[key] = value;
                }
            }
        }

        /// <summary>
        /// Throws KeyNotFoundException if the key is not present.
        /// </summary>
        /// <param name="key">The header name.</param>
        /// <returns></returns>
        StringValues IDictionary<string, StringValues>.this[string key]
        {
            get { return Store[key]; }
            set { this[key] = value; }
        }

        public long? ContentLength
        {
            get
            {
                long value;
                var rawValue = this[HeaderNames.ContentLength];
                if (rawValue.Count == 1 &&
                    !string.IsNullOrWhiteSpace(rawValue[0]) &&
                    HeaderUtilities.TryParseNonNegativeInt64(new StringSegment(rawValue[0]).Trim(), out value))
                {
                    return value;
                }

                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    this[HeaderNames.ContentLength] = HeaderUtilities.FormatNonNegativeInt64(value.Value);
                }
                else
                {
                    this.Remove(HeaderNames.ContentLength);
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="HeaderDictionary"/>;.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="HeaderDictionary"/>.</returns>
        public int Count
        {
            get
            {
                return Store?.Count ?? 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="HeaderDictionary"/> is in read-only mode.
        /// </summary>
        /// <returns>
        /// true if the <see cref="HeaderDictionary"/> is in read-only mode; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                if (Store == null)
                {
                    return EmptyKeys;
                }
                return Store.Keys;
            }
        }

        public ICollection<StringValues> Values
        {
            get
            {
                if (Store == null)
                {
                    return EmptyValues;
                }
                return Store.Values;
            }
        }

        /// <summary>
        /// Adds a new list of items to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(KeyValuePair<string, StringValues> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException("The key is null");
            }
            if (Store == null)
            {
                Store = new Dictionary<string, StringValues>(1, StringComparer.OrdinalIgnoreCase);
            }
            Store.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds the given header and values to the collection.
        /// </summary>
        /// <param name="key">The header name.</param>
        /// <param name="value">The header values.</param>
        public void Add(string key, StringValues value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (Store == null)
            {
                Store = new Dictionary<string, StringValues>(1);
            }
            Store.Add(key, value);
        }

        /// <summary>
        /// Clears the entire list of objects.
        /// </summary>
        public void Clear()
        {
            Store?.Clear();
        }

        /// <summary>
        /// Returns a value indicating whether the specified object occurs within this collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the specified object occurs within this collection; otherwise, false.</returns>
        public bool Contains(KeyValuePair<string, StringValues> item)
        {
            StringValues value;
            if (Store == null ||
                !Store.TryGetValue(item.Key, out value) ||
                !StringValues.Equals(value, item.Value))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the <see cref="HeaderDictionary"/> contains a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// true if the <see cref="HeaderDictionary"/> contains a specific key; otherwise, false.
        /// </returns>
        public bool ContainsKey(string key)
        {
            if (Store == null)
            {
                return false;
            }
            return Store.ContainsKey(key);
        }

        /// <summary>
        /// Copies the <see cref="HeaderDictionary"/> elements to a one-dimensional Array instance at
        /// the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the specified objects copied from
        /// the <see cref="HeaderDictionary"/>.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
            if (Store == null)
            {
                return;
            }

            foreach (var item in Store)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Removes the given item from the the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// true if the specified object was removed from the collection; otherwise, false.
        /// </returns>
        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            if (Store == null)
            {
                return false;
            }

            StringValues value;

            if (Store.TryGetValue(item.Key, out value) && StringValues.Equals(item.Value, value))
            {
                return Store.Remove(item.Key);
            }
            return false;
        }

        /// <summary>
        /// Removes the given header from the collection.
        /// </summary>
        /// <param name="key">The header name.</param>
        /// <returns>
        /// true if the specified object was removed from the collection; otherwise, false.
        /// </returns>
        public bool Remove(string key)
        {
            if (Store == null)
            {
                return false;
            }
            return Store.Remove(key);
        }

        /// <summary>
        /// Retrieves a value from the dictionary.
        /// </summary>
        /// <param name="key">The header name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if the <see cref="HeaderDictionary"/> contains the key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out StringValues value)
        {
            if (Store == null)
            {
                value = default(StringValues);
                return false;
            }
            return Store.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="Enumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public Enumerator GetEnumerator()
        {
            if (Store == null || Store.Count == 0)
            {
                // Non-boxed Enumerator
                return EmptyEnumerator;
            }
            return new Enumerator(Store.GetEnumerator());
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<string, StringValues>> IEnumerable<KeyValuePair<string, StringValues>>.GetEnumerator()
        {
            if (Store == null || Store.Count == 0)
            {
                // Non-boxed Enumerator
                return EmptyIEnumeratorType;
            }
            return Store.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Store == null || Store.Count == 0)
            {
                // Non-boxed Enumerator
                return EmptyIEnumerator;
            }
            return Store.GetEnumerator();
        }

        public struct Enumerator : IEnumerator<KeyValuePair<string, StringValues>>
        {
            // Do NOT make this readonly, or MoveNext will not work
            private Dictionary<string, StringValues>.Enumerator _dictionaryEnumerator;

            private bool _notEmpty;

            internal Enumerator(Dictionary<string, StringValues>.Enumerator dictionaryEnumerator)
            {
                _dictionaryEnumerator = dictionaryEnumerator;
                _notEmpty = true;
            }

            public bool MoveNext()
            {
                if (_notEmpty)
                {
                    return _dictionaryEnumerator.MoveNext();
                }
                return false;
            }

            public KeyValuePair<string, StringValues> Current
            {
                get
                {
                    if (_notEmpty)
                    {
                        return _dictionaryEnumerator.Current;
                    }
                    return default(KeyValuePair<string, StringValues>);
                }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_notEmpty)
                {
                    ((IEnumerator)_dictionaryEnumerator).Reset();
                }
            }
        }
    }
}