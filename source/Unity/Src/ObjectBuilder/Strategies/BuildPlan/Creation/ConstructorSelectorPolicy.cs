// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IConstructorSelectorPolicy"/> that chooses
    /// constructors based on these criteria: first, pick a constructor marked with the
    /// <typeparamref name="TInjectionConstructorMarkerAttribute"/> attribute. If there
    /// isn't one, then choose the constructor with the longest parameter list. If that is ambiguous,
    /// then throw.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the constructor to choose is ambiguous.</exception>
    /// <typeparam name="TInjectionConstructorMarkerAttribute">Attribute used to mark the constructor to call.</typeparam>
    public class ConstructorSelectorPolicy<TInjectionConstructorMarkerAttribute> : ConstructorSelectorPolicyBase<TInjectionConstructorMarkerAttribute> 
        where TInjectionConstructorMarkerAttribute : Attribute
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification="Validation done by Guard class")] 
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            Guard.ArgumentNotNull(parameter, "parameter");
            return new FixedTypeResolverPolicy(parameter.ParameterType);
        }
    }
}
