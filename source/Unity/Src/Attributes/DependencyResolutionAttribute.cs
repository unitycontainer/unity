// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Base class for attributes that can be placed on parameters
    /// or properties to specify how to resolve the value for
    /// that parameter or property.
    /// </summary>
    public abstract class DependencyResolutionAttribute : Attribute
    {
        /// <summary>
        /// Create an instance of <see cref="IDependencyResolverPolicy"/> that
        /// will be used to get the value for the member this attribute is
        /// applied to.
        /// </summary>
        /// <param name="typeToResolve">Type of parameter or property that
        /// this attribute is decoration.</param>
        /// <returns>The resolver object.</returns>
        public abstract IDependencyResolverPolicy CreateResolver(Type typeToResolve);
    }
}
