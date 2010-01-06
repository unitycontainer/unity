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
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element used to specify which extensions to
    /// add to the configuration schema.
    /// </summary>
    public class SectionExtensionElement : DeserializableConfigurationElement
    {
        private const string TypeNamePropertyName = "type";
        private const string PrefixPropertyName = "prefix";

        /// <summary>
        /// Type of the section extender object that will provide new elements to the schema.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true, IsKey = true)]
        public string TypeName
        {
            get { return (string) base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Optional prefix that will be added to the element names added by this
        /// section extender. If left out, no prefix will be added.
        /// </summary>
        [ConfigurationProperty(PrefixPropertyName, IsRequired = false)]
        public string Prefix
        {
            get { return (string) base[PrefixPropertyName]; }
            set { base[PrefixPropertyName] = value; }
        }

        /// <summary>
        /// The extension object represented by this element.
        /// </summary>
        public SectionExtension ExtensionObject
        {
            get; private set;
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

            Type extensionType = TypeResolver.ResolveType(TypeName);
            GuardIsValidExtensionType(extensionType);

            ExtensionObject = (SectionExtension) Activator.CreateInstance(extensionType);
        }

        private void GuardIsValidExtensionType(Type extensionType)
        {
            if(extensionType == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.CurrentUICulture,
                    Resources.ExtensionTypeNotFound, TypeName));
            }

            if(!typeof(SectionExtension).IsAssignableFrom(extensionType))
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.CurrentUICulture,
                    Resources.ExtensionTypeNotValid, TypeName));
            }
        }
    }
}
