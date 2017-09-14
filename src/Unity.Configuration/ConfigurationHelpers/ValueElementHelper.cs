// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Xml;
using Unity.Configuration.Properties;

namespace Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A helper class that assists in deserializing parameter and property
    /// elements. These elements both have a single "value" child element that
    /// specify the value to inject for the property or parameter.
    /// </summary>
    public class ValueElementHelper
    {
        private readonly IValueProvidingElement parentElement;
        private static readonly DependencyElement DefaultDependency = new DependencyElement();
        private readonly UnknownElementHandlerMap unknownElementHandlerMap;
        private readonly Dictionary<string, string> attributeMap = new Dictionary<string, string>();

        private static readonly Dictionary<Type, string> KnownValueElementTags =
            new Dictionary<Type, string>
            {
                { typeof(DependencyElement), "dependency" },
                { typeof(ValueElement), "value" },
                { typeof(ArrayElement), "array" },
                { typeof(OptionalElement), "optional" }
            };

        /// <summary>
        /// Create a new <see cref="ValueElementHelper"/> that wraps reading
        /// values and storing them in the given <paramref name="parentElement"/>.
        /// </summary>
        /// <param name="parentElement">Element that contains the value elements.</param>
        public ValueElementHelper(IValueProvidingElement parentElement)
        {
            this.parentElement = parentElement;

            unknownElementHandlerMap = new UnknownElementHandlerMap
            {
                { "value",      xr => SetValue<ValueElement>(xr) },
                { "dependency", xr => SetValue<DependencyElement>(xr) },
                { "array",      xr => SetValue<ArrayElement>(xr) },
                { "optional",   xr => SetValue<OptionalElement>(xr) }
            };
        }

        /// <summary>
        /// Gets a <see cref="ParameterValueElement"/>, or if none is present,
        /// returns a default <see cref="DependencyElement"/>.
        /// </summary>
        /// <param name="currentValue">The <see cref="ParameterValueElement"/>.</param>
        /// <returns>The given <paramref name="currentValue"/>, unless
        /// <paramref name="currentValue"/> is null, in which case returns
        /// a <see cref="DependencyElement"/>.</returns>
        public static ParameterValueElement GetValue(ParameterValueElement currentValue)
        {
            return currentValue ?? DefaultDependency;
        }

        /// <summary>
        /// Helper method used during deserialization to handle
        /// attributes for the dependency and value tags.
        /// </summary>
        /// <param name="name">attribute name.</param>
        /// <param name="value">attribute value.</param>
        /// <returns>true</returns>
        public bool DeserializeUnrecognizedAttribute(string name, string value)
        {
            attributeMap[name] = value;
            return true;
        }

        /// <summary>
        /// Helper method used during deserialization to handle the default
        /// value element tags.
        /// </summary>
        /// <param name="elementName">The element name.</param>
        /// <param name="reader">XML data to read.</param>
        /// <returns>True if deserialization succeeded, false if it failed.</returns>
        public bool DeserializeUnknownElement(string elementName, XmlReader reader)
        {
            return unknownElementHandlerMap.ProcessElement(elementName, reader) ||
                DeserializeExtensionValueElement(elementName, reader);
        }

        /// <summary>
        /// Call this method at the end of deserialization of your element to
        /// set your value element.
        /// </summary>
        public void CompleteValueElement(XmlReader reader)
        {
            if (ShouldConstructValueElementFromAttributes(reader))
            {
                ConstructValueElementFromAttributes();
            }
        }

        /// <summary>
        /// Serialize a <see cref="ParameterValueElement"/> object out to XML.
        /// This method is aware of and implements the shorthand attributes
        /// for dependency and value elements.
        /// </summary>
        /// <param name="writer">Writer to output XML to.</param>
        /// <param name="element">The <see cref="ParameterValueElement"/> to serialize.</param>
        /// <param name="elementsOnly">If true, always output an element. If false, then
        /// dependency and value elements will be serialized as attributes in the parent tag.</param>
        public static void SerializeParameterValueElement(XmlWriter writer, ParameterValueElement element, bool elementsOnly)
        {
            string tag = GetTagForElement(element);
            if (!elementsOnly)
            {
                var attributeOnlyElement = element as IAttributeOnlyElement;
                if (attributeOnlyElement != null)
                {
                    attributeOnlyElement.SerializeContent(writer);
                }
                else
                {
                    writer.WriteElement(tag, element.SerializeContent);
                }
            }
            else
            {
                writer.WriteElement(tag, element.SerializeContent);
            }
        }

        private static string GetTagForElement(ConfigurationElement element)
        {
            Type elementType = element.GetType();
            return KnownValueElementTags.GetOrNull(elementType) ??
                ExtensionElementMap.GetTagForExtensionElement(element);
        }
        private void SetValue<TElement>(XmlReader reader)
            where TElement : ParameterValueElement, new()
        {
            if (parentElement.Value != null)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.DuplicateParameterValueElement,
                        parentElement.DestinationName),
                    reader);
            }

            var element = new TElement();
            element.Deserialize(reader);
            parentElement.Value = element;
        }

        private bool ShouldConstructValueElementFromAttributes(XmlReader reader)
        {
            if (parentElement.Value != null)
            {
                if (attributeMap.Count > 0)
                {
                    throw new ConfigurationErrorsException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ElementWithAttributesAndParameterValueElements,
                            parentElement.DestinationName),
                        reader);
                }

                return false;
            }

            return attributeMap.Count > 0;
        }

        private void ConstructValueElementFromAttributes()
        {
            if (attributeMap.ContainsKey("value"))
            {
                parentElement.Value = new ValueElement(attributeMap);
            }
            else if (attributeMap.ContainsKey("dependencyName") || attributeMap.ContainsKey("dependencyType"))
            {
                parentElement.Value = new DependencyElement(attributeMap);
            }
            else
            {
                throw new ConfigurationErrorsException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidValueAttributes, parentElement.DestinationName));
            }
        }

        private bool DeserializeExtensionValueElement(string elementName, XmlReader reader)
        {
            Type elementType = ExtensionElementMap.GetParameterValueElementType(elementName);
            if (elementType != null)
            {
                var element = (ParameterValueElement)Activator.CreateInstance(elementType);
                element.Deserialize(reader);
                parentElement.Value = element;
            }
            return elementType != null;
        }
    }
}
