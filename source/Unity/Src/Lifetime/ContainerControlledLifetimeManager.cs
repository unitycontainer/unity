// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
    public class ContainerControlledLifetimeManager : SynchronizedLifetimeManager, IDisposable
    {
        private object value;

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        protected override object SynchronizedGetValue()
        {
            return this.value;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        protected override void SynchronizedSetValue(object newValue)
        {
            this.value = newValue;
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            this.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this); // shut FxCop up
        }

        /// <summary>
        /// Standard Dispose pattern implementation. Not needed, but it keeps FxCop happy.
        /// </summary>
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>
        //[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing",
                         //Justification = "This method is only here to avoid the other IDisposable warning")]
        protected virtual void Dispose(bool disposing)
        {
            if (this.value != null)
            {
                if (this.value is IDisposable)
                {
                    ((IDisposable)this.value).Dispose();
                }
                this.value = null;
            }
        }
    }
}
