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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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
            foreach(PropertyInfo prop in t.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance))
            {
                if(prop.GetIndexParameters().Length == 0 &&               // Ignore indexers
                   prop.CanWrite &&                                      // Is writable
                   prop.IsDefined(typeof(TResolutionAttribute), false))  // Marked with the attribute
                {
                    yield return CreateSelectedProperty(context, resolverPolicyDestination, prop);
                }
            }
        }

        private SelectedProperty CreateSelectedProperty(IBuilderContext context, IPolicyList resolverPolicyDestination, PropertyInfo property)
        {
            string key = Guid.NewGuid().ToString();
            SelectedProperty result = new SelectedProperty(property, key);
            resolverPolicyDestination.Set<IDependencyResolverPolicy>(CreateResolver(property), key);
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
