using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A collection of <see cref="InstanceElement"/>s.
    /// </summary>
    public class InstanceElementCollection : DeserializableConfigurationElementCollection<InstanceElement>
    {
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
            return ((InstanceElement) element).Key;
        }
    }
}
