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
using System.Globalization;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element for configuring injection properties.
    /// </summary>
    public class InjectionPropertyElement : InjectionMemberElement
    {
        private InjectionParameterValueElement valueElement;

        /// <summary>
        /// Name of this element - used when calculating the collection key.
        /// </summary>
        public override string ElementName
        {
            get { return "property"; }
        }

        /// <summary>
        /// Type of this property.
        /// </summary>
        [ConfigurationProperty("propertyType", IsRequired = false)]
        public string PropertyTypeName
        {
            get { return (string)this["propertyType"]; }
            set { this["propertyType"] = value; }
        }

        /// <summary>
        /// Generic parameter name of this property.
        /// </summary>
        [ConfigurationProperty("genericParameterName", IsRequired = false)]
        public string GenericParameterName
        {
            get { return (string)this["genericParameterName"]; }
            set { this["genericParameterName"] = value; }
        }

        /// <summary>
        /// Return the InjectionMember object represented by this configuration
        /// element.
        /// </summary>
        /// <returns>The injection member object.</returns>
        public override InjectionMember CreateInjectionMember()
        {
            InjectionParameterValue param 
                = InjectionParameterValueHelper.CreateParameterValue(
                    PropertyTypeName, 
                    GenericParameterName, 
                    valueElement, 
                    TypeResolver);

            return new InjectionProperty(Name, param);
        }

        ///<summary>
        ///Gets a value indicating whether an unknown element is encountered during deserialization.
        ///</summary>
        ///
        ///<returns>
        ///true when an unknown element is encountered while deserializing; otherwise, false.
        ///</returns>
        ///
        ///<param name="reader">The <see cref="T:System.Xml.XmlReader"></see> being used for deserialization.</param>
        ///<param name="elementName">The name of the unknown subelement.</param>
        ///<exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by elementName is locked.- or -One or more of the element's attributes is locked.- or -elementName is unrecognized, or the element has an unrecognized attribute.- or -The element has a Boolean attribute with an invalid value.- or -An attempt was made to deserialize a property more than once.- or -An attempt was made to deserialize a property that is not a valid member of the element.- or -The element cannot contain a CDATA or text element.</exception>
        protected override bool OnDeserializeUnrecognizedElement(
            string elementName, XmlReader reader)
        {
            return InjectionParameterValueHelper.DeserializeUnrecognizedElement(
                elementName,
                reader,
                Name,
                GenericParameterName,
                ref valueElement);
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> that reads from the configuration file.</param>
        /// <param name="serializeCollectionKey"><see langword="true"/> to serialize only the collectionkey properties; 
        /// otherwise, <see langword="false"/>.</param>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            InjectionParameterValueHelper.GuardAttributeOccurrences(reader, "propertyType");

            base.DeserializeElement(reader, serializeCollectionKey);
        }
    }
}
