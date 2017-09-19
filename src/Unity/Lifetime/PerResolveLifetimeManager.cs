// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System.Diagnostics;


namespace Unity
{
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    public class PerResolveLifetimeManager : LifetimeManager
    {
        // TODO: Use InUse instead
        private bool isSet;

        private object value;

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue()
        {
            return value;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later. In this class,
        /// this is a noop, since it has special hooks down in the guts.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            Debug.Assert(!isSet, "The value for PerResolveLifetimeManager can only be set once");

            isSet = true;
            value = newValue;
        }

        /// <summary>
        /// Remove the given object from backing store. Noop in this class.
        /// </summary>
        public override void RemoveValue()
        {
        }
    }
}
