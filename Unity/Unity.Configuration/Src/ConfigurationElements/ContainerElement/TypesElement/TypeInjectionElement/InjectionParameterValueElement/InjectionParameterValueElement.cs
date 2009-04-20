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
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Base class for config elements that specify how to
    /// create values for a constructor or method to be
    /// injected.
    /// </summary>
    public class InjectionParameterValueElement : TypeResolvingConfigurationElement
    {
        /// <summary>
        /// The concrete type of this element.
        /// </summary>
        [ConfigurationProperty("elementType", IsRequired=false)]
        public string ElementType
        {
            get { return (string)this["elementType"]; }
            set { this["elementType"] = value; }
        }

        /// <summary>
        /// Read the contents of this element from the given <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Source of XML to read from.</param>
        public void DeserializeElement(XmlReader reader)
        {
            base.DeserializeElement(reader, false);
        }

        /// <summary>
        /// Return an instance of <see cref="InjectionParameterValue"/> based
        /// on the contents of this 
        /// </summary>
        /// <param name="targetType">Type of the parameter we are creating the value for.</param>
        /// <returns>The created InjectionParameterValue, ready to pass to the container config API.</returns>
        public virtual InjectionParameterValue CreateParameterValue(Type targetType)
        {
            return null;
        }
    }
}
