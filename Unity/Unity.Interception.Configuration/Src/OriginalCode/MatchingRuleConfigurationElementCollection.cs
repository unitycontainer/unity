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

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Collection of <see cref="MatchingRuleConfigurationElement"/> elements
    /// from the configuration file.
    /// </summary>
    // FxCop suppression: This is not a normal collection, not going to implement generic interfaces
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class MatchingRuleConfigurationElementCollection : TypeResolvingConfigurationElementCollection
    {
        /// <summary>
        /// Resolve the given element by key.
        /// </summary>
        /// <param name="key">Name of element to find.</param>
        /// <returns>Element at the given key.</returns>
        public new MatchingRuleConfigurationElement this[string key]
        {
            get { return (MatchingRuleConfigurationElement)Get(key); }
        }

        ///<summary>
        ///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        ///
        protected override ConfigurationElement CreateNewElement()
        {
            return new MatchingRuleConfigurationElement();
        }

        ///<summary>
        ///Gets the element key for a specified configuration element when overridden in a derived class.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        ///
        ///<param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            MatchingRuleConfigurationElement configElement = (MatchingRuleConfigurationElement)(element);
            return configElement.Name;
        }
    }
}
