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
    /// Event argument class for the <see cref="ExtensionContext.ChildContainerCreated"/> event.
    /// </summary>
    public class ChildContainerCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a new <see cref="ChildContainerCreatedEventArgs"/> object with the
        /// given child container object.
        /// </summary>
        /// <param name="childContext">An <see cref="ExtensionContext"/> for the newly created child
        /// container.</param>
        public ChildContainerCreatedEventArgs(ExtensionContext childContext)
        {
            ChildContext = childContext;
        }

        /// <summary>
        /// The newly created child container.
        /// </summary>
        public IUnityContainer ChildContainer { get { return ChildContext.Container; } }

        /// <summary>
        /// An extension context for the created child container.
        /// </summary>
        public ExtensionContext ChildContext { get; private set; }
    }
}
