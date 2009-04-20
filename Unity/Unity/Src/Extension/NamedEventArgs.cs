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
        /// <value>Name used for this event arg object.</value>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
