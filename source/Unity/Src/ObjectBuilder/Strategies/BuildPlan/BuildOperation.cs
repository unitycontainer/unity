// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
