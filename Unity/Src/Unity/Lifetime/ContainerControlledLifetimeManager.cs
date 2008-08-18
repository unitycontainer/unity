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
using System.Threading;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds onto the instance given to it.
    /// When the <see cref="ContainerControlledLifetimeManager"/> is disposed,
    /// the instance is disposed with it.
    /// </summary>
    public class ContainerControlledLifetimeManager : LifetimeManager, IDisposable, IRequiresRecovery
    {
        private object value;
        private object lockObject = new object();
        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue()
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
        public override void SetValue(object newValue)
        {
            value = newValue;
            TryExit(lockObject);
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            Dispose();
        }


        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Standard Dispose pattern implementation. Not needed, but it keeps FxCop happy.
        /// </summary>
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>
        // FxCop suppression: This method is only here to avoid the other IDisposable warning.
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        protected void Dispose(bool disposing)
        {
            if (value != null)
            {
                if (value is IDisposable)
                {
                    ((IDisposable) value).Dispose();
                }
                value = null;
            }
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
            TryExit(lockObject);
        }

        private static void TryExit(object lockObj)
        {
            try
            {
                Monitor.Exit(lockObj);
            }
            catch(SynchronizationLockException)
            {
                // This is ok, we don't own the lock.
            }
        }
    }
}
