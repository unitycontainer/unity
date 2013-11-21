// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A collection of <see cref="AliasElement"/>s.
    /// </summary>
    [ConfigurationCollection(typeof(AliasElement))]
    public class AliasElementCollection : DeserializableConfigurationElementCollection<AliasElement>
    {
        private static readonly UnknownElementHandlerMap<AliasElementCollection> unknownElementHandlerMap
            = new UnknownElementHandlerMap<AliasElementCollection>
                {
                    { "typeAlias", (aec, xr) => aec.ReadUnwrappedElement(xr, aec)}
                };

        /// <summary>
        /// Indexer that allows you to get or set an alias by the alias name.
        /// </summary>
        /// <param name="alias">Alias of element to get or set.</param>
        /// <returns>The type name at that alias.</returns>
        public new string this[string alias]
        {
            get
            {
                var element = (AliasElement) BaseGet(alias);
                if (element != null)
                {
                    return element.TypeName;
                }
                return null;
            }
            set
            {
                if (BaseGet(alias) != null)
                {
                    BaseRemove(alias);
                }
                BaseAdd(new AliasElement {Alias = alias, TypeName = value}, true);
            }
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
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return unknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
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
            return ((AliasElement) element).Alias;
        }
    }
}
