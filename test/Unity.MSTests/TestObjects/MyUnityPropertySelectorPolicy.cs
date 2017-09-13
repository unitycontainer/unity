// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ObjectBuilder2;

using System.Collections.Generic;

namespace Unity.Tests.TestObjects
{
    internal class MyUnityPropertySelectorPolicy : PropertySelectorBase<DependencyResolutionAttribute>
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IDependencyResolverPolicy CreateResolver(PropertyInfo property)
        {
            List<DependencyResolutionAttribute> attributes = new List<DependencyResolutionAttribute>(
                from item in property.GetCustomAttributes(typeof(DependencyResolutionAttribute), false)
                where item is DependencyResolutionAttribute
                select (DependencyResolutionAttribute)item);

            // We must have one of these, otherwise this method would never have been called.
            Debug.Assert(attributes.Count == 1);

            return attributes[0].CreateResolver(property.PropertyType);
        }

        public override IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList policyList)
        {
            return base.SelectProperties(context, policyList);
        }
    }
}
