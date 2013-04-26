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
using Microsoft.Practices.Unity.Mvc;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds onto the instance given to it during
    /// the lifetime of a single HTTP request.
    /// This lifetime manager enables you to create instances of registered types that behave like
    /// singletons within the scope of an HTTP request.
    /// See remarks for important usage information.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Although the <see cref="PerRequestLifetimeManager"/> lifetime manager works correctly and can help
    /// in working with stateful or thread-unsafe dependencies within the scope of an HTTP request, it is
    /// generally not a good idea to use it when it can be avoided, as it can often lead to bad practices or
    /// hard to find bugs in the end-user's application code when used incorrectly. 
    /// It is recommended that the dependencies you register are stateless and if there is a need to share
    /// common state between several objects during the lifetime of an HTTP request, then you can
    /// have a stateless service that explicitly stores and retrieves this state using the
    /// <see cref="System.Web.HttpContext.Items"/> collection of the <see cref="System.Web.HttpContext.Current"/> object.
    /// </para>
    /// <para>
    /// For the instance of the registered type to be disposed automatically when the HTTP request completes,
    /// make sure to register the <see cref="UnityPerRequestHttpModule"/> with the web application.
    /// To do this, invoke the following in the Unity bootstrapping class (typically UnityMvcActivator.cs):
    /// <code>DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));</code>
    /// </para>
    /// </remarks>
    public class PerRequestLifetimeManager : LifetimeManager
    {
        private readonly object lifetimeKey = new object();

        /// <summary>
        /// Retrieves a value from the backing store associated with this lifetime policy.
        /// </summary>
        /// <returns>The desired object, or null if no such object is currently stored.</returns>
        public override object GetValue()
        {
            return UnityPerRequestHttpModule.GetValue(this.lifetimeKey);
        }

        /// <summary>
        /// Stores the given value into the backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            UnityPerRequestHttpModule.SetValue(this.lifetimeKey, newValue);
        }

        /// <summary>
        /// Removes the given object from the backing store.
        /// </summary>
        public override void RemoveValue()
        {
            var disposable = this.GetValue() as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }

            UnityPerRequestHttpModule.SetValue(this.lifetimeKey, null);
        }
    }
}
