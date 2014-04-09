// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a lifetime container.
    /// </summary>
    /// <remarks>
    /// A lifetime container tracks the lifetime of an object, and implements
    /// IDisposable. When the container is disposed, any objects in the
    /// container which implement IDisposable are also disposed.
    /// </remarks>
    // FxCop suppression: No
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class LifetimeContainer : ILifetimeContainer
    {
        private readonly List<object> items = new List<object>();

        /// <summary>
        /// Gets the number of references in the lifetime container
        /// </summary>
        /// <value>
        /// The number of references in the lifetime container
        /// </value>
        public int Count
        {
            get
            {
                lock (items)
                {
                    return items.Count;
                }
            }
        }

        /// <summary>
        /// Adds an object to the lifetime container.
        /// </summary>
        /// <param name="item">The item to be added to the lifetime container.</param>
        public void Add(object item)
        {
            lock (items)
            {
                items.Add(item);
            }
        }

        /// <summary>
        /// Determine if a given object is in the lifetime container.
        /// </summary>
        /// <param name="item">
        /// The item to locate in the lifetime container.
        /// </param>
        /// <returns>
        /// Returns true if the object is contained in the lifetime
        /// container; returns false otherwise.
        /// </returns>
        public bool Contains(object item)
        {
            lock (items)
            {
                return items.Contains(item);
            }
        }

        /// <summary>
        /// Releases the resources used by the <see cref="LifetimeContainer"/>. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Doesn't matter, but shuts up FxCop
        }

        /// <summary>
        /// Releases the resources used by the <see cref="LifetimeContainer"/>. 
        /// </summary>
        /// <param name="disposing">
        /// true to release managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (items)
                {
                    var itemsCopy = new List<object>(items);
                    itemsCopy.Reverse();

                    foreach (object o in itemsCopy)
                    {
                        var d = o as IDisposable;

                        if (d != null)
                        {
                            d.Dispose();
                        }
                    }

                    items.Clear();
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the lifetime container.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the life time container. 
        /// </returns>
        public IEnumerator<object> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the lifetime container.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the life time container. 
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes an item from the lifetime container. The item is
        /// not disposed.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void Remove(object item)
        {
            lock (items)
            {
                if (!items.Contains(item))
                {
                    return;
                }

                items.Remove(item);
            }
        }
    }
}
