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
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A helper class that assists in deserializing parameter and property
    /// elements. These elements both have a single "value" child element that
    /// specify the value to inject for the property or parameter.
    /// </summary>
    class ValueElementHelper
    {
        private readonly IValueProvidingElement parentElement;
        private static readonly DependencyElement defaultDependency = new DependencyElement();
        private readonly UnknownElementHandlerMap unknownElementHandlerMap;
        private readonly Dictionary<string, string> attributeMap = new Dictionary<string, string>();

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

        internal static ParameterValueElement GetValue(ParameterValueElement currentValue)
        {
            return currentValue ?? defaultDependency;
        }

        internal bool DeserializeUnrecognizedAttribute(string name, string value)
        {
            attributeMap[name] = value;
            return true;
        }

        internal bool DeserializeUnknownElement(string elementName, XmlReader reader)
        {
            return unknownElementHandlerMap.ProcessElement(elementName, reader) ||
                DeserializeExtensionValueElement(elementName, reader);
        }

        internal void CompleteValueElement()
        {
            if (ShouldConstructValueElementFromAttributes())
            {
                ConstructValueElementFromAttributes();
            }
        }

        private void SetValue<TElement>(XmlReader reader)
            where TElement : ParameterValueElement, new()
        {
            var element = new TElement();
            element.Deserialize(reader);
            parentElement.Value = element;
        }

        private bool ShouldConstructValueElementFromAttributes()
        {
            return ReferenceEquals(parentElement.Value, defaultDependency) &&
                attributeMap.Count > 0;
        }

        private void ConstructValueElementFromAttributes()
        {
            if (attributeMap.ContainsKey("value"))
            {
                parentElement.Value = new ValueElement(attributeMap);
            }
            else if (attributeMap.ContainsKey("dependencyName"))
            {
                parentElement.Value = new DependencyElement(attributeMap);
            }
            else
            {
                throw new ConfigurationErrorsException(
                    string.Format(CultureInfo.CurrentUICulture,
                        Resources.InvalidValueAttributes, parentElement.DestinationName));
            }
        }

        private bool DeserializeExtensionValueElement(string elementName, XmlReader reader)
        {
            Type elementType = ExtensionElementMap.GetParameterValueElementType(elementName);
            if(elementType != null)
            {
                var element = (ParameterValueElement) Activator.CreateInstance(elementType);
                element.Deserialize(reader);
                parentElement.Value = element;
            }
            return elementType != null;
        }
    }
}
