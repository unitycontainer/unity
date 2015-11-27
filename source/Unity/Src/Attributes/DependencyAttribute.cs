// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;
using Unity.ObjectBuilder;

namespace Unity
{
    /// <summary>
    /// This attribute is used to mark properties and parameters as targets for injection.
    /// </summary>
    /// <remarks>
    /// For properties, this attribute is necessary for injection to happen. For parameters,
    /// it's not needed unless you want to specify additional information to control how
    /// the parameter is resolved.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class DependencyAttribute : DependencyResolutionAttribute
    {
        private readonly string name;

        /// <summary>
        /// Create an instance of <see cref="DependencyAttribute"/> with no name.
        /// </summary>
        public DependencyAttribute()
            : this(null) { }

        /// <summary>
        /// Create an instance of <see cref="DependencyAttribute"/> with the given name.
        /// </summary>
        /// <param name="name">Name to use when resolving this dependency.</param>
        public DependencyAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// The name specified in the constructor.
        /// </summary>
        public string Name
        {
            get { return this.name; }
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
            return new NamedTypeDependencyResolverPolicy(typeToResolve, this.name);
        }
    }
}
