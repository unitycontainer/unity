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
    /// Configuration collection of the information used to configure member injection.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class InjectionMemberElementCollection : TypeResolvingConfigurationElementCollection
    {
        private const string ConstructorElementName = "constructor";
        private const string PropertyElementName = "property";
        private const string MethodElementName = "method";

        ///<summary>
        ///Causes the configuration system to throw an exception.
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
            switch (elementName)
            {
            case ConstructorElementName:
                return DeserializeConstructorElement(reader);

            case PropertyElementName:
                return DeserializePropertyElement(reader);

            case MethodElementName:
                return DeserializeMethodElement(reader);

            default:
                return DeserializeCustomElement(elementName, reader);
            }
        }

        private bool DeserializeConstructorElement(XmlReader reader)
        {
            return DeserializeAndAdd(new InjectionConstructorElement(), reader);
        }

        private bool DeserializePropertyElement(XmlReader reader)
        {
            return DeserializeAndAdd(new InjectionPropertyElement(), reader);
        }

        private bool DeserializeMethodElement(XmlReader reader)
        {
            return DeserializeAndAdd(new InjectionMethodElement(), reader);
        }

        private bool DeserializeCustomElement(string elementName, XmlReader reader)
        {
            string elementTypeName = reader.GetAttribute("elementType");
            if(!string.IsNullOrEmpty(elementTypeName))
            {
                Type elementType = Type.GetType(elementTypeName);
                return
                    DeserializeAndAdd(
                        (InjectionMemberElement)Activator.CreateInstance(elementType), reader);
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        private bool DeserializeAndAdd(InjectionMemberElement element, XmlReader reader)
        {
            element.DeserializeElement(reader);
            BaseAdd(element);
            return true;
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
            return new InjectionMemberElement();
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
            InjectionMemberElement e = (InjectionMemberElement)element;
            return e.ElementName + ":" + e.Name;
        }
    }
}
