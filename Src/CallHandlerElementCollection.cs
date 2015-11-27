// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using System.Xml;
using Unity.Configuration.ConfigurationHelpers;

namespace Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A collection of <see cref="CallHandlerElement"/>s for configuration.
    /// </summary>
    [ConfigurationCollection(typeof(CallHandlerElement))]
    public class CallHandlerElementCollection : DeserializableConfigurationElementCollection<CallHandlerElement>
    {
        private static readonly UnknownElementHandlerMap<CallHandlerElementCollection> UnknownElementHandlerMap
            = new UnknownElementHandlerMap<CallHandlerElementCollection>
                {
                    { "callHandler", (chec, xr) => chec.ReadUnwrappedElement(xr, chec) }
                };

        /// <summary>
        /// Retrieve a call handler element from the collection by name.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>The rule, or null if not in the collection.</returns>
        public new CallHandlerElement this[string name]
        {
            get { return (CallHandlerElement)BaseGet(name); }
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CallHandlerElement)element).Name;
        }

        /// <summary>
        /// Causes the configuration system to throw an exception.
        /// </summary>
        /// <returns>
        /// true if the unrecognized element was deserialized successfully; otherwise, false. The default is false.
        /// </returns>
        /// <param name="elementName">The name of the unrecognized element. </param>
        /// <param name="reader">An input stream that reads XML from the configuration file. </param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element specified in <paramref name="elementName"/> is the &lt;clear&gt; element.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="elementName"/> starts with the reserved prefix "config" or "lock".</exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return UnknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}
