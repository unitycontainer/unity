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

using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// An element with a single "name" property, used for
    /// the namespaces and assemblies.
    /// </summary>
    public abstract class NamedElement : DeserializableConfigurationElement
    {
        private const string NamePropertyName = "name";

        /// <summary>
        /// Name attribute for this element.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        public override void SerializeContent(XmlWriter writer)
        {
            Guard.ArgumentNotNull(writer, "writer");
            writer.WriteAttributeString(NamePropertyName, Name);
        }
    }
}
