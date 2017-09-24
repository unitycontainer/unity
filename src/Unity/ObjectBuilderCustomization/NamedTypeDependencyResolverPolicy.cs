// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;
using Unity.Utility;

namespace Unity.ObjectBuilder
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolverPolicy"/> that stores a
    /// type and name, and at resolution time puts them together into a
    /// <see cref="NamedTypeBuildKey"/>.
    /// </summary>
    public class NamedTypeDependencyResolverPolicy : IDependencyResolverPolicy
    {
        private Type type;
        private readonly string name;

        /// <summary>
        /// Create an instance of <see cref="NamedTypeDependencyResolverPolicy"/>
        /// with the given type and name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name (may be null).</param>
        public NamedTypeDependencyResolverPolicy(Type type, string name)
        {
            this.type = type;
            this.name = name;
        }

        /// <summary>
        /// Resolve the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        // FxCop suppression: Validation is done by Guard class
        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return context.NewBuildUp(new NamedTypeBuildKey(type, name));
        }

        /// <summary>
        /// The type that this resolver resolves.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// The name that this resolver resolves.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
    }
}
