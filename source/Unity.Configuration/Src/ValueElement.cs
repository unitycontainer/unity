// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Xml;
using Unity.Configuration.ConfigurationHelpers;
using Unity.Configuration.Properties;
using Unity.Utility;

namespace Unity.Configuration
{
    /// <summary>
    /// Element that describes a constant value that will be
    /// injected into the container.
    /// </summary>
    public class ValueElement : ParameterValueElement, IAttributeOnlyElement
    {
        private const string ValuePropertyName = "value";
        private const string TypeConverterTypeNamePropertyName = "typeConverter";

        /// <summary>
        /// Construct a new <see cref="ValueElement"/> object.
        /// </summary>
        public ValueElement()
        {
        }

        /// <summary>
        /// Construct a new <see cref="ValueElement"/> object,
        /// initializing properties from the contents of
        /// <paramref name="propertyValues"/>.
        /// </summary>
        /// <param name="propertyValues">Name/value pairs which
        /// contain the values to initialize properties to.</param>
        public ValueElement(IDictionary<string, string> propertyValues)
        {
            ParameterValueElement.GuardPropertyValueIsPresent(propertyValues, "value");
            this.Value = propertyValues["value"];
            if (propertyValues.ContainsKey("typeConverter"))
            {
                this.TypeConverterTypeName = propertyValues["typeConverter"];
            }
        }

        /// <summary>
        /// Value for this element
        /// </summary>
        [ConfigurationProperty(ValuePropertyName)]
        public string Value
        {
            get { return (string)base[ValuePropertyName]; }
            set { base[ValuePropertyName] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty(TypeConverterTypeNamePropertyName, IsRequired = false)]
        public string TypeConverterTypeName
        {
            get { return (string)base[TypeConverterTypeNamePropertyName]; }
            set { base[TypeConverterTypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        void IAttributeOnlyElement.SerializeContent(XmlWriter writer)
        {
            this.SerializeContent(writer);
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>. This
        /// method always outputs an explicit &lt;dependency&gt; tag, instead of providing
        /// attributes to the parent method.
        /// </summary>
        /// <param name="writer">Writer to send XML content to.</param>
        public override void SerializeContent(XmlWriter writer)
        {
            writer.WriteAttributeIfNotEmpty(ValuePropertyName, this.Value)
                .WriteAttributeIfNotEmpty(TypeConverterTypeNamePropertyName, this.TypeConverterTypeName);
        }

        /// <summary>
        /// Generate an <see cref="InjectionParameterValue"/> object
        /// that will be used to configure the container for a type registration.
        /// </summary>
        /// <param name="container">Container that is being configured. Supplied in order
        /// to let custom implementations retrieve services; do not configure the container
        /// directly in this method.</param>
        /// <param name="parameterType">Type of the parameter to get the value for.</param>
        /// <returns>The required <see cref="InjectionParameterValue"/> object.</returns>
        public override InjectionParameterValue GetInjectionParameterValue(IUnityContainer container, Type parameterType)
        {
            this.CheckNonGeneric(parameterType);

            var converter = this.GetTypeConverter(parameterType);
            return new InjectionParameter(parameterType, converter.ConvertFromInvariantString(this.Value));
        }

        private void CheckNonGeneric(Type parameterType)
        {
            if (parameterType.IsGenericParameter)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ValueNotAllowedForGenericParameterType,
                        parameterType.Name,
                        this.Value));
            }

            var reflector = new ReflectionHelper(parameterType);

            if (reflector.IsOpenGeneric)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ValueNotAllowedForOpenGenericType,
                        parameterType.Name,
                        this.Value));
            }

            if (reflector.IsGenericArray)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ValueNotAllowedForGenericArrayType,
                        parameterType.Name,
                        this.Value));
            }
        }

        private TypeConverter GetTypeConverter(Type parameterType)
        {
            if (!string.IsNullOrEmpty(this.TypeConverterTypeName))
            {
                Type converterType = TypeResolver.ResolveType(this.TypeConverterTypeName);
                return (TypeConverter)Activator.CreateInstance(converterType);
            }
            return TypeDescriptor.GetConverter(parameterType);
        }
    }
}
