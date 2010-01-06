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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Element that describes a constant value that will be
    /// injected into the container.
    /// </summary>
    public class ValueElement : ParameterValueElement
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
            GuardPropertyValueIsPresent(propertyValues, "value");
            Value = propertyValues["value"];
            if(propertyValues.ContainsKey("typeConverter"))
            {
                TypeConverterTypeName = propertyValues["typeConverter"];
            }
        }

        /// <summary>
        /// Value for this element
        /// </summary>
        [ConfigurationProperty(ValuePropertyName)]
        public string Value
        {
            get { return (string) base[ValuePropertyName]; }
            set { base[ValuePropertyName] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty(TypeConverterTypeNamePropertyName, IsRequired = false)]
        public string TypeConverterTypeName
        {
            get { return (string) base[TypeConverterTypeNamePropertyName]; }
            set { base[TypeConverterTypeNamePropertyName] = value; }
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
            var converter = GetTypeConverter(parameterType);
            return new InjectionParameter(parameterType, converter.ConvertFromInvariantString(Value));
        }

        private TypeConverter GetTypeConverter(Type parameterType)
        {
            if (!string.IsNullOrEmpty(TypeConverterTypeName))
            {
                Type converterType = TypeResolver.ResolveType(TypeConverterTypeName);
                return (TypeConverter)Activator.CreateInstance(converterType);
            }
            return TypeDescriptor.GetConverter(parameterType);
        }
    }
}
