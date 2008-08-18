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
using System.Globalization;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    internal static class InjectionParameterValueHelper
    {
        public static InjectionParameterValue CreateParameterValue(
            string typeName,
            string genericParameterName,
            InjectionParameterValueElement valueElement,
            UnityTypeResolver typeResolver)
        {
            if (!string.IsNullOrEmpty(genericParameterName))
            {
                DependencyValueElement dependencyElement = valueElement as DependencyValueElement;
                if (dependencyElement != null && !string.IsNullOrEmpty(dependencyElement.Name))
                {
                    return new GenericParameter(genericParameterName, dependencyElement.Name);
                }
                else
                {
                    return new GenericParameter(genericParameterName);
                }
            }
            else
            {
                if (valueElement == null)
                {
                    valueElement = new DependencyValueElement();
                }
                valueElement.TypeResolver = typeResolver;
                return valueElement.CreateParameterValue(typeResolver.ResolveType(typeName));
            }
        }

        public static bool DeserializeUnrecognizedElement(
            string elementName,
            XmlReader reader,
            string name,
            string genericParameterName,
            ref InjectionParameterValueElement valueElement)
        {
            GuardOnlyOneValue(valueElement, name);

            switch (elementName)
            {
                case "value":
                    GuardNotGeneric(reader, genericParameterName, name);
                    return DeserializeValueElement(
                        reader, 
                        ref valueElement);

                case "dependency":
                    return DeserializeDependencyValueElement(
                        reader, 
                        genericParameterName, 
                        name, 
                        ref valueElement);

                default:
                    GuardNotGeneric(reader, genericParameterName, name);
                    return DeserializePolymorphicElement(
                        reader, 
                        ref valueElement);
            }
        }


        private static bool DeserializeValueElement(XmlReader reader, ref InjectionParameterValueElement valueElement)
        {
            InstanceValueElement element = new InstanceValueElement();
            element.DeserializeElement(reader);
            valueElement = element;
            return true;
        }

        private static bool DeserializeDependencyValueElement(
            XmlReader reader, 
            string genericParameterName, 
            string name, 
            ref InjectionParameterValueElement valueElement)
        {
            DependencyValueElement element = new DependencyValueElement();
            element.DeserializeElement(reader);

            if (!string.IsNullOrEmpty(genericParameterName) 
                && !string.IsNullOrEmpty(element.TypeName))
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.MustNotProvideATypeForDependencyIfBelongsToGeneric,
                        name,
                        element.Name),
                    reader);
            }

            valueElement = element;
            return true;
        }

        private static bool DeserializePolymorphicElement(XmlReader reader, ref InjectionParameterValueElement valueElement)
        {
            string elementTypeName = reader.GetAttribute("elementType");
            if (!string.IsNullOrEmpty(elementTypeName))
            {
                Type elementType = Type.GetType(elementTypeName);
                InjectionParameterValueElement element =
                    (InjectionParameterValueElement)Activator.CreateInstance(elementType);
                element.DeserializeElement(reader);
                valueElement = element;
                return true;
            }

            return false;
        }

        public static void GuardOnlyOneValue(InjectionParameterValueElement valueElement, string name)
        {
            if (valueElement != null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.OnlyOneValueElementAllowed,
                        name));
            }
        }

        public static void GuardNotGeneric(XmlReader reader, string genericParameterName, string name)
        {
            if (!string.IsNullOrEmpty(genericParameterName))
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.OnlyDependencySupportedForGenericParameter,
                        name,           /* the invalid parameter's name */
                        reader.Name),   /* the invalid value element's name */
                    reader);
            }
        }

        public static void GuardAttributeOccurrences(XmlReader reader, string typeAttributeName)
        {
            bool hasParameterType = !string.IsNullOrEmpty(reader.GetAttribute(typeAttributeName));
            bool hasGenericParameterName = !string.IsNullOrEmpty(reader.GetAttribute("genericParameterName"));
            if (hasParameterType && hasGenericParameterName)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.OnlyOneOfParameterTypeAndGenericParameterNameAllowed,
                        reader.GetAttribute("name")),
                    reader);
            }
            else if (!hasParameterType && !hasGenericParameterName)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NeitherParameterTypeNorGenericParameterNameSpecified,
                        reader.GetAttribute("name")),
                    reader);
            }
        }
    }
}
