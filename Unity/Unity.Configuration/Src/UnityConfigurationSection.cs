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
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration section describing configuration for an <see cref="IUnityContainer"/>.
    /// </summary>
    public class UnityConfigurationSection : ConfigurationSection
    {
        private const string ContainersPropertyName = "";
        private const string TypeAliasesPropertyName = "aliases";
        private const string SectionExtensionsPropertyName = "extensions";
        private const string NamespacesPropertyName = "namespaces";
        private const string AssembliesPropertyName = "asemblies";

        [ThreadStatic]
        private static UnityConfigurationSection currentSection;

        private static readonly UnknownElementHandlerMap<UnityConfigurationSection> unknownElementHandlerMap
            = new UnknownElementHandlerMap<UnityConfigurationSection>
                {
                    { "typeAliases", (s, xr) => s.TypeAliases.Deserialize(xr) },
                    { "containers", (s, xr) => s.Containers.Deserialize(xr) },
                    { "alias", (s, xr) => s.ReadUnwrappedElement(xr, s.TypeAliases) },
                    { "sectionExtension", (s, xr) => s.DeserializeSectionExtension(xr) },
                    { "namespace", (s, xr) => s.ReadUnwrappedElement(xr, s.Namespaces) },
                    { "assembly", (s, xr) => s.ReadUnwrappedElement(xr, s.Assemblies) }
                };

        /// <summary>
        /// The current <see cref="UnityConfigurationSection"/> that is being deserialized
        /// or being configured from.
        /// </summary>
        public static UnityConfigurationSection CurrentSection
        {
            get { return currentSection; }
        }

        /// <summary>
        /// The set of containers defined in this configuration section.
        /// </summary>
        [ConfigurationProperty(ContainersPropertyName, IsDefaultCollection = true)]
        public ContainerElementCollection Containers
        {
            get
            {
                var containers = (ContainerElementCollection)base[ContainersPropertyName];
                containers.ContainingSection = this;
                return containers;
            }
        }

        /// <summary>
        /// The set of type aliases defined in this configuration file.
        /// </summary>
        [ConfigurationProperty(TypeAliasesPropertyName)]
        public AliasElementCollection TypeAliases
        {
            get { return (AliasElementCollection)base[TypeAliasesPropertyName]; }
        }

        /// <summary>
        /// Any schema extensions that are added.
        /// </summary>
        [ConfigurationProperty(SectionExtensionsPropertyName)]
        public SectionExtensionElementCollection SectionExtensions
        {
            get { return (SectionExtensionElementCollection) base[SectionExtensionsPropertyName]; }
        }

        /// <summary>
        /// Any namespaces added to the type search list.
        /// </summary>
        [ConfigurationProperty(NamespacesPropertyName)]
        public NamedElementCollection Namespaces
        {
            get { return (NamedElementCollection) base[NamespacesPropertyName]; }
        }

        /// <summary>
        /// Any assemblies added to the type search list.
        /// </summary>
        [ConfigurationProperty(AssembliesPropertyName)]
        public NamedElementCollection Assemblies
        {
            get { return (NamedElementCollection) base[AssembliesPropertyName]; }
        }


        /// <summary>
        /// Apply the configuration in the default container element to the given container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <returns>The passed in <paramref name="container"/>.</returns>
        public IUnityContainer Configure(IUnityContainer container)
        {
            return Configure(container, "");
        }

        /// <summary>
        /// Apply the configuration in the default container element to the given container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="configuredContainerName">Name of the container element to use to configure the container.</param>
        /// <returns>The passed in <paramref name="container"/>.</returns>
        public IUnityContainer Configure(IUnityContainer container, string configuredContainerName)
        {
            currentSection = this;
            TypeResolver.SetAliases(this);
            var containerElement = GuardContainerExists(configuredContainerName, Containers[configuredContainerName]);

            containerElement.ConfigureContainer(container);
            return container;
        }

        private static ContainerElement GuardContainerExists(string configuredContainerName, ContainerElement containerElement)
        {
            if(containerElement == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentUICulture,
                        Resources.NoSuchContainer, configuredContainerName),
                    "configuredContainerName");
            }
            return containerElement;
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> object, which reads from the configuration file. 
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException"><paramref name="reader"/> found no elements in the configuration file.
        ///                 </exception>
        protected override void DeserializeSection(XmlReader reader)
        {
            ExtensionElementMap.Clear();
            currentSection = this;
            base.DeserializeSection(reader);
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="elementName">The name of the unknown subelement.
        ///                 </param><param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.
        ///                     - or -
        ///                     One or more of the element's attributes is locked.
        ///                     - or -
        ///                 <paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.
        ///                     - or -
        ///                     The element has a Boolean attribute with an invalid value.
        ///                     - or -
        ///                     An attempt was made to deserialize a property more than once.
        ///                     - or -
        ///                     An attempt was made to deserialize a property that is not a valid member of the element.
        ///                     - or -
        ///                     The element cannot contain a CDATA or text element.
        ///                 </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return unknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        private void DeserializeSectionExtension(XmlReader reader)
        {
            TypeResolver.SetAliases(this);
            var element = this.ReadUnwrappedElement(reader, SectionExtensions);
            element.ExtensionObject.AddExtensions(new ExtensionContext(this, element.Prefix));
        }

        private class ExtensionContext : SectionExtensionContext
        {
            private readonly UnityConfigurationSection section;
            private readonly string prefix;

            public ExtensionContext(UnityConfigurationSection section, string prefix)
            {
                this.section = section;
                this.prefix = prefix;
            }

            /// <summary>
            /// Add a new alias to the configuration section. This is useful
            /// for those extensions that add commonly used types to configuration
            /// so users don't have to alias them repeatedly.
            /// </summary>
            /// <param name="newAlias">The alias to use.</param>
            /// <param name="aliasedType">Type the alias maps to.</param>
            public override void AddAlias(string newAlias, Type aliasedType)
            {
                string alias = newAlias;
                if(!string.IsNullOrEmpty(prefix))
                {
                    alias = prefix + "." + alias;
                }

                section.TypeAliases.Add(new AliasElement(alias, aliasedType));
            }

            /// <summary>
            /// Add a new element to the configuration section schema.
            /// </summary>
            /// <param name="tag">Tag name in the XML.</param>
            /// <param name="elementType">Type the tag maps to.</param>
            public override void AddElement(string tag, Type elementType)
            {
                if(typeof(ContainerConfiguringElement).IsAssignableFrom(elementType))
                {
                    ExtensionElementMap.AddContainerConfiguringElement(prefix, tag, elementType);
                }
                else if(typeof(InjectionMemberElement).IsAssignableFrom(elementType))
                {
                    ExtensionElementMap.AddInjectionMemberElement(prefix, tag, elementType);
                }
                else if(typeof(ParameterValueElement).IsAssignableFrom(elementType))
                {
                    ExtensionElementMap.AddParameterValueElement(prefix, tag, elementType);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture,
                        Resources.InvalidExtensionElementType,
                        elementType.Name));
                }
            }
        }
    }
}
