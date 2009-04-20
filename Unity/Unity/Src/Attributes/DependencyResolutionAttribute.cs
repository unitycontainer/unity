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
