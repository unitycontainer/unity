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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Configuration.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element representing a parameter passed to a constructor
    /// or a method.
    /// </summary>
    public class ParameterElement : DeserializableConfigurationElement, IValueProvidingElement
    {
        private const string NamePropertyName = "name";
        private const string TypeNamePropertyName = "type";

        private ParameterValueElement valueElement;
        private readonly ValueElementHelper valueElementHelper;

        /// <summary>
        /// Construct a new instance of <see cref="ParameterElement"/>.
        /// </summary>
        public ParameterElement()
        {
            valueElementHelper = new ValueElementHelper(this);
        }

        /// <summary>
        /// Name of this parameter.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Type of this parameter.
        /// </summary>
        /// <remarks>This is only needed in order to disambiguate method overloads. Normally
        /// the parameter name is sufficient.</remarks>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = false)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Element that describes the value for this property.
        /// </summary>
        /// <remarks>
        /// This is NOT marked as a ConfigurationProperty because this
        /// child element is polymorphic, and the element tag determines
        /// the type. Standard configuration properties only let you do
        /// this if it's a collection, but we only want one value. Thus
        /// the separate property. The element is deserialized in 
        /// <see cref="OnDeserializeUnrecognizedElement"/>.</remarks>
        public ParameterValueElement Value
        {
            get { return ValueElementHelper.GetValue(valueElement); }
            set { valueElement = value; }
        }

        ParameterValueElement IValueProvidingElement.Value
        {
            get { return this.valueElement; }
            set { this.Value = value; }
        }

        /// <summary>
        /// A string describing where the value this element contains
        /// is being used. For example, if setting a property Prop1,
        /// this should return "property Prop1" (in english).
        /// </summary>
        public string DestinationName
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture,
                    Resources.DestinationNameFormat,
                    Resources.Parameter, Name);
            }
        }

        /// <summary>
        /// Returns the required <see cref="InjectionParameterValue"/> needed
        /// to configure the container so that the correct value is injected.
        /// </summary>
        /// <param name="container">Container being configured.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>The value to use to configure the container.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public InjectionParameterValue GetParameterValue(IUnityContainer container, Type parameterType)
        {
            Guard.ArgumentNotNull(parameterType, "parameterType");

            Type requiredType = parameterType;
            if (!string.IsNullOrEmpty(TypeName))
            {
                Debug.Assert(
                    !parameterType.IsGenericParameter,
                    "parameterType shouldn't be a generic parameter if TypeName is not null, because the " +
                    "TypeName is used to match the method parameter when present.");

                requiredType = TypeResolver.ResolveType(TypeName);
            }
            return Value.GetInjectionParameterValue(container, requiredType);
        }

        /// <summary>
        /// Does the information in this <see cref="ParameterElement"/> match
        /// up with the given <paramref name="parameterInfo"/>?
        /// </summary>
        /// <param name="parameterInfo">Information about the parameter.</param>
        /// <returns>True if this is a match, false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public bool Matches(ParameterInfo parameterInfo)
        {
            Guard.ArgumentNotNull(parameterInfo, "parameterInfo");
            if (Name != parameterInfo.Name) return false;
            if (!string.IsNullOrEmpty(TypeName))
            {
                Type parameterElementType = TypeResolver.ResolveType(TypeName);
                return parameterElementType == parameterInfo.ParameterType;
            }
            return true;
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> that reads from the configuration file.
        ///                 </param><param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element to read is locked.
        ///                     - or -
        ///                     An attribute of the current node is not recognized.
        ///                     - or -
        ///                     The lock status of the current node cannot be determined.  
        ///                 </exception>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
            valueElementHelper.CompleteValueElement(reader);
        }

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown attribute is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="name">The name of the unrecognized attribute.
        ///                 </param><param name="value">The value of the unrecognized attribute.
        ///                 </param>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            // Noop - we can read this for back compat, but it's now ignored.
            if (name == "genericParameterName")
                return true;
            return valueElementHelper.DeserializeUnrecognizedAttribute(name, value);
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="elementName">The name of the unknown subelement.
        ///                 </param><param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.
        ///                     - or -
        ///                     One or more of the element's attributes is locked.
        ///                     - or -
        ///                 <paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.
        ///                     - or -
        ///                     The element has a Boolean attribute with an invalid value.
        ///                     - or -
        ///                     An attempt was made to deserialize a property more than once.
        ///                     - or -
        ///                     An attempt was made to deserialize a property that is not a valid member of the element.
        ///                     - or -
        ///                     The element cannot contain a CDATA or text element.
        ///                 </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return valueElementHelper.DeserializeUnknownElement(elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
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
            writer.WriteAttributeString(NamePropertyName, Name);
            writer.WriteAttributeIfNotEmpty(TypeNamePropertyName, TypeName);
            ValueElementHelper.SerializeParameterValueElement(writer, Value, false);
        }
    }
}
