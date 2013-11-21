// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="ILifetimePolicy"/> that does nothing,
    /// ensuring that a new object gets created every time.
    /// </summary>
    public class TransientLifetimePolicy : ILifetimePolicy
    {
        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public object GetValue()
        {
            return null;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public void SetValue(object newValue)
        {
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public void RemoveValue()
        {
        }
    }
}
