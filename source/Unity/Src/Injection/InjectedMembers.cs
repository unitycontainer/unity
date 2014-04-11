// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A Unity container extension that allows you to configure
    /// which constructors, properties, and methods get injected
    /// via an API rather than through attributes.
    /// </summary>
    [Obsolete("Use the IUnityContainer.RegisterType method instead of this interface")]
    public class InjectedMembers : UnityContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="ExtensionContext"/> by adding strategies, policies, etc. to
        /// install it's functions into the container.</remarks>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// API to configure the injection settings for a particular type.
        /// </summary>
        /// <typeparam name="TTypeToInject">Type the injection is being configured for.</typeparam>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>This extension object.</returns>
        public InjectedMembers ConfigureInjectionFor<TTypeToInject>(params InjectionMember[] injectionMembers)
        {
            return this.ConfigureInjectionFor(typeof(TTypeToInject), null, injectionMembers);
        }

        /// <summary>
        /// API to configure the injection settings for a particular type/name pair.
        /// </summary>
        /// <typeparam name="TTypeToInject">Type the injection is being configured for.</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>This extension object.</returns>
        public InjectedMembers ConfigureInjectionFor<TTypeToInject>(string name, params InjectionMember[] injectionMembers)
        {
            return this.ConfigureInjectionFor(typeof(TTypeToInject), name, injectionMembers);
        }

        /// <summary>
        /// API to configure the injection settings for a particular type.
        /// </summary>
        /// <param name="typeToInject">Type to configure.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>This extension object.</returns>
        public InjectedMembers ConfigureInjectionFor(Type typeToInject, params InjectionMember[] injectionMembers)
        {
            return this.ConfigureInjectionFor(null, typeToInject, null, injectionMembers);
        }

        /// <summary>
        /// API to configure the injection settings for a particular type/name pair.
        /// </summary>
        /// <param name="typeToInject">Type to configure.</param>
        /// <param name="name">Name of registration.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>This extension object.</returns>
        public InjectedMembers ConfigureInjectionFor(Type typeToInject, string name, params InjectionMember[] injectionMembers)
        {
            return this.ConfigureInjectionFor(null, typeToInject, name, injectionMembers);
        }

        /// <summary>
        /// API to configure the injection settings for a particular type/name pair.
        /// </summary>
        /// <param name="serviceType">Type of interface/base class being registered (may be null).</param>
        /// <param name="implementationType">Type of actual implementation class being registered.</param>
        /// <param name="name">Name of registration.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>This extension object.</returns>
        public InjectedMembers ConfigureInjectionFor(Type serviceType, Type implementationType, string name, params InjectionMember[] injectionMembers)
        {
            Container.RegisterType(serviceType, implementationType, name, injectionMembers);
            return this;
        }
    }
}
