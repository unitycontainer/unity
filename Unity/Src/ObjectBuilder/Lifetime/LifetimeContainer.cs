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
		readonly List<object> items = new List<object>();

        /// <summary>
        /// Gets the number of references in the lifetime container
        /// </summary>
        /// <value>
        /// The number of references in the lifetime container
        /// </value>
		public int Count
		{
			get { return items.Count; }
		}

        /// <summary>
        /// Adds an object to the lifetime container.
        /// </summary>
        /// <param name="item">The item to be added to the lifetime container.</param>
		public void Add(object item)
		{
			items.Add(item);
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
			return items.Contains(item);
		}

        /// <summary>
        /// Releases the resources used by the <see cref="LifetimeContainer"/>. 
        /// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

        /// <summary>
        /// Releases the managed resources used by the DbDataReader and optionally releases the unmanaged resources. 
        /// </summary>
        /// <param name="disposing">
        /// true to release managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				List<object> itemsCopy = new List<object>(items);
				itemsCopy.Reverse();

				foreach (object o in itemsCopy)
				{
					IDisposable d = o as IDisposable;

					if (d != null)
						d.Dispose();
				}

				items.Clear();
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
			if (!items.Contains(item))
				return;

			items.Remove(item);
		}
	}
}
