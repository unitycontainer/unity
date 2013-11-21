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
using System.Reflection;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IMethodSelectorPolicy"/> that selects
    /// methods by looking for the given <typeparamref name="TMarkerAttribute"/>
    /// attribute on those methods.
    /// </summary>
    /// <typeparam name="TMarkerAttribute">Type of attribute used to mark methods
    /// to inject.</typeparam>
    public class MethodSelectorPolicy<TMarkerAttribute> : MethodSelectorPolicyBase<TMarkerAttribute> 
        where TMarkerAttribute : Attribute
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", 
            Justification = "Validation done by Guard class")]
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(parameter, "parameter");

            return new FixedTypeResolverPolicy(parameter.ParameterType);
        }
    }
}
