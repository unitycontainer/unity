// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A collection of <see cref="PolicyElement"/> in the configuration.
    /// </summary>
    [ConfigurationCollection(typeof(PolicyElement))]
    public class PolicyElementCollection : DeserializableConfigurationElementCollection<PolicyElement>
    {
        /// <summary>
        /// Indexer to retrieve policy element objects by name.
        /// </summary>
        /// <param name="policyName">Name of policy to get.</param>
        /// <returns>The element.</returns>
        public new PolicyElement this[string policyName]
        {
            get { return (PolicyElement) BaseGet(policyName); }
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
            return ((PolicyElement) element).Name;
        }
    }
}
