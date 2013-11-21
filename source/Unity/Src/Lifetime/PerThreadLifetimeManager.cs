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

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds the instances given to it, 
    /// keeping one instance per thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This LifetimeManager does not dispose the instances it holds.
    /// </para>
    /// </remarks>
    public class PerThreadLifetimeManager : LifetimeManager
    {
        [ThreadStatic]
        private static Dictionary<Guid, object> values;
        private readonly Guid key;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerThreadLifetimeManager"/> class.
        /// </summary>
        public PerThreadLifetimeManager()
        {
            this.key = Guid.NewGuid();
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy for the 
        /// current thread.
        /// </summary>
        /// <returns>the object desired, or <see langword="null"/> if no such object is currently 
        /// stored for the current thread.</returns>
        public override object GetValue()
        {
            EnsureValues();

            object result;
            values.TryGetValue(this.key, out result);
            return result;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later when requested
        /// in the current thread.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            EnsureValues();

            values[this.key] = newValue;
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        /// <remarks>Not implemented for this lifetime manager.</remarks>
        public override void RemoveValue()
        {
        }

        private static void EnsureValues()
        {
            // no need for locking, values is TLS
            if (values == null)
            {
                values = new Dictionary<Guid, object>();
            }
        }
    }
}
