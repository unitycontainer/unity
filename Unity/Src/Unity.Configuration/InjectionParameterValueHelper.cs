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
            string genericParameter;
            bool isGenericParameterArray;
            ExtractGenericParameter(genericParameterName, out genericParameter, out isGenericParameterArray);

            if (!string.IsNullOrEmpty(genericParameter))
            {
                if (!isGenericParameterArray)
                {
                    if (valueElement == null)
                    {
                        return new GenericParameter(genericParameter);
                    }
                    else
                    {
                        DependencyValueElement dependencyElement = valueElement as DependencyValueElement;
                        if (dependencyElement != null)
                        {
                            if (!string.IsNullOrEmpty(dependencyElement.Name))
                            {
                                return new GenericParameter(genericParameter, dependencyElement.Name);
                            }
                            else
                            {
                                return new GenericParameter(genericParameter);
                            }
                        }
                        else
                        {
                            // should not happen - checked during config deserialization
                            throw new InvalidOperationException(Resources.InvalidConfiguration);
                        }
                    }
                }
                else
                {
                    if (valueElement == null)
                    {
                        return new GenericResolvedArrayParameter(genericParameter);
                    }
                    else
                    {
                        ArrayValueElement arrayElement = valueElement as ArrayValueElement;
                        if (arrayElement != null)
                        {
                            return arrayElement.CreateParameterValue(genericParameter);
                        }
                        else
                        {
                            // should not happen - checked during config deserialization
                            throw new InvalidOperationException(Resources.InvalidConfiguration);
                        }
                    }
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

        public static bool DeserializeSingleUnrecognizedElement(
            string elementName,
            XmlReader reader,
            string name,
            string genericParameterName,
            ref InjectionParameterValueElement valueElement)
        {
            GuardOnlyOneValue(valueElement, name);
            return DeserializeUnrecognizedElement(
                elementName,
                reader,
                name,
                genericParameterName,
                out valueElement);
        }

        public static bool DeserializeUnrecognizedElement(
            string elementName,
            XmlReader reader,
            string name,
            string genericParameterName,
            out InjectionParameterValueElement valueElement)
        {

            string genericParameter;
            bool isGenericParameterArray;
            ExtractGenericParameter(genericParameterName, out genericParameter, out isGenericParameterArray);

            switch (elementName)
            {
                case "value":
                    GuardNotGeneric(reader, genericParameterName, name);
                    return DeserializeValueElement(
                        reader,
                        out valueElement);

                case "dependency":
                    GuardNotGenericArray(reader, isGenericParameterArray, name);
                    return DeserializeDependencyValueElement(
                        reader,
                        genericParameterName,
                        name,
                        out valueElement);

                case "array":
                    if (!isGenericParameterArray)
                    {
                        GuardNotGeneric(reader, genericParameterName, name);
                    }
                    return DeserializeArrayValueElement(
                        reader,
                        out valueElement);

                default:
                    GuardNotGeneric(reader, genericParameterName, name);
                    return DeserializePolymorphicElement(
                        reader,
                        out valueElement);
            }
        }

        private static void ExtractGenericParameter(
            string genericParameterName,
            out string genericParameter,
            out bool isGenericParameterArray)
        {
            genericParameter = genericParameterName;
            isGenericParameterArray = false;

            if (genericParameter != null)
            {
                if (genericParameter.EndsWith("[]"))
                {
                    genericParameter = genericParameter.Substring(0, genericParameter.Length - 2);
                    isGenericParameterArray = true;
                }
            }
        }

        private static bool DeserializeValueElement(
            XmlReader reader,
            out InjectionParameterValueElement valueElement)
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
            out InjectionParameterValueElement valueElement)
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

        private static bool DeserializeArrayValueElement(
            XmlReader reader,
            out InjectionParameterValueElement valueElement)
        {
            ArrayValueElement element = new ArrayValueElement();
            element.DeserializeElement(reader);

            valueElement = element;
            return true;
        }

        private static bool DeserializePolymorphicElement(
            XmlReader reader, 
            out InjectionParameterValueElement valueElement)
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

            valueElement = null;
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
                        Resources.GenericParameterNotSupported,
                        name,           /* the invalid parameter's name */
                        reader.Name),   /* the invalid value element's name */
                    reader);
            }
        }

        public static void GuardNotGenericArray(XmlReader reader, bool isGenericParameterArray, string name)
        {
            if (isGenericParameterArray)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.GenericParameterArrayNotSupported,
                        name,           /* the invalid parameter's name */
                        reader.Name),   /* the invalid value element's name */
                    reader);
            }
        }

        public static void GuardAttributeOccurrences(XmlReader reader, string typeAttributeName)
        {
            int attributeCount
                = GetAttributeCount(reader, typeAttributeName)
                    + GetAttributeCount(reader, "genericParameterName")
                    + GetAttributeCount(reader, "genericParameterArray");

            if (attributeCount > 1)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.OnlyOneOfParameterTypeAndGenericParameterNameAllowed,
                        reader.GetAttribute("name")),
                    reader);
            }
            else if (attributeCount == 0)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NeitherParameterTypeNorGenericParameterNameSpecified,
                        reader.GetAttribute("name")),
                    reader);
            }
        }

        private static int GetAttributeCount(XmlReader reader, string attributeName)
        {
            return string.IsNullOrEmpty(reader.GetAttribute(attributeName)) ? 0 : 1;
        }
    }
}
