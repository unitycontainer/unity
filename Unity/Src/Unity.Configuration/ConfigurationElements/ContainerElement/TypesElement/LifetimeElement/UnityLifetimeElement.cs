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

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element class wrapping the lifetime element
    /// inside a type element.
    /// </summary>
    public class UnityLifetimeElement : TypeResolvingConfigurationElement
    {
        /// <summary>
        /// Returns the string name of the type of lifetime manager.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Optional value used when creating the lifetime manager
        /// </summary>
        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// Type name of the type converter used to create the lifetime manager.
        /// If not specified, the default type converter (if any) is used.
        /// </summary>
        [ConfigurationProperty("typeConverter")]
        public string TypeConverterName
        {
            get { return (string)this["typeConverter"]; }
            set { this["typeConverter"] = value; }
        }

        /// <summary>
        /// The type converter to use to convert the value into
        /// a lifetime manager, or null if no typeconverter is
        /// required.
        /// </summary>
        public TypeConverter TypeConverter
        {
            get
            {
                if(string.IsNullOrEmpty(Value) && string.IsNullOrEmpty(TypeConverterName))
                {
                    return null;
                }

                if(!string.IsNullOrEmpty(TypeConverterName))
                {
                    Type converterType = TypeResolver.ResolveType(TypeConverterName);
                    return (TypeConverter)Activator.CreateInstance(converterType);
                }

                return TypeDescriptor.GetConverter(Type);
            }
        }

        /// <summary>
        /// The underlying type of lifetime manager.
        /// </summary>
        public Type Type
        {
            get { return TypeResolver.ResolveType(TypeName); }
        }
        /// <summary>
        /// Create the lifetime manager instance configured in this section.
        /// </summary>
        /// <returns>The lifetime manager configured.</returns>
        public LifetimeManager CreateLifetimeManager()
        {
            TypeConverter converter = TypeConverter;
            if (converter == null)
            {
                if (!string.IsNullOrEmpty(TypeName))
                {
                    return (LifetimeManager) Activator.CreateInstance(Type);
                }
                return null;
            }
            return (LifetimeManager)converter.ConvertFrom(Value);
        }
    }
}
