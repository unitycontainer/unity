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

using System.Collections.Generic;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static TValue GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(dictionary, "dictionary");

            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return default(TValue);
        }
    }
}
