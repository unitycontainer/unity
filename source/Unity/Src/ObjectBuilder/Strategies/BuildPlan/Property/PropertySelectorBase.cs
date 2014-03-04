// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
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

                if (prop.GetIndexParameters().Length == 0 &&                // Ignore indexers
                   prop.IsDefined(typeof(TResolutionAttribute), false))     // Marked with the attribute
                {
                    yield return CreateSelectedProperty(context, resolverPolicyDestination, prop);
                }
            }
        }

        private SelectedProperty CreateSelectedProperty(IBuilderContext context, IPolicyList resolverPolicyDestination, PropertyInfo property)
        {
            string key = Guid.NewGuid().ToString();
            var resolver = CreateResolver(property);
            var result = new SelectedProperty(property, resolver);
            resolverPolicyDestination.Set(resolver, key);
            DependencyResolverTrackerPolicy.TrackKey(context.PersistentPolicies,
                context.BuildKey,
                key);
            return result;
        }

        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
        protected abstract IDependencyResolverPolicy CreateResolver(PropertyInfo property);
    }
}
