// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Collection element for <see cref="ContainerElement"/>s.
    /// </summary>
    [ConfigurationCollection(typeof(ContainerElement), AddItemName="container")]
    public class ContainerElementCollection : DeserializableConfigurationElementCollection<ContainerElement>
    {
        internal UnityConfigurationSection ContainingSection { get; set; }

        /// <summary>
        /// Retrieve a stored <see cref="ContainerElement"/> by name.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>The stored container or null if not in the collection.</returns>
        public new ContainerElement this[string name]
        {
            get { return GetElement(name); }
        }

        /// <summary>
        /// Return the default container in the collection. The default is the one without a name.
        /// </summary>
        public ContainerElement Default
        {
            get { return GetElement(string.Empty); }
        }

        /// <summary>
        /// Plug point to get objects out of the collection.
        /// </summary>
        /// <param name="index">Index in the collection to retrieve the item from.</param>
        /// <returns>Item at that index or null if not present.</returns>
        protected override ContainerElement GetElement(int index)
        {
            return PrepareElement(base.GetElement(index));
        }

        /// <summary>
        /// Plug point to get objects out of the collection.
        /// </summary>
        /// <param name="key">Key to look up the object by.</param>
        /// <returns>Item with that key or null if not present.</returns>
        protected override ContainerElement GetElement(object key)
        {
            return PrepareElement(base.GetElement(key));
        }

        private ContainerElement PrepareElement(ContainerElement element)
        {
            if(element != null)
            {
                element.ContainingSection = ContainingSection;
            }
            return element;
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. 
        ///                 </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ContainerElement)element).Name;
        }
    }
}
