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
using System.Linq;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element class defining the set of registrations to be
    /// put into a container.
    /// </summary>
    public class ContainerElement : ConfigurationElement
    {
        private const string RegistrationsPropertyName = "";
        private const string NamePropertyName = "name";
        private const string InstancesPropertyName = "instances";
        private const string ExtensionsPropertyName = "extensions";

        private static readonly UnknownElementHandlerMap<ContainerElement> unknownElementHandlerMap =
            new UnknownElementHandlerMap<ContainerElement>
                {
                    {"types", (ce, xr) => ce.Registrations.Deserialize(xr) },
                    {"extension", (ce, xr) => ce.ReadUnwrappedElement(xr, ce.Extensions) },
                    {"instance", (ce, xr) => ce.ReadUnwrappedElement(xr, ce.Instances) }
                };

        private readonly ContainerConfiguringElementCollection configuringElements = new ContainerConfiguringElementCollection();

        internal UnityConfigurationSection ContainingSection { get; set; }

        /// <summary>
        /// Name for this container configuration as given in the config file.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsKey = true, DefaultValue = "")]
        public string Name
        {
            get { return (string) base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// The type registrations in this container.
        /// </summary>
        [ConfigurationProperty(RegistrationsPropertyName, IsDefaultCollection =  true)]
        public RegisterElementCollection Registrations
        {
            get { return (RegisterElementCollection) base[RegistrationsPropertyName]; }
        }

        /// <summary>
        /// Any instances to register in the container.
        /// </summary>
        [ConfigurationProperty(InstancesPropertyName)]
        public InstanceElementCollection Instances
        {
            get { return (InstanceElementCollection) base[InstancesPropertyName]; }
        }

        /// <summary>
        /// Any extensions to add to the container.
        /// </summary>
        [ConfigurationProperty(ExtensionsPropertyName)]
        public ContainerExtensionElementCollection Extensions
        {
            get { return (ContainerExtensionElementCollection) base[ExtensionsPropertyName]; }
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
            get { return configuringElements; }
        }

        /// <summary>
        /// Original configuration API kept for backwards compatibility.
        /// </summary>
        /// <param name="container">Container to configure</param>
        [Obsolete("Use the UnityConfigurationSection.Configure(container, name) method instead")]
        public void Configure(IUnityContainer container)
        {
            ContainingSection.Configure(container, Name);
        }

        /// <summary>
        /// Apply the configuration information in this element to the
        /// given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        internal void ConfigureContainer(IUnityContainer container)
        {
            Extensions.Cast<ContainerConfiguringElement>()
                .Concat(Registrations.Cast<ContainerConfiguringElement>())
                .Concat(Instances.Cast<ContainerConfiguringElement>())
                .Concat(ConfiguringElements)
                .ForEach(element => element.ConfigureContainerInternal(container));
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
                DeserializeContainerConfiguringElement(elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        private bool DeserializeContainerConfiguringElement(string elementName, XmlReader reader)
        {
            Type elementType = ExtensionElementMap.GetContainerConfiguringElementType(elementName);
            if(elementType != null)
            {
                this.ReadElementByType(reader, elementType, ConfiguringElements);
                return true;
            }
            return false;
        }
    }
}
