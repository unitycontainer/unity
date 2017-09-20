// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds a weak reference to
    /// it's managed instance.
    /// </summary>
    public class ExternallyControlledLifetimeManager : LifetimeManager
    {
        private WeakReference value = new WeakReference(null);

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue()
        {
            return this.value.Target;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            this.value = new WeakReference(newValue);
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            // Deliberate Noop - we don't own this instance after all.
        }
    }
}
