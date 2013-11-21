// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IPropertySelectorPolicy"/> that looks
    /// for properties marked with the <typeparamref name="TResolutionAttribute"/>
    /// attribute that are also settable and not indexers.
    /// </summary>
    /// <typeparam name="TResolutionAttribute"></typeparam>
    public class PropertySelectorPolicy<TResolutionAttribute> : PropertySelectorBase<TResolutionAttribute>
        where TResolutionAttribute : Attribute
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", 
            Justification = "Validation done by Guard class")]
        protected override IDependencyResolverPolicy CreateResolver(PropertyInfo property)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(property, "property");
            
            return new FixedTypeResolverPolicy(property.PropertyType);
        }
    }
}
