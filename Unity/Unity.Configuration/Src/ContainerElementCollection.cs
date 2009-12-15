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
        /// <summary>
        /// Retrieve a stored <see cref="ContainerElement"/> by name.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>The stored container or null if not in the collection.</returns>
        public new ContainerElement this[string name]
        {
            get { return (ContainerElement) BaseGet(name); }
        }

        /// <summary>
        /// Return the default container in the collection. The default is the one without a name.
        /// </summary>
        public ContainerElement Default
        {
            get { return (ContainerElement)BaseGet(string.Empty); }
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
