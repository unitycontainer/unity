// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element that represents lifetime managers.
    /// </summary>
    public class LifetimeElement : DeserializableConfigurationElement
    {
        private const string TypeConverterTypeNamePropertyName = "typeConverter";
        private const string TypeNamePropertyName = "type";
        private const string ValuePropertyName = "value";

        /// <summary>
        /// Type of the lifetime manager.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true, DefaultValue = "")]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Extra initialization information used by the type converter for this lifetime manager.
        /// </summary>
        [ConfigurationProperty(ValuePropertyName, IsRequired = false)]
        public string Value
        {
            get { return (string)base[ValuePropertyName]; }
            set { base[ValuePropertyName] = value; }
        }

        /// <summary>
        /// Type of <see cref="TypeConverter"/> to use to create the
        /// lifetime manager.
        /// </summary>
        [ConfigurationProperty(TypeConverterTypeNamePropertyName, IsRequired = false)]
        public string TypeConverterTypeName
        {
            get { return (string)base[TypeConverterTypeNamePropertyName]; }
            set { base[TypeConverterTypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Create the <see cref="LifetimeManager"/> described by
        /// this element.
        /// </summary>
        /// <returns>A <see cref="LifetimeManager"/> instance.</returns>
        public LifetimeManager CreateLifetimeManager()
        {
            TypeConverter converter = this.GetTypeConverter();
            if (converter == null)
            {
                return (LifetimeManager)Activator.CreateInstance(this.GetLifetimeType());
            }
            return (LifetimeManager)converter.ConvertFrom(this.Value);
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
            if (!string.IsNullOrEmpty(this.Value))
            {
                writer.WriteAttributeString(ValuePropertyName, this.Value);
            }
            if (!string.IsNullOrEmpty(this.TypeConverterTypeName))
            {
                writer.WriteAttributeString(TypeConverterTypeNamePropertyName, this.TypeConverterTypeName);
            }
        }

        private Type GetLifetimeType()
        {
            return TypeResolver.ResolveTypeWithDefault(this.TypeName,
                typeof(TransientLifetimeManager));
        }

        private TypeConverter GetTypeConverter()
        {
            if (string.IsNullOrEmpty(this.Value) && string.IsNullOrEmpty(this.TypeConverterTypeName))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(this.TypeConverterTypeName))
            {
                Type converterType = TypeResolver.ResolveType(this.TypeConverterTypeName);
                return (TypeConverter)Activator.CreateInstance(converterType);
            }
            return TypeDescriptor.GetConverter(this.GetLifetimeType());
        }
    }
}
