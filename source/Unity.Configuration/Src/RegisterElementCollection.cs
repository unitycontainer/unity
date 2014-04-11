// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A collection of <see cref="RegisterElement"/>s.
    /// </summary>
    [ConfigurationCollection(typeof(RegisterElement), AddItemName = "register")]
    public class RegisterElementCollection : DeserializableConfigurationElementCollection<RegisterElement>
    {
        private static readonly UnknownElementHandlerMap<RegisterElementCollection> UnknownElementHandlerMap
            = new UnknownElementHandlerMap<RegisterElementCollection>
                {
                    { "type", (rec, xr) => rec.ReadUnwrappedElement(xr, rec) }
                };

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

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var registerElement = (RegisterElement)element;
            return registerElement.TypeName + ":" + registerElement.Name;
        }
    }
}
