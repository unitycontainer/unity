// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// An EventArgs class that holds a string Name.
    /// </summary>
    public abstract class NamedEventArgs : EventArgs
    {
        private string name;

        /// <summary>
        /// Create a new <see cref="NamedEventArgs"/> with a null name.
        /// </summary>
        protected NamedEventArgs()
        {
        }

        /// <summary>
        /// Create a new <see cref="NamedEventArgs"/> with the given name.
        /// </summary>
        /// <param name="name">Name to store.</param>
        protected NamedEventArgs(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// The name.
        /// </summary>
        /// <value>Name used for this EventArg object.</value>
        public virtual string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
