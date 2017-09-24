// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Utility;

namespace ObjectBuilder2
{
    /// <summary>
    /// Base class that provides an implementation of <see cref="IPropertySelectorPolicy"/>
    /// which lets you override how the parameter resolvers are created.
    /// </summary>
    public abstract class PropertySelectorBase<TResolutionAttribute> : IPropertySelectorPolicy
        where TResolutionAttribute : Attribute
    {
        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of <see cref="PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        public virtual IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type t = context.BuildKey.Type;

            foreach (PropertyInfo prop in t.GetPropertiesHierarchical().Where(p => p.CanWrite))
            {
                var propertyMethod = prop.SetMethod ?? prop.GetMethod;
                if (propertyMethod.IsStatic)
                {
                    // Skip static properties. In the previous implementation the reflection query took care of this.
                    continue;
                }

                // Ignore indexers and return properties marked with the attribute
                if (prop.GetIndexParameters().Length == 0 &&
                   prop.IsDefined(typeof(TResolutionAttribute), false))
                {
                    yield return CreateSelectedProperty(prop);
                }
            }
        }

        private SelectedProperty CreateSelectedProperty(PropertyInfo property)
        {
            IDependencyResolverPolicy resolver = this.CreateResolver(property);
            return new SelectedProperty(property, resolver);
        }

        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected abstract IDependencyResolverPolicy CreateResolver(PropertyInfo property);
    }
}
