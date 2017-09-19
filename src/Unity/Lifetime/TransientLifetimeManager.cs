// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using ObjectBuilder2;

namespace Unity
{
    /// <summary>
    /// An <see cref="LifetimeManager"/> implementation that does nothing,
    /// thus ensuring that instances are created new every time.
    /// </summary>
    public class TransientLifetimeManager : LifetimeManager, ITransientPolicy
    {
        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue()
        {
            return null;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
        }
    }
}
