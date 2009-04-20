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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.ObjectBuilder
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolverPolicy"/> that stores a
    /// type and name, and at resolution time puts them together into a
    /// <see cref="NamedTypeBuildKey"/>.
    /// </summary>
    public class NamedTypeDependencyResolverPolicy : IDependencyResolverPolicy
    {
        private Type type;
        private string name;

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
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            NamedTypeBuildKey key = new NamedTypeBuildKey(type, name);
            IBuilderContext recursiveContext = context.CloneForNewBuild(key, null);
            return recursiveContext.Strategies.ExecuteBuildUp(recursiveContext);
        }

        /// <summary>
        /// The type that this resolver resolves.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
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
