// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// An <see cref="DependencyResolutionAttribute"/> used to mark a dependency
    /// as optional - the container will try to resolve it, and return null
    /// if the resolution fails rather than throw.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class OptionalDependencyAttribute : DependencyResolutionAttribute
    {
        private readonly string name;

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object.
        /// </summary>
        public OptionalDependencyAttribute()
            : this(null)
        {

        }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object that
        /// specifies a named dependency.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        public OptionalDependencyAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Name of the dependency.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Create an instance of <see cref="IDependencyResolverPolicy"/> that
        /// will be used to get the value for the member this attribute is
        /// applied to.
        /// </summary>
        /// <param name="typeToResolve">Type of parameter or property that
        /// this attribute is decoration.</param>
        /// <returns>The resolver object.</returns>
        public override IDependencyResolverPolicy CreateResolver(Type typeToResolve)
        {
            return new OptionalDependencyResolverPolicy(typeToResolve, name);
        }
    }
}
