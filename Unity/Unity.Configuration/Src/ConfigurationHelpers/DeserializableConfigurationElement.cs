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
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// Base class for configuration elements with a default implementation of
    /// public deserialization.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializable",
        Justification = "It is spelled correctly")]
    public abstract class DeserializableConfigurationElement : ConfigurationElement
    {
        #region IDeserializableElement Members

        /// <summary>
        /// Load this element from the given <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">Contains the XML to initialize from.</param>
        public virtual void Deserialize(XmlReader reader)
        {
            DeserializeElement(reader, false);
        }

        #endregion
    }
}
