using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

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

            private static string CreateTag(string prefix, string tag)
            {
                if(string.IsNullOrEmpty(prefix))
                {
                    return tag;
                }
                return prefix + "." + tag;
            }
        }
        #endregion

    }
}
