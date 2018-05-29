﻿using Microsoft.Extensions.Primitives;
using Rabbit.Go.Features;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rabbit.Go.Abstractions.Features
{
    public class QueryCollection : IQueryCollection
    {
        public static readonly QueryCollection Empty = new QueryCollection();
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

        private Dictionary<string, StringValues> Store { get; set; }

        public QueryCollection()
        {
        }

        public QueryCollection(Dictionary<string, StringValues> store)
        {
            Store = store;
        }

        public QueryCollection(QueryCollection store)
        {
            Store = store.Store;
        }

        public QueryCollection(int capacity)
        {
            Store = new Dictionary<string, StringValues>(capacity, StringComparer.OrdinalIgnoreCase);
        }

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
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="HeaderDictionary"/>;.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="HeaderDictionary"/>.</returns>
        public int Count
        {
            get
            {
                if (Store == null)
                {
                    return 0;
                }
                return Store.Count;
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
        /// An <see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.
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