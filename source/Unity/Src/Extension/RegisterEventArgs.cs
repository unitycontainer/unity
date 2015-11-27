// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity
{
    /// <summary>
    /// Event argument class for the <see cref="ExtensionContext.Registering"/> event.
    /// </summary>
    public class RegisterEventArgs : NamedEventArgs
    {
        private Type typeFrom;
        private Type typeTo;
        private LifetimeManager lifetimeManager;

        /// <summary>
        /// Create a new instance of <see cref="RegisterEventArgs"/>.
        /// </summary>
        /// <param name="typeFrom">Type to map from.</param>
        /// <param name="typeTo">Type to map to.</param>
        /// <param name="name">Name for the registration.</param>
        /// <param name="lifetimeManager"><see cref="LifetimeManager"/> to manage instances.</param>
        public RegisterEventArgs(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager)
            : base(name)
        {
            this.typeFrom = typeFrom;
            this.typeTo = typeTo;
            this.lifetimeManager = lifetimeManager;
        }

        /// <summary>
        /// Type to map from.
        /// </summary>
        public Type TypeFrom
        {
            get { return this.typeFrom; }
            set { this.typeFrom = value; }
        }

        /// <summary>
        /// Type to map to.
        /// </summary>
        public Type TypeTo
        {
            get { return this.typeTo; }
            set { this.typeTo = value; }
        }

        /// <summary>
        /// <see cref="LifetimeManager"/> to manage instances.
        /// </summary>
        public LifetimeManager LifetimeManager
        {
            get { return this.lifetimeManager; }
            set { this.lifetimeManager = value; }
        }
    }
}
