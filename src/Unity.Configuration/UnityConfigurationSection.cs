// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Xml;
using Unity.Configuration.ConfigurationHelpers;
using Unity.Configuration.Properties;

namespace Unity.Configuration
{
    /// <summary>
    /// A configuration section describing configuration for an <see cref="IUnityContainer"/>.
    /// </summary>
    public class UnityConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The name of the section where unity configuration is expected to be found.
        /// </summary>
        public const string SectionName = "unity";

        /// <summary>
        /// XML Namespace string used for IntelliSense in this section.
        /// </summary>
        public const string XmlNamespace = "http://schemas.microsoft.com/practices/2010/unity";

        private const string ContainersPropertyName = "";
        private const string TypeAliasesPropertyName = "aliases";
        private const string SectionExtensionsPropertyName = "extensions";
        private const string NamespacesPropertyName = "namespaces";
        private const string AssembliesPropertyName = "assemblies";
        private const string XmlnsPropertyName = "xmlns";

        private static readonly UnknownElementHandlerMap<UnityConfigurationSection> UnknownElementHandlerMap
            = new UnknownElementHandlerMap<UnityConfigurationSection>
                {
                    { "typeAliases", (s, xr) => s.TypeAliases.Deserialize(xr) },
                    { "containers", (s, xr) => s.Containers.Deserialize(xr) },
                    { "alias", (s, xr) => s.ReadUnwrappedElement(xr, s.TypeAliases) },
                    { "sectionExtension", (s, xr) => s.DeserializeSectionExtension(xr) },
                    { "namespace", (s, xr) => s.ReadUnwrappedElement(xr, s.Namespaces) },
                    { "assembly", (s, xr) => s.ReadUnwrappedElement(xr, s.Assemblies) }
                };

        [ThreadStatic]
        private static UnityConfigurationSection currentSection;

        /// <summary>
        /// The current <see cref="UnityConfigurationSection"/> that is being deserialized
        /// or being configured from.
        /// </summary>
        public static UnityConfigurationSection CurrentSection
        {
            get { return currentSection; }
        }

        /// <summary>
        /// Storage for XML namespace. The namespace isn't used or validated by config, but
        /// it is useful for Visual Studio XML IntelliSense to kick in.
        /// </summary>
        [ConfigurationProperty(XmlnsPropertyName, IsRequired = false, DefaultValue = XmlNamespace)]
        public string Xmlns
        {
            get { return (string)base[XmlnsPropertyName]; }
            set { base[XmlnsPropertyName] = value; }
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
            get { return (SectionExtensionElementCollection)base[SectionExtensionsPropertyName]; }
        }

        /// <summary>
        /// Any namespaces added to the type search list.
        /// </summary>
        [ConfigurationProperty(NamespacesPropertyName)]
        public NamespaceElementCollection Namespaces
        {
            get { return (NamespaceElementCollection)base[NamespacesPropertyName]; }
        }

        /// <summary>
        /// Any assemblies added to the type search list.
        /// </summary>
        [ConfigurationProperty(AssembliesPropertyName)]
        public AssemblyElementCollection Assemblies
        {
            get { return (AssemblyElementCollection)base[AssembliesPropertyName]; }
        }

        /// <summary>
        /// Apply the configuration in the default container element to the given container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <returns>The passed in <paramref name="container"/>.</returns>
        public IUnityContainer Configure(IUnityContainer container)
        {
            return this.Configure(container, String.Empty);
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
            var containerElement = GuardContainerExists(configuredContainerName, this.Containers[configuredContainerName]);

            containerElement.ConfigureContainer(container);
            return container;
        }

