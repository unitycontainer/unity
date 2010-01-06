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
using System.ComponentModel;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element that represents lifetime managers.
    /// </summary>
    public class LifetimeElement : ConfigurationElement
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
            get { return (string) base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Extra initialization information used by the type converter for this lifetime manager.
        /// </summary>
        [ConfigurationProperty(ValuePropertyName, IsRequired = false)]
        public string Value
        {
            get { return (string) base[ValuePropertyName]; }
            set { base[ValuePropertyName] = value; }
        }

        /// <summary>
        /// Type of <see cref="TypeConverter"/> to use to create the
        /// lifetime manager.
        /// </summary>
        [ConfigurationProperty(TypeConverterTypeNamePropertyName, IsRequired = false)]
        public string TypeConverterTypeName
        {
            get { return (string) base[TypeConverterTypeNamePropertyName]; }
            set { base[TypeConverterTypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Create the <see cref="LifetimeManager"/> described by
        /// this element.
        /// </summary>
        /// <returns>A <see cref="LifetimeManager"/> instance.</returns>
        public LifetimeManager CreateLifetimeManager()
        {
            TypeConverter converter = GetTypeConverter();
            if (converter == null)
            {
                return (LifetimeManager) Activator.CreateInstance(GetLifetimeType());
            }
            return (LifetimeManager) converter.ConvertFrom(Value);
        }

        private Type GetLifetimeType()
        {
            return TypeResolver.ResolveTypeWithDefault(TypeName,
                typeof (TransientLifetimeManager));
        }

        private TypeConverter GetTypeConverter()
        {
            if (string.IsNullOrEmpty(Value) && string.IsNullOrEmpty(TypeConverterTypeName))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(TypeConverterTypeName))
            {
                Type converterType = TypeResolver.ResolveType(TypeConverterTypeName);
                return (TypeConverter) Activator.CreateInstance(converterType);
            }
            return TypeDescriptor.GetConverter(GetLifetimeType());
        }
    }
}
