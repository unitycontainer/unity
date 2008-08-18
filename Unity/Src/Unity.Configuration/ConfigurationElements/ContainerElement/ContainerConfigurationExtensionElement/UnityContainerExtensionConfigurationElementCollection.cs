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

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Collection of <see cref="UnityContainerExtensionConfigurationElement"/> elements
    /// from the configuration file. This collection can contain derived classes of
    /// <see cref="UnityContainerExtensionConfigurationElement"/>.
    /// </summary>
    // FxCop suppression: This is not a normal collection, not going to implement generic interfaces
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class UnityContainerExtensionConfigurationElementCollection : TypeResolvingConfigurationElementCollection
    {
        /// <summary>
        /// Resolve the given element by key.
        /// </summary>
        /// <param name="key">Name of element to find.</param>
        /// <returns>Element at the given key.</returns>
        public new UnityContainerExtensionConfigurationElement this[string key]
        {
            get { return (UnityContainerExtensionConfigurationElement)Get(key); }
        }

        ///<summary>
        /// Figure out the concrete type to create given the type attribute on the given
        /// xml reader. If not present, throws an exception.
        ///</summary>
        ///
        ///<returns>
        ///true if the unrecognized element was deserialized successfully; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="reader">An input stream that reads XML from the configuration file. </param>
        ///<param name="elementName">The name of the unrecognized element. </param>
        ///<exception cref="T:System.ArgumentException">elementName starts with the reserved prefix "config" or "lock".</exception>
        ///<exception cref="T:System.Configuration.ConfigurationErrorsException">The element specified in elementName is the &lt;clear&gt; element.</exception>
        protected override bool OnDeserializeUnrecognizedElement(
            string elementName, XmlReader reader)
        {
            if(elementName == base.AddElementName)
            {
                string typeName = reader.GetAttribute("type");
                if(!string.IsNullOrEmpty(typeName))
                {
                    Type elementType = Type.GetType(typeName);
                    UnityContainerExtensionConfigurationElement element =
                        (UnityContainerExtensionConfigurationElement)
                            Activator.CreateInstance(elementType);
                    element.DeserializeElement(reader);
                    BaseAdd(element);
                    return true;
                }
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
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
            return new UnityContainerExtensionConfigurationElement();
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
            UnityContainerExtensionConfigurationElement configElement = (UnityContainerExtensionConfigurationElement)( element );
            return configElement.Name;
        }
    }
}
