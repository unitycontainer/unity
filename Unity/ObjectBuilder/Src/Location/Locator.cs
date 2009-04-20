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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IReadableLocator"/> and <see cref="IReadWriteLocator"/>.
    /// </summary>
    // FxCop suppression: No
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class Locator : ReadWriteLocator
	{
		readonly WeakRefDictionary<object, object> references = new WeakRefDictionary<object, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Locator"/> class. 
        /// </summary>
        public Locator()
			: this(null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Locator"/> class as a child of the <paramref name="parentLocator"/>. 
        /// </summary>
        /// <param name="parentLocator">The parent locator.</param>
		public Locator(IReadableLocator parentLocator)
			: base(parentLocator) {}

        /// <summary>
        /// Gets the number of items in the locator.
        /// </summary>
        /// <value>
        /// The number of items in the locator.
        /// </value>
		public override int Count
		{
			get { return references.Count; }
		}

        /// <summary>
        /// Adds an object to the locator, with the given key.
        /// </summary>
        /// <param name="key">The key to register the object with.</param>
        /// <param name="value">The object to be registered.</param>
        /// <exception cref="ArgumentNullException">Key or value are null.</exception>
		public override void Add(object key,
		                         object value)
		{
			Guard.ArgumentNotNull(key, "key");
			Guard.ArgumentNotNull(value, "value");

			references.Add(key, value);
		}

        /// <summary>
        /// Determine if the locator contains an object for the given key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// true if the locator contains an object for the key; returns
        /// false otherwise.
        /// </returns>
        public override bool Contains(object key)
		{
			Guard.ArgumentNotNull(key, "key");

			if (references.ContainsKey(key))
				return true;

			if (ParentLocator != null)
				return ParentLocator.Contains(key);

			return false;
		}

        /// <summary>
        /// Gets an object from the locator, registered with the given key.
        /// </summary>
        /// <param name="key">The key that the object is registered with.</param>
        /// <returns>The object, if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
		public override object Get(object key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

            object value;
            if(references.TryGet(key, out value))
            {
                return value;
            }

			if (ParentLocator != null)
				return ParentLocator.Get(key);

			return null;
		}

        /// <summary>
        /// Returns an enumerator that iterates through a locator.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> object that can be used to iterate through the locator. 
        /// </returns>
		public override IEnumerator<KeyValuePair<object, object>> GetEnumerator()
		{
			return references.GetEnumerator();
		}

        /// <summary>
        /// Removes an object from the locator.
        /// </summary>
        /// <param name="key">The key under which the object was registered.</param>
        /// <returns>
        /// Returns true if the object was found in the locator; returns
        /// false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
		public override bool Remove(object key)
		{
			Guard.ArgumentNotNull(key, "key");

			return references.Remove(key);
		}
	}
}
