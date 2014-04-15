// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element storing information about a single type alias.
    /// </summary>
    public class AliasElement : DeserializableConfigurationElement
    {
        private const string AliasPropertyName = "alias";
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Construct a new, uninitialized <see cref="AliasElement"/>.
        /// </summary>
        public AliasElement()
        {
        }

        /// <summary>
        /// Construct a new <see cref="AliasElement"/> that is initialized
        /// to alias <paramref name="alias"/> to the target <paramref name="targetType"/>.
        /// </summary>
        /// <param name="alias">Alias to use.</param>
        /// <param name="targetType">Type that is aliased.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public AliasElement(string alias, Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");

            this.Alias = alias;
            this.TypeName = targetType.AssemblyQualifiedName;
        }

        /// <summary>
        /// The alias used for this type.
        /// </summary>
        [ConfigurationProperty(AliasPropertyName, IsRequired = true, IsKey = true)]
        public string Alias
        {
            get { return (string)base[AliasPropertyName]; }
            set { base[AliasPropertyName] = value; }
        }

        /// <summary>
        /// The fully qualified name this alias refers to.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
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
        public override void SerializeContent(XmlWriter writer)
        {
            Guard.ArgumentNotNull(writer, "writer");
            writer.WriteAttributeString(AliasPropertyName, this.Alias);
            writer.WriteAttributeString(TypeNamePropertyName, this.TypeName);
        }
    }
}
