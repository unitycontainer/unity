// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Xml;
using Unity.Configuration.ConfigurationHelpers;
using Unity.Interception.Configuration.Properties;

namespace Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element that represents the configuration for
    /// a specific interceptor, as presented in the config file inside
    /// the &lt;interceptors&gt; element.
    /// </summary>
    public class InterceptorsInterceptorElement : DeserializableConfigurationElement
    {
        private const string TypeNamePropertyName = "type";
        private const string RegistrationsPropertyName = "registrations";
        private const string ValuePropertyName = "value";
        private const string TypeConverterTypeNamePropertyName = "typeConverter";

        private static readonly UnknownElementHandlerMap<InterceptorsInterceptorElement> UnknownElementHandlerMap =
            new UnknownElementHandlerMap<InterceptorsInterceptorElement>
            {
                { "default", (iie, xr) => iie.ReadElementByType(xr, typeof(DefaultElement), iie.Registrations) },
                { "key", (iie, xr) => iie.ReadElementByType(xr, typeof(KeyElement), iie.Registrations) }
            };

        /// <summary>
        /// Type of interceptor to configure.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true, IsKey = true)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// The types that this interceptor will be registered against.
        /// </summary>
        [ConfigurationProperty(RegistrationsPropertyName)]
        public InterceptorRegistrationElementCollection Registrations
        {
            get { return (InterceptorRegistrationElementCollection)base[RegistrationsPropertyName]; }
        }

        /// <summary>
        /// Any value passed to the type converter.
        /// </summary>
        [ConfigurationProperty(ValuePropertyName, IsRequired = false)]
        public string Value
        {
            get { return (string)base[ValuePropertyName]; }
            set { base[ValuePropertyName] = value; }
        }

        /// <summary>
        /// Type converter to use to create the interceptor, if any.
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void SerializeContent(XmlWriter writer)
        {
            Unity.Utility.Guard.ArgumentNotNull(writer, "writer");

            writer.WriteAttributeString(TypeNamePropertyName, this.TypeName);
            writer.WriteAttributeIfNotEmpty(ValuePropertyName, this.Value);
            writer.WriteAttributeIfNotEmpty(TypeConverterTypeNamePropertyName, this.TypeConverterTypeName);

            foreach (var registration in this.Registrations)
            {
                writer.WriteElement(registration.ElementName, registration.SerializeContent);
            }
        }

        internal void ConfigureContainer(IUnityContainer container)
        {
            var interceptor = this.CreateInterceptor();

            foreach (var registration in this.Registrations)
            {
                registration.RegisterInterceptor(container, interceptor);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="elementName">The name of the unknown subelement.</param>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.</param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.
        /// - or -
        /// One or more of the element's attributes is locked.
        /// - or -
        /// <paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.
        /// - or -
        /// The element has a Boolean attribute with an invalid value.
        /// - or -
        /// An attempt was made to deserialize a property more than once.
        /// - or -
        /// An attempt was made to deserialize a property that is not a valid member of the element.
        /// - or -
        /// The element cannot contain a CDATA or text element.
        /// </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return UnknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
            base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        private void GuardIsValidInterceptorType(Type type)
        {
            this.GuardTypesCompatible<IInterceptor>(type);
        }

        private void GuardIsValidTypeConverterType(Type type)
        {
            this.GuardTypesCompatible<TypeConverter>(type);
        }

        private void GuardTypesCompatible<TTargetType>(Type type)
        {
            if (!typeof(TTargetType).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.ExceptionResolvedTypeNotCompatible,
                    this.TypeName, type.FullName, typeof(TTargetType).FullName));
            }
        }

        private IInterceptor CreateInterceptor()
        {
            if (!string.IsNullOrEmpty(this.TypeConverterTypeName))
            {
                return this.CreateInterceptorWithTypeConverter();
            }
            return this.CreateInterceptorWithNew();
        }

        private IInterceptor CreateInterceptorWithNew()
        {
            Type interceptorType = TypeResolver.ResolveType(this.TypeName);
            this.GuardIsValidInterceptorType(interceptorType);

            return (IInterceptor)Activator.CreateInstance(interceptorType);
        }

        private IInterceptor CreateInterceptorWithTypeConverter()
        {
            Type converterType = TypeResolver.ResolveType(this.TypeConverterTypeName);
            this.GuardIsValidTypeConverterType(converterType);

            var converter = (TypeConverter)Activator.CreateInstance(converterType);
            return (IInterceptor)converter.ConvertFromInvariantString(this.Value);
        }
    }
}
