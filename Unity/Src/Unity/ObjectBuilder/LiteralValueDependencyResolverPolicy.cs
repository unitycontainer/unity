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

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.ObjectBuilder
{
    /// <summary>
    /// A <see cref="IDependencyResolverPolicy"/> implementation that returns
    /// the value set in the constructor.
    /// </summary>
    public class LiteralValueDependencyResolverPolicy : IDependencyResolverPolicy
    {
        private object dependencyValue;

        /// <summary>
        /// Create a new instance of <see cref="LiteralValueDependencyResolverPolicy"/>
        /// which will return the given value when resolved.
        /// </summary>
        /// <param name="dependencyValue">The value to return.</param>
        public LiteralValueDependencyResolverPolicy(object dependencyValue)
        {
            this.dependencyValue = dependencyValue;
        }

        /// <summary>
        /// Get the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve(IBuilderContext context)
        {
            return dependencyValue;
        }
    }
}
