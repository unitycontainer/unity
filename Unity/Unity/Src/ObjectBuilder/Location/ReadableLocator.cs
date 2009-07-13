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

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="IReadableLocator"/>.
    /// </summary>
    // FxCop suppression: No
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public abstract class ReadableLocator : IReadableLocator
    {
        readonly IReadableLocator parentLocator;

        /// <summary>
        /// Initialize a new instance of the <see cref="ReadableLocator"/> class.
        /// </summary>
        protected ReadableLocator() {}

        /// <summary>
        /// Initialize a new instance of the <see cref="ReadableLocator"/> class with a parent <see cref="IReadableLocator"/>.
        /// </summary>
        /// <param name="parentLocator">A parent <see cref="IReadableLocator"/>.</param>
        protected ReadableLocator(IReadableLocator parentLocator)
        {
            this.parentLocator = parentLocator;
        }

        /// <summary>
        /// Gets the number of items in the locator.
        /// </summary>
        /// <value>
        /// The number of items in the locator.
        /// </value>
        public abstract int Count { get; }

        /// <summary>
        /// Gets the parent locator.
        /// </summary>
        /// <value>
        /// The parent locator.
        /// </value>
        public virtual IReadableLocator ParentLocator
        {
            get { return parentLocator; }
        }

        /// <summary>
        /// Determines if the locator is read-only.
        /// </summary>
        /// <value>
        /// true if the locator is read-only; otherwise, false.
        /// </value>
        public abstract bool ReadOnly { get; }

        /// <summary>
        /// Determine if the locator contains an object for the given key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// true if the locator contains an object for the key; returns
        /// false otherwise.
        /// </returns>
        public abstract bool Contains(object key);

        /// <summary>
        /// Finds objects in the locator using the predicate, and returns a temporary locator
        /// filled with the found objects.
        /// </summary>
        /// <param name="predicate">The predicate to test whether to include an object.</param>
        /// <returns>The new locator</returns>
        /// <exception cref="ArgumentNullException">Predicate is null.</exception>
        public IReadableLocator FindBy(Predicate<KeyValuePair<object, object>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            Locator results = new Locator();
            IReadableLocator currentLocator = this;

            while (currentLocator != null)
            {
                FindInLocator(predicate, results, currentLocator);
                currentLocator = currentLocator.ParentLocator;
            }

            return new ReadOnlyLocator(results);
        }

        static void FindInLocator(Predicate<KeyValuePair<object, object>> predicate,
                                  IReadWriteLocator results,
                                  IEnumerable<KeyValuePair<object, object>> currentLocator)
        {
            foreach (KeyValuePair<object, object> kvp in currentLocator)
                if (!results.Contains(kvp.Key) && predicate(kvp))
                    results.Add(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Gets an object from the locator, registered with the key of typeof(T).
        /// </summary>
        /// <typeparam name="TItem">The type of the object to find.</typeparam>
        /// <returns>The object, if found; null otherwise.</returns>
        public TItem Get<TItem>()
        {
            return (TItem)Get(typeof(TItem));
        }

        /// <summary>
        /// Gets an object from the locator, registered with the given key.
        /// </summary>
        /// <typeparam name="TItem">The type of the object to find.</typeparam>
        /// <param name="key">The key that the object is registered with.</param>
        /// <returns>The object, if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        // FxCop suppression: The whole point of the overload is to vary by return type
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
        public TItem Get<TItem>(object key)
        {
            return (TItem)Get(key);
        }

        /// <summary>
        /// Gets an object from the locator, registered with the given key.
        /// </summary>
        /// <param name="key">The key that the object is registered with.</param>
        /// <returns>The object, if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        // FxCop suppression: The whole point of the overload is to vary by return type
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
        public abstract object Get(object key);

        /// <summary>
        /// Returns an enumerator that iterates through the locator.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the locator. 
        /// </returns>
        public abstract IEnumerator<KeyValuePair<object, object>> GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the locator.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the locator. 
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
