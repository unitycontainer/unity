// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using ObjectBuilder2;

namespace Unity.ObjectBuilder
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
