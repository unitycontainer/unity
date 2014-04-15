// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A collection of <see cref="SectionExtensionElement"/>s.
    /// </summary>
    [ConfigurationCollection(typeof(SectionExtensionElement))]
    public class SectionExtensionElementCollection :
        DeserializableConfigurationElementCollection<SectionExtensionElement>
    {
        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var sectionElement = (SectionExtensionElement)element;
            string prefix = sectionElement.Prefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix = prefix + ".";
            }

            return prefix + sectionElement.TypeName;
        }
    }
}
