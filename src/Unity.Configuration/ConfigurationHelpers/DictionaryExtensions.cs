// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A couple of useful extension methods on IDictionary
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Get the value from a dictionary, or null if there is no value.
        /// </summary>
        /// <typeparam name="TKey">Key type of dictionary.</typeparam>
        /// <typeparam name="TValue">Value type of dictionary.</typeparam>
        /// <param name="dictionary">Dictionary to search.</param>
        /// <param name="key">Key to look up.</param>
        /// <returns>The value at the key or null if not in the dictionary.</returns>
        public static TValue GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            Unity.Utility.Guard.ArgumentNotNull(dictionary, "dictionary");

            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return default(TValue);
        }
    }
}
