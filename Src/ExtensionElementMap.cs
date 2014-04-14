// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Configuration.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// This class manages the set of extension elements
    /// added by section elements.
    /// </summary>
    public static class ExtensionElementMap
    {
        #region Singleton side of the interface

        [ThreadStatic]
        private static ExtensionElementMapImpl instance;

        private static ExtensionElementMapImpl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ExtensionElementMapImpl();
                }
                return instance;
            }
        }

        /// <summary>
        /// Clear the current set of extension elements.
        /// </summary>
        public static void Clear()
        {
            Instance.Clear();
        }

        /// <summary>
        /// Register a new ContainerExtensionConfigurationElement with he section so it
        /// can be read.
        /// </summary>
        /// <param name="prefix">prefix if any.</param>
        /// <param name="tag">tag name.</param>
        /// <param name="elementType">Type of element to register.</param>
        public static void AddContainerConfiguringElement(string prefix, string tag, Type elementType)
        {
            Instance.AddContainerConfiguringElement(prefix, tag, elementType);
        }

        /// <summary>
        /// Register a new <see cref="InjectionMemberElement"/> with the section
        /// so it can be read.
        /// </summary>
        /// <param name="prefix">prefix if any.</param>
        /// <param name="tag">Tag name.</param>
        /// <param name="elementType">Type of element to register.</param>
        public static void AddInjectionMemberElement(string prefix, string tag, Type elementType)
        {
            Instance.AddInjectionMemberElement(prefix, tag, elementType);
        }

        /// <summary>
        /// Register a new <see cref="ParameterValueElement"/> with the section
        /// so it can be read.
        /// </summary>
        /// <param name="prefix">prefix if any.</param>
        /// <param name="tag">Tag name.</param>
        /// <param name="elementType">Type of element to register.</param>
        public static void AddParameterValueElement(string prefix, string tag, Type elementType)
        {
            Instance.AddParameterValueElement(prefix, tag, elementType);
        }

        /// <summary>
        /// Retrieve the <see cref="ContainerConfiguringElement"/> registered for the given
        /// tag.
        /// </summary>
        /// <param name="tag">Tag to look up.</param>
        /// <returns>Type of element, or null if not registered.</returns>
        public static Type GetContainerConfiguringElementType(string tag)
        {
            return Instance.GetContainerConfiguringElementType(tag);
        }

        /// <summary>
        /// Retrieve the ContainerExtensionConfigurationElement registered for the given
        /// tag.
        /// </summary>
        /// <param name="tag">Tag to look up.</param>
        /// <returns>Type of element, or null if not registered.</returns>
        public static Type GetInjectionMemberElementType(string tag)
        {
            return Instance.GetInjectionMemberElementType(tag);
        }

        /// <summary>
        /// Retrieve the ContainerExtensionConfigurationElement registered for the given
        /// tag.
        /// </summary>
        /// <param name="tag">Tag to look up.</param>
        /// <returns>Type of element, or null if not registered.</returns>
        public static Type GetParameterValueElementType(string tag)
        {
            return Instance.GetParameterValueElementType(tag);
        }

        /// <summary>
        /// Retrieve the correct tag to use when serializing the given
        /// <paramref name="element"/> to XML.
        /// </summary>
        /// <param name="element">Element to be serialized.</param>
        /// <returns>The tag for that element type.</returns>
        /// <exception cref="ArgumentException"> if the element is of a type that
        /// is not registered with the section already.</exception>
        public static string GetTagForExtensionElement(ConfigurationElement element)
        {
            Guard.ArgumentNotNull(element, "element");
            return Instance.GetTagForExtensionElement(element);
        }

        #endregion

        #region Instance implementation of the interface

        private class ExtensionElementMapImpl
        {
            private readonly Dictionary<string, Type> containerConfiguringElements = new Dictionary<string, Type>();
            private readonly Dictionary<string, Type> injectionMemberElements = new Dictionary<string, Type>();
            private readonly Dictionary<string, Type> parameterValueElements = new Dictionary<string, Type>();

            public void Clear()
            {
                containerConfiguringElements.Clear();
                injectionMemberElements.Clear();
                parameterValueElements.Clear();
            }

            public void AddContainerConfiguringElement(string prefix, string tag, Type elementType)
            {
                containerConfiguringElements[CreateTag(prefix, tag)] = elementType;
            }

            public void AddInjectionMemberElement(string prefix, string tag, Type elementType)
            {
                injectionMemberElements[CreateTag(prefix, tag)] = elementType;
            }

            public void AddParameterValueElement(string prefix, string tag, Type elementType)
            {
                parameterValueElements[CreateTag(prefix, tag)] = elementType;
            }

            public Type GetContainerConfiguringElementType(string tag)
            {
                return containerConfiguringElements.GetOrNull(tag);
            }

            public Type GetInjectionMemberElementType(string tag)
            {
                return injectionMemberElements.GetOrNull(tag);
            }

            public Type GetParameterValueElementType(string tag)
            {
                return parameterValueElements.GetOrNull(tag);
            }

            public string GetTagForExtensionElement(ConfigurationElement element)
            {
                Type elementType = element.GetType();

                Dictionary<string, Type> dictToSearch = GetDictToSearch(elementType);

                foreach (var keyValue in dictToSearch)
                {
                    if (keyValue.Value == elementType)
                    {
                        return keyValue.Key;
                    }
                }

                throw ElementTypeNotFound(elementType);
            }

            private Dictionary<string, Type> GetDictToSearch(Type elementType)
            {
                if (typeof(ContainerConfiguringElement).IsAssignableFrom(elementType))
                {
                    return containerConfiguringElements;
                }
                if (typeof(InjectionMemberElement).IsAssignableFrom(elementType))
                {
                    return injectionMemberElements;
                }
                if (typeof(ParameterValueElement).IsAssignableFrom(elementType))
                {
                    return parameterValueElements;
                }
                throw ElementTypeNotFound(elementType);
            }

            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly",
                Justification = "Factory method to create exception for callers.")]
            private static Exception ElementTypeNotFound(Type elementType)
            {
                return new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.ElementTypeNotRegistered,
                        elementType), "memberElement");
            }

            private static string CreateTag(string prefix, string tag)
            {
                if (string.IsNullOrEmpty(prefix))
                {
                    return tag;
                }
                return prefix + "." + tag;
            }
        }
        #endregion
    }
}