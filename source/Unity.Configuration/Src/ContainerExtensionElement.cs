// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element representing an extension to add to a container.
    /// </summary>
    public class ContainerExtensionElement : ContainerConfiguringElement
    {
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Type of the extension to add.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Add the extension specified in this element to the container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        protected override void ConfigureContainer(IUnityContainer container)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(container, "container");

            var extensionType = this.GetExtensionType();
            var extension = (UnityContainerExtension)container.Resolve(extensionType);
            container.AddExtension(extension);
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void SerializeContent(System.Xml.XmlWriter writer)
        {
            Guard.ArgumentNotNull(writer, "writer");
            writer.WriteAttributeString(TypeNamePropertyName, this.TypeName);
        }

        private Type GetExtensionType()
        {
            return TypeResolver.ResolveType(this.TypeName, true);
        }
    }
}