        private static ContainerElement GuardContainerExists(string configuredContainerName, ContainerElement containerElement)
        {
            if (containerElement == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.NoSuchContainer, configuredContainerName),
                    "configuredContainerName");
            }
            return containerElement;
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> object, which reads from the configuration file. </param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException"><paramref name="reader"/> found no elements in the configuration file.</exception>
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
        /// <param name="elementName">The name of the unknown subelement.</param>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.</param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">
        ///                     The element identified by <paramref name="elementName"/> is locked.
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
        /// </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return UnknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        private void DeserializeSectionExtension(XmlReader reader)
        {
            TypeResolver.SetAliases(this);
            var element = this.ReadUnwrappedElement(reader, this.SectionExtensions);
            element.ExtensionObject.AddExtensions(new ExtensionContext(this, element.Prefix));
        }

        /// <summary>
        /// Creates an XML string containing an unmerged view of the <see cref="T:System.Configuration.ConfigurationSection"/> object as a single section to write to a file.
        /// </summary>
        /// <returns>
        /// An XML string containing an unmerged view of the <see cref="T:System.Configuration.ConfigurationSection"/> object.
        /// </returns>
        /// <param name="parentElement">The <see cref="T:System.Configuration.ConfigurationElement"/> instance to use as the parent when performing the un-merge.</param>
        /// <param name="name">The name of the section to create.</param>
        /// <param name="saveMode">The <see cref="T:System.Configuration.ConfigurationSaveMode"/> instance to use when writing to a string.</param>
        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            ExtensionElementMap.Clear();
            currentSection = this;
            TypeResolver.SetAliases(this);
            this.InitializeSectionExtensions();

            var sb = new StringBuilder();
            using (var writer = MakeXmlWriter(sb))
            {
                writer.WriteStartElement(name, XmlNamespace);
                writer.WriteAttributeString("xmlns", XmlNamespace);
                this.TypeAliases.SerializeElementContents(writer, "alias");
                this.Namespaces.SerializeElementContents(writer, "namespace");
                this.Assemblies.SerializeElementContents(writer, "assembly");
                this.SectionExtensions.SerializeElementContents(writer, "sectionExtension");
                this.Containers.SerializeElementContents(writer, "container");
                writer.WriteEndElement();
            }

            return sb.ToString();
        }

        private static XmlWriter MakeXmlWriter(StringBuilder sb)
        {
            var settings = new XmlWriterSettings
                               {
                                   Indent = true,
                                   OmitXmlDeclaration = true,
                                   ConformanceLevel = ConformanceLevel.Fragment
                               };

            return XmlWriter.Create(sb, settings);
        }

        private void InitializeSectionExtensions()
        {
            foreach (var extensionElement in this.SectionExtensions)
            {
                SectionExtension extensionObject = extensionElement.ExtensionObject;
                extensionObject.AddExtensions(new ExtensionContext(this, extensionElement.Prefix, false));
            }
        }

        private class ExtensionContext : SectionExtensionContext
        {
            private readonly UnityConfigurationSection section;
            private readonly string prefix;
            private readonly bool saveAliases;

            public ExtensionContext(UnityConfigurationSection section, string prefix)
                : this(section, prefix, true)
            {
            }

            public ExtensionContext(UnityConfigurationSection section, string prefix, bool saveAliases)
            {
                this.section = section;
                this.prefix = prefix;
                this.saveAliases = saveAliases;
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
                if (!this.saveAliases)
                {
                    return;
                }

                string alias = newAlias;
                if (!string.IsNullOrEmpty(this.prefix))
                {
                    alias = this.prefix + "." + alias;
                }

                this.section.TypeAliases.Add(new AliasElement(alias, aliasedType));
            }

            /// <summary>
            /// Add a new element to the configuration section schema.
            /// </summary>
            /// <param name="tag">Tag name in the XML.</param>
            /// <param name="elementType">Type the tag maps to.</param>
            public override void AddElement(string tag, Type elementType)
            {
                Unity.Utility.Guard.ArgumentNotNull(elementType, "elementType");

                if (typeof(ContainerConfiguringElement).IsAssignableFrom(elementType))
                {
                    ExtensionElementMap.AddContainerConfiguringElement(this.prefix, tag, elementType);
                }
                else if (typeof(InjectionMemberElement).IsAssignableFrom(elementType))
                {
                    ExtensionElementMap.AddInjectionMemberElement(this.prefix, tag, elementType);
                }
                else if (typeof(ParameterValueElement).IsAssignableFrom(elementType))
                {
                    ExtensionElementMap.AddParameterValueElement(this.prefix, tag, elementType);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidExtensionElementType,
                        elementType.Name));
                }
            }
        }
    }
}
