// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using Unity.Configuration.ConfigurationHelpers;

namespace Unity.Configuration
{
    /// <summary>
    /// A configuration element that describes an instance to add to the container.
    /// </summary>
    public class InstanceElement : ContainerConfiguringElement
    {
        private const string NamePropertyName = "name";
        private const string TypeConverterTypeNamePropertyName = "typeConverter";
        private const string TypeNamePropertyName = "type";
        private const string ValuePropertyName = "value";

        /// <summary>
        /// Name to register instance under
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = false, DefaultValue = "")]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Value for this instance
        /// </summary>
        [ConfigurationProperty(ValuePropertyName, IsRequired = false)]
        public string Value
        {
            get { return (string)base[ValuePropertyName]; }
            set { base[ValuePropertyName] = value; }
        }

        /// <summary>
        /// Type of the instance. If not given, defaults to string
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = false, DefaultValue = "")]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Type name for the type converter to use to create the instance. If not
        /// given, defaults to the default type converter for this instance type.
        /// </summary>
        [ConfigurationProperty(TypeConverterTypeNamePropertyName, IsRequired = false, DefaultValue = "")]
        public string TypeConverterTypeName
        {
            get { return (string)base[TypeConverterTypeNamePropertyName]; }
            set { base[TypeConverterTypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Key used to keep these instances unique in the config collection.
        /// </summary>
        public override string Key
        {
            get { return "instance:" + this.Name + ":" + this.Value; }
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
            writer.WriteAttributeIfNotEmpty(InstanceElement.NamePropertyName, this.Name)
                .WriteAttributeIfNotEmpty(InstanceElement.ValuePropertyName, this.Value)
                .WriteAttributeIfNotEmpty(InstanceElement.TypeNamePropertyName, this.TypeName)
                .WriteAttributeIfNotEmpty(InstanceElement.TypeConverterTypeNamePropertyName, this.TypeConverterTypeName);
        }

        /// <summary>
        /// Add the instance defined by this element to the given container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        protected override void ConfigureContainer(IUnityContainer container)
        {
            Type instanceType = this.GetInstanceType();
            object instanceValue = this.GetInstanceValue();

            container.RegisterInstance(instanceType, this.Name, instanceValue);
        }

        private Type GetInstanceType()
        {
            return TypeResolver.ResolveTypeWithDefault(this.TypeName, typeof(string));
        }

        private object GetInstanceValue()
        {
            if (string.IsNullOrEmpty(this.Value) && string.IsNullOrEmpty(this.TypeConverterTypeName))
            {
                return null;
            }

            TypeConverter converter = this.GetTypeConverter();
            return converter.ConvertFromInvariantString(this.Value);
        }

        private TypeConverter GetTypeConverter()
        {
            if (!string.IsNullOrEmpty(this.TypeConverterTypeName))
            {
                Type converterType = TypeResolver.ResolveType(this.TypeConverterTypeName);
                return (TypeConverter)Activator.CreateInstance(converterType);
            }
            return TypeDescriptor.GetConverter(this.GetInstanceType());
        }
    }
}
