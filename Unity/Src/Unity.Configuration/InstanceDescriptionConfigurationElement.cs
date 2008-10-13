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
    /// Base class for configuration elements which describe objects.
    /// </summary>
    public class InstanceDescriptionConfigurationElement : TypeResolvingConfigurationElement
    {
        /// <summary>
        /// Returns the string name of the type of the represented object.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Optional value used when creating the represented object.
        /// </summary>
        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// Type name of the type converter used to create the represented object..
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
        /// the represented object, or null if no typeconverter is
        /// required.
        /// </summary>
        public TypeConverter TypeConverter
        {
            get
            {
                if (string.IsNullOrEmpty(Value) && string.IsNullOrEmpty(TypeConverterName))
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(TypeConverterName))
                {
                    Type converterType = TypeResolver.ResolveType(TypeConverterName);
                    return (TypeConverter)Activator.CreateInstance(converterType);
                }

                return TypeDescriptor.GetConverter(Type);
            }
        }

        /// <summary>
        /// The underlying type of the represented object.
        /// </summary>
        public Type Type
        {
            get { return TypeResolver.ResolveType(TypeName); }
        }

        /// <summary>
        /// Indicates whether the configuration element has information.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the configuration element has information, 
        /// otherwise <see langword="false"/>.
        /// </value>
        public bool HasData
        {
            get
            {
                return !(string.IsNullOrEmpty(this.TypeName) && string.IsNullOrEmpty(this.TypeConverterName));
            }
        }

        /// <summary>
        /// Create the object represented in this section.
        /// </summary>
        /// <returns>The represented object.</returns>
        protected T CreateInstance<T>()
        {
            TypeConverter converter = TypeConverter;
            if (converter == null)
            {
                if (!string.IsNullOrEmpty(TypeName))
                {
                    return (T)Activator.CreateInstance(Type);
                }
                return default(T);
            }
            return (T)converter.ConvertFrom(Value);
        }
    }
}