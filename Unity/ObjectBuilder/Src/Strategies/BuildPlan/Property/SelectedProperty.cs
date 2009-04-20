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

using System.Reflection;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Objects of this type are returned from
    /// <see cref="IPropertySelectorPolicy.SelectProperties"/>.
    /// This class combines the <see cref="PropertyInfo"/> about
    /// the property with the string key used to look up the resolver
    /// for this property's value.
    /// </summary>
    public class SelectedProperty
    {
        private PropertyInfo property;
        private string key;

        /// <summary>
        /// Create an instance of <see cref="SelectedProperty"/>
        /// with the given <see cref="PropertyInfo"/> and key.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="key">Key to use to look up the resolver.</param>
        public SelectedProperty(PropertyInfo property, string key)
        {
            this.property = property;
            this.key = key;
        }

        /// <summary>
        /// PropertyInfo for this property.
        /// </summary>
        public PropertyInfo Property
        {
            get { return property; }
        }

        /// <summary>
        /// Key to look up this property's resolver.
        /// </summary>
        public string Key
        {
            get { return key; }
        }
    }
}
