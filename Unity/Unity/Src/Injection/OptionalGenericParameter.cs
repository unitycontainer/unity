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
    /// A <see cref="InjectionParameterValue"/> that lets you specify that
    /// an instance of a generic type parameter should be resolved, providing the <see langword="null"/>
    /// value if resolving fails.
    /// </summary>
    public class OptionalGenericParameter : GenericParameterBase
    {
        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        public OptionalGenericParameter(string genericParameterName)
            : base(genericParameterName)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="resolutionKey">name to use when looking up in the container.</param>
        public OptionalGenericParameter(string genericParameterName, string resolutionKey)
            : base(genericParameterName, resolutionKey)
        { }

        /// <summary>
        /// Return a <see cref="IDependencyResolverPolicy"/> instance that will
        /// return this types value for the parameter.
        /// </summary>
        /// <param name="typeToResolve">The actual type to resolve.</param>
        /// <param name="resolutionKey">The resolution key.</param>
        /// <returns>The <see cref="IDependencyResolverPolicy"/>.</returns>
        protected override IDependencyResolverPolicy DoGetResolverPolicy(Type typeToResolve, string resolutionKey)
        {
            return new OptionalDependencyResolverPolicy(typeToResolve, resolutionKey);
        }
    }
}
