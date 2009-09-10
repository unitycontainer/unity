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

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Base class for elements that control which members are injected.
    /// </summary>
    public class InjectionMemberElement : TypeResolvingConfigurationElement
    {
        /// <summary>
        /// Name of the element.
        /// </summary>
        [ConfigurationProperty("name")]
        public virtual string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Name of this element - used when calculating the collection key.
        /// </summary>
        public virtual string ElementName
        {
            get { return ""; }
        }

        /// <summary>
        /// Read the contents of the element from the given XmlReader.
        /// </summary>
        /// <param name="reader">The reader for the configuration file.</param>
        public virtual void DeserializeElement(XmlReader reader)
        {
            this.DeserializeElement(reader, false);
        }

        /// <summary>
        /// Return the InjectionMember object represented by this configuration
        /// element.
        /// </summary>
        /// <returns>The injection member object.</returns>
        public virtual InjectionMember CreateInjectionMember()
        {
            return null;
        }

        /// <summary>
        /// Return the key that represents this configuration element in a collection.
        /// </summary>
        /// <returns>The key.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Calculated value")]
        protected internal virtual object GetElementKey()
        {
            return this.ElementName + ":" + this.Name;
        }
    }
}
