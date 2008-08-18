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
    /// A configuration element that gives a value for an instance.
    /// It lets you specify the type, value, and type converter to
    /// use to create the instance.
    /// </summary>
    public class InstanceValueElement : InjectionParameterValueElement
    {
        /// <summary>
        /// Type of the instance to create. If not specified, defaults to string
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = null)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Value to use to initialize the instance.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// The type converter to use to convert the Value into the instance.
        /// If not specified, we use the default converter for the type.
        /// </summary>
        [ConfigurationProperty("typeConverter", IsRequired = false, DefaultValue = null)]
        public string TypeConverterName
        {
            get { return (string)this["typeConverter"]; }
            set { this["typeConverter"] = value; }
        }

        /// <summary>
        /// Create an instance as specified by this element's configuration.
        /// </summary>
        /// <returns>The created instance</returns>
        public object CreateInstance()
        {
            if(TypeToCreate == typeof(string))
            {
                return Value;
            }
            using(new TypeConverterManager(TypeToCreate, TypeConverterName, TypeResolver))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(TypeToCreate);
                return converter.ConvertFromString(Value);
            }
        }

        /// <summary>
        /// Return an instance of <see cref="InjectionParameterValue"/> based
        /// on the contents of this 
        /// </summary>
        /// <param name="targetType">Type of parent parameter. Ignored by this implementation.</param>
        /// <returns>The created InjectionParameterValue, ready to pass to the container config API.</returns>
        public override InjectionParameterValue CreateParameterValue(Type targetType)
        {
            Type typeToCreate;
            if(string.IsNullOrEmpty(TypeName))
            {
                typeToCreate = targetType;
            }
            else
            {
                typeToCreate = TypeResolver.ResolveType(TypeName);
            }
            return new InjectionParameter(typeToCreate, CreateInstance());
        }

        /// <summary>
        /// The <see cref="Type"/> instance after resolution of type aliases.
        /// </summary>
        /// <remarks>If <see cref="TypeName"/> property is empty, this returns typeof(string).</remarks>
        public Type TypeToCreate
        {
            get { return TypeResolver.ResolveWithDefault(TypeName, typeof(string)); }
        }

        private class TypeConverterManager : IDisposable
        {
            private Type targetType;
            private TypeDescriptionProvider descriptionProvider;

            public TypeConverterManager(Type targetType, string converterTypeName, UnityTypeResolver typeResolver)
            {
                this.targetType = targetType;
                if (!string.IsNullOrEmpty(converterTypeName))
                {
                    Type converterType = typeResolver.ResolveType(converterTypeName);
                    descriptionProvider = TypeDescriptor.AddAttributes(targetType,
                        new Attribute[] { new TypeConverterAttribute(converterType) });
                }
            }

            public void Dispose()
            {
                if (descriptionProvider != null)
                {
                    TypeDescriptor.RemoveProvider(descriptionProvider, targetType);
                    descriptionProvider = null;
                }
            }
        }
    }
}
