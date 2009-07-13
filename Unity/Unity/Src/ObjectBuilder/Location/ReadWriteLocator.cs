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
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="IReadWriteLocator"/>.
    /// </summary>
    // FxCop suppression: No
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public abstract class ReadWriteLocator : ReadableLocator, IReadWriteLocator
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="ReadWriteLocator"/> class.
        /// </summary>
        protected ReadWriteLocator() {}

        /// <summary>
        /// Initialize a new instance of the <see cref="ReadWriteLocator"/> class with a parent <see cref="IReadableLocator"/>.
        /// </summary>
        /// <param name="parentLocator">A parent <see cref="IReadableLocator"/>.</param>
        protected ReadWriteLocator(IReadableLocator parentLocator)
            : base(parentLocator) {}

        /// <summary>
        /// Determines if the locator is read-only.
        /// </summary>
        /// <value>
        /// true if the locator is read-only; otherwise, false.
        /// </value>
        public override bool ReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds an object to the locator, with the given key.
        /// </summary>
        /// <param name="key">The key to register the object with.</param>
        /// <param name="value">The object to be registered.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> or value are null.</exception>
        public abstract void Add(object key,
                                 object value);

        /// <summary>
        /// Removes an object from the locator.
        /// </summary>
        /// <param name="key">The key under which the object was registered.</param>
        /// <returns>
        /// Returns true if the object was found in the locator; returns
        /// false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public abstract bool Remove(object key);
    }
}
