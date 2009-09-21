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

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Base class for the current operation stored in the build context.
    /// </summary>
    public abstract class BuildOperation
    {
        /// <summary>
        /// Create a new <see cref="BuildOperation"/>.
        /// </summary>
        /// <param name="typeBeingConstructed">Type currently being built.</param>
        protected BuildOperation(Type typeBeingConstructed)
        {
            TypeBeingConstructed = typeBeingConstructed;
        }

        ///<summary>
        /// The type that's currently being built.
        ///</summary>
        public Type TypeBeingConstructed { get; private set; }
    }
}
