// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.Linq;
using System.Xml;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element class defining the set of registrations to be
    /// put into a container.
    /// </summary>
    public class ContainerElement : DeserializableConfigurationElement
    {
        private const string RegistrationsPropertyName = "";
        private const string NamePropertyName = "name";
        private const string InstancesPropertyName = "instances";
        private const string ExtensionsPropertyName = "extensions";

        private static readonly UnknownElementHandlerMap<ContainerElement> UnknownElementHandlerMap =
            new UnknownElementHandlerMap<ContainerElement>
                {
                    { "types", (ce, xr) => ce.Registrations.Deserialize(xr) },
                    { "extension", (ce, xr) => ce.ReadUnwrappedElement(xr, ce.Extensions) },
                    { "instance", (ce, xr) => ce.ReadUnwrappedElement(xr, ce.Instances) }
                };

        private readonly ContainerConfiguringElementCollection configuringElements = new ContainerConfiguringElementCollection();

        internal UnityConfigurationSection ContainingSection { get; set; }

        /// <summary>
        /// Name for this container configuration as given in the config file.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsKey = true, DefaultValue = "")]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// The type registrations in this container.
        /// </summary>
        [ConfigurationProperty(RegistrationsPropertyName, IsDefaultCollection = true)]
        public RegisterElementCollection Registrations
        {
            get { return (RegisterElementCollection)base[RegistrationsPropertyName]; }
        }

        /// <summary>
        /// Any instances to register in the container.
        /// </summary>
        [ConfigurationProperty(InstancesPropertyName)]
        public InstanceElementCollection Instances
        {
            get { return (InstanceElementCollection)base[InstancesPropertyName]; }
        }

        /// <summary>
        /// Any extensions to add to the container.
        /// </summary>
        [ConfigurationProperty(ExtensionsPropertyName)]
        public ContainerExtensionElementCollection Extensions
        {
            get { return (ContainerExtensionElementCollection)base[ExtensionsPropertyName]; }
        }

        /// <summary>
        /// Set of any extra configuration elements that were added by a
        /// section extension.
        /// </summary>
        /// <remarks>
        /// This is not marked as a configuration property because we don't want
        /// the actual property to show up as a nested element in the configuration.</remarks>
        public ContainerConfiguringElementCollection ConfiguringElements
        {
            get { return this.configuringElements; }
        }

        /// <summary>
        /// Original configuration API kept for backwards compatibility.
        /// </summary>
        /// <param name="container">Container to configure</param>
        [Obsolete("Use the UnityConfigurationSection.Configure(container, name) method instead")]
        public void Configure(IUnityContainer container)
        {
            this.ContainingSection.Configure(container, this.Name);
        }

        /// <summary>
        /// Apply the configuration information in this element to the
        /// given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        internal void ConfigureContainer(IUnityContainer container)
        {
            this.Extensions.Cast<ContainerConfiguringElement>()
                .Concat(this.Registrations.Cast<ContainerConfiguringElement>())
                .Concat(this.Instances.Cast<ContainerConfiguringElement>())
                .Concat(this.ConfiguringElements)
                .ForEach(element => element.ConfigureContainerInternal(container));
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="elementName">The name of the unknown subelement.</param>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.</param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.
        /// - or -
        /// One or more of the element's attributes is locked.
        /// - or -
        /// <paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.
        /// - or -
        /// The element has a Boolean attribute with an invalid value.
        /// - or -
        /// An attempt was made to deserialize a property more than once.
        /// - or -
        /// An attempt was made to deserialize a property that is not a valid member of the element.
        /// - or -
        /// The element cannot contain a CDATA or text element.
        /// </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return UnknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                this.DeserializeContainerConfiguringElement(elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        public override void SerializeContent(XmlWriter writer)
        {
            writer.WriteAttributeIfNotEmpty(ContainerElement.NamePropertyName, this.Name);

            this.Extensions.SerializeElementContents(writer, "extension");
            this.Registrations.SerializeElementContents(writer, "register");
            this.Instances.SerializeElementContents(writer, "instance");
            this.SerializeContainerConfiguringElements(writer);
        }

        private bool DeserializeContainerConfiguringElement(string elementName, XmlReader reader)
        {
            Type elementType = ExtensionElementMap.GetContainerConfiguringElementType(elementName);
            if (elementType != null)
            {
                this.ReadElementByType(reader, elementType, this.ConfiguringElements);
                return true;
            }
            return false;
        }

        private void SerializeContainerConfiguringElements(XmlWriter writer)
        {
            foreach (var element in this.ConfiguringElements)
            {
                string tag = ExtensionElementMap.GetTagForExtensionElement(element);
                writer.WriteElement(tag, element.SerializeContent);
            }
        }
    }
}
