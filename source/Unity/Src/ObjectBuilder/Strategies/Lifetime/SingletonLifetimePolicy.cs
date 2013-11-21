// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="ILifetimePolicy"/> that stores objects in the locator and
    /// lifetime container provided by the context.
    /// </summary>
    public class SingletonLifetimePolicy : ILifetimePolicy, IRequiresRecovery
    {
        private object value;
        private object lockObject = new object();

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public object GetValue()
        {
            Monitor.Enter(lockObject);
            if(value != null)
            {
                Monitor.Exit(lockObject);
            }
            return value;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public void SetValue(object newValue)
        {
            this.value = newValue;
            TryExit();
        }


        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public void RemoveValue()
        {
            if(value is IDisposable)
            {
                ((IDisposable) value).Dispose();
            }
            value = null;
        }


        /// <summary>
        /// A method that does whatever is needed to clean up
        /// as part of cleaning up after an exception.
        /// </summary>
        /// <remarks>
        /// Don't do anything that could throw in this method,
        /// it will cause later recover operations to get skipped
        /// and play real havok with the stack trace.
        /// </remarks>
        public void Recover()
        {
            TryExit();
        }

        private void TryExit()
        {
            try
            {
                Monitor.Exit(lockObject);
            }
            catch (SynchronizationLockException)
            {
                // Ok to eat this, we weren't holding the lock at the time.
            }
        }
    }
}
