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
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IReadableLocator"/> that wraps an existing locator
    /// to ensure items are not written into the locator.
    /// </summary>
    // FxCop suppression: No
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class ReadOnlyLocator : ReadableLocator
	{
		readonly IReadableLocator innerLocator;

        /// <summary>
        /// Initialize a new instance of the <see cref="ReadableLocator"/> class with an <see cref="IReadableLocator"/> to wrap.
        /// </summary>
        /// <param name="innerLocator">The inner locator to be wrapped.</param>
		public ReadOnlyLocator(IReadableLocator innerLocator)
		{
			Guard.ArgumentNotNull(innerLocator, "innerLocator");

			this.innerLocator = innerLocator;
		}

        /// <summary>
        /// Gets the number of items in the locator.
        /// </summary>
        /// <value>
        /// The number of items in the locator.
        /// </value>
		public override int Count
		{
			get { return innerLocator.Count; }
		}

        /// <summary>
        /// Gets the parent locator.
        /// </summary>
        /// <value>
        /// The parent locator.
        /// </value>
		public override IReadableLocator ParentLocator
		{
			get { return innerLocator.ParentLocator == null ? null : new ReadOnlyLocator(innerLocator.ParentLocator); }
		}

        /// <summary>
        /// Determines if the locator is read-only.
        /// </summary>
        /// <value>
        /// true if the locator is read-only; otherwise, false.
        /// </value>
		public override bool ReadOnly
		{
			get { return true; }
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
			return innerLocator.Contains(key);
		}

        /// <summary>
        /// Gets an object from the locator, registered with the given key.
        /// </summary>
        /// <param name="key">The key that the object is registered with.</param>
        /// <returns>The object, if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
		public override object Get(object key)
		{
			return innerLocator.Get(key);
		}

        /// <summary>
        /// Returns an enumerator that iterates through the locator.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the locator. 
        /// </returns>
		public override IEnumerator<KeyValuePair<object, object>> GetEnumerator()
		{
			return innerLocator.GetEnumerator();
		}
	}
}
