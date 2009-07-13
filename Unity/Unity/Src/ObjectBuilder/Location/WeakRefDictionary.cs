//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a dictionary which stores the values as weak references instead of strong
    /// references. Null values are supported.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification = "Just because it doesn't implement IDictionary doesn't mean it's not a dictionary")]
    public class WeakRefDictionary<TKey, TValue>
    {
        readonly Dictionary<TKey, WeakReference> inner = new Dictionary<TKey, WeakReference>();

        /// <summary>
        /// Returns a count of the number of items in the dictionary.
        /// </summary>
        /// <remarks>
        /// Since the items in the dictionary are held by weak reference, the count value
        /// cannot be relied upon to guarantee the number of objects that would be discovered via
        /// enumeration. Treat the Count as an estimate only.
        /// </remarks>
        public int Count
        {
            get
            {
                CleanAbandonedItems();
                return inner.Count;
            }
        }

        /// <summary>
        /// Retrieves a value from the dictionary.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value in the dictionary.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the key does exist in the dictionary.
        /// Since the dictionary contains weak references, the key may have been removed by the
        /// garbage collection of the object.</exception>
        public TValue this[TKey key]
        {
            get
            {
                TValue result;

                if (TryGet(key, out result))
                    return result;

                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Gets the count of the number of items in the dictionary.
        /// </summary>
        /// <value>
        /// The count of the number of items in the dictionary.
        /// </value>
        /// <remarks>
        /// Since the items in the dictionary are held by weak reference, the count value
        /// cannot be relied upon to guarantee the number of objects that would be discovered via
        /// enumeration. Treat the Count as an estimate only.
        /// </remarks>
        public void Add(TKey key,
                        TValue value)
        {
            TValue dummy;

            if (TryGet(key, out dummy))
                throw new ArgumentException(Resources.KeyAlreadyPresent);

            inner.Add(key, new WeakReference(EncodeNullObject(value)));
        }

        void CleanAbandonedItems()
        {
            List<TKey> deadKeys = new List<TKey>();

            foreach (KeyValuePair<TKey, WeakReference> kvp in inner)
                if (kvp.Value.Target == null)
                    deadKeys.Add(kvp.Key);

            foreach (TKey key in deadKeys)
                inner.Remove(key);
        }

        /// <summary>
        /// Determines if the dictionary contains a value for the key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>true if the key is contained in the dictionary; otherwise, false.</returns>
        public bool ContainsKey(TKey key)
        {
            TValue dummy;
            return TryGet(key, out dummy);
        }

        static TObject DecodeNullObject<TObject>(object innerValue)
        {
            if (innerValue == typeof(NullObject))
                return default(TObject);
            else
                return (TObject)innerValue;
        }

        static object EncodeNullObject(object value)
        {
            if (value == null)
                return typeof(NullObject);
            else
                return value;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the dictionary. 
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Standard interface for dictionary like types.")]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, WeakReference> kvp in inner)
            {
                object innerValue = kvp.Value.Target;

                if (innerValue != null)
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, DecodeNullObject<TValue>(innerValue));
            }
        }

        /// <summary>
        /// Removes an item from the dictionary.
        /// </summary>
        /// <param name="key">The key of the item to be removed.</param>
        /// <returns>Returns true if the key was in the dictionary; return false otherwise.</returns>
        public bool Remove(TKey key)
        {
            return inner.Remove(key);
        }

        /// <summary>
        /// Attempts to get a value from the dictionary.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>Returns true if the value was present; false otherwise.</returns>
        public bool TryGet(TKey key,
                           out TValue value)
        {
            value = default(TValue);
            WeakReference wr;

            if (!inner.TryGetValue(key, out wr))
                return false;

            object result = wr.Target;

            if (result == null)
            {
                inner.Remove(key);
                return false;
            }

            value = DecodeNullObject<TValue>(result);
            return true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
            Justification="Type object is used as a marker")]
        class NullObject {}
    }
}
