// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity
{
    /// <summary>
    /// Event argument class for the <see cref="ExtensionContext.RegisteringInstance"/> event.
    /// </summary>
    public class RegisterInstanceEventArgs : NamedEventArgs
    {
        private Type registeredType;
        private object instance;
        private LifetimeManager lifetimeManager;

        /// <summary>
        /// Create a default <see cref="RegisterInstanceEventArgs"/> instance.
        /// </summary>
        public RegisterInstanceEventArgs()
        {
        }

        /// <summary>
        /// Create a <see cref="RegisterInstanceEventArgs"/> instance initialized with the given arguments.
        /// </summary>
        /// <param name="registeredType">Type of instance being registered.</param>
        /// <param name="instance">The instance object itself.</param>
        /// <param name="name">Name to register under, null if default registration.</param>
        /// <param name="lifetimeManager"><see cref="LifetimeManager"/> object that handles how
        /// the instance will be owned.</param>
        public RegisterInstanceEventArgs(Type registeredType, object instance, string name, LifetimeManager lifetimeManager)
            : base(name)
        {
            this.registeredType = registeredType;
            this.instance = instance;
            this.lifetimeManager = lifetimeManager;
        }

        /// <summary>
        /// Type of instance being registered.
        /// </summary>
        /// <value>
        /// Type of instance being registered.
        /// </value>
        public Type RegisteredType
        {
            get { return this.registeredType; }
            set { this.registeredType = value; }
        }

        /// <summary>
        /// Instance object being registered.
        /// </summary>
        /// <value>Instance object being registered</value>
        public object Instance
        {
            get { return this.instance; }
            set { this.instance = value; }
        }

        /// <summary>
        /// <see cref="Unity.LifetimeManager"/> that controls ownership of
        /// this instance.
        /// </summary>
        public LifetimeManager LifetimeManager
        {
            get { return this.lifetimeManager; }
            set { this.lifetimeManager = value; }
        }
    }
}
