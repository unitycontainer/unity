﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Threading;
using ObjectBuilder2;

namespace Unity
{
    /// <summary>
    /// Base class for Lifetime managers which need to synchronize calls to
    /// <see cref="SynchronizedLifetimeManager.GetValue"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The purpose of this class is to provide a basic implementation of the lifetime manager synchronization pattern.
    /// </para>
    /// <para>
    /// Calls to the <see cref="SynchronizedLifetimeManager.GetValue"/> method of a <see cref="SynchronizedLifetimeManager"/> 
    /// instance acquire a lock, and if the instance has not been initialized with a value yet the lock will only be released 
    /// when such an initialization takes place by calling the <see cref="SynchronizedLifetimeManager.SetValue"/> method or if 
    /// the build request which resulted in the call to the GetValue method fails.
    /// </para>
    /// </remarks>
    /// <see cref="LifetimeManager"/>
    public abstract class SynchronizedLifetimeManager : LifetimeManager, IRequiresRecovery
    {
        private object lockObj = new object();

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        /// <remarks>Calls to this method acquire a lock which is released only if a non-null value
        /// has been set for the lifetime manager.</remarks>
        public override object GetValue()
        {
            Monitor.Enter(this.lockObj);
            object result = this.SynchronizedGetValue();
            if (result != null)
            {
                Monitor.Exit(this.lockObj);
            }
            return result;
        }

        /// <summary>
        /// Performs the actual retrieval of a value from the backing store associated 
        /// with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.GetValue"/>
        /// after it has acquired its lock.</remarks>
        protected abstract object SynchronizedGetValue();

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <remarks>Setting a value will attempt to release the lock acquired by 
        /// <see cref="SynchronizedLifetimeManager.GetValue"/>.</remarks>
        public override void SetValue(object newValue)
        {
            this.SynchronizedSetValue(newValue);
            this.TryExit();
        }

        /// <summary>
        /// Performs the actual storage of the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.SetValue"/>
        /// before releasing its lock.</remarks>
        protected abstract void SynchronizedSetValue(object newValue);

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
        }

        /// <summary>
        /// A method that does whatever is needed to clean up
        /// as part of cleaning up after an exception.
        /// </summary>
        /// <remarks>
        /// Don't do anything that could throw in this method,
        /// it will cause later recover operations to get skipped
        /// and play real havoc with the stack trace.
        /// </remarks>
        public void Recover()
        {
            this.TryExit();
        }

        private void TryExit()
        {
            // Prevent first chance exception when abandoning a lock that has not been entered
            if (Monitor.IsEntered(this.lockObj))
            {
                try
                {
                    Monitor.Exit(this.lockObj);
                }
                catch (SynchronizationLockException)
                {
                    // Noop here - we don't hold the lock and that's ok.
                }
            }
        }
    }
}
