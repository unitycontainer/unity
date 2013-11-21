// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A collection of <see cref="MatchingRuleElement"/>s for configuration.
    /// </summary>
    [ConfigurationCollection(typeof(MatchingRuleElement))]
    public class MatchingRuleElementCollection : DeserializableConfigurationElementCollection<MatchingRuleElement>
    {
        private static readonly UnknownElementHandlerMap<MatchingRuleElementCollection> unknownElementHandlerMap
            = new UnknownElementHandlerMap<MatchingRuleElementCollection>
                {
                    { "matchingRule", (mrec, xr) => mrec.ReadUnwrappedElement(xr, mrec) }
                };

        /// <summary>
        /// Retrieve a matching rule element from the collection by name.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>The rule, or null if not in the collection.</returns>
        public new MatchingRuleElement this[string name]
        {
            get { return (MatchingRuleElement) BaseGet(name); }
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
            return ((MatchingRuleElement) element).Name;
        }

        /// <summary>
        /// Causes the configuration system to throw an exception.
        /// </summary>
        /// <returns>
        /// true if the unrecognized element was deserialized successfully; otherwise, false. The default is false.
        /// </returns>
        /// <param name="elementName">The name of the unrecognized element. 
        ///                 </param><param name="reader">An input stream that reads XML from the configuration file. 
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element specified in <paramref name="elementName"/> is the &lt;clear&gt; element.
        ///                 </exception><exception cref="T:System.ArgumentException"><paramref name="elementName"/> starts with the reserved prefix "config" or "lock".
        ///                 </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            return unknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}
