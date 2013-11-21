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
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A collection of <see cref="ParameterValueElement"/> objects.
    /// </summary>
    public class ParameterValueElementCollection : DeserializableConfigurationElementCollectionBase<ParameterValueElement>
    {
        private class DeserializedElementHolder : IValueProvidingElement
        {
            private ParameterValueElement ValueElement { get; set; }

            /// <summary>
            /// String that will be deserialized to provide the value.
            /// </summary>
            public ParameterValueElement Value
            {
                get { return ValueElement; }
                set { ValueElement = value; }
            }

            /// <summary>
            /// A string describing where the value this element contains
            /// is being used. For example, if setting a property Prop1,
            /// this should return "property Prop1" (in english).
            /// </summary>
            public string DestinationName
            {
                get { throw new NotImplementedException(); }
            }
        }

        /// <summary>
        /// Causes the configuration system to throw an exception.
        /// </summary>
        /// <returns>
        /// true if the unrecognized element was deserialized successfully; otherwise, false. The default is false.
        /// </returns>
        /// <param name="elementName">The name of the unrecognized element. 
        ///                 </param><param name="reader">An input stream that reads XML from the configuration file. 
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element specified in <paramref name="elementName"/> is the &lt;clear&gt; element.
        ///                 </exception><exception cref="T:System.ArgumentException"><paramref name="elementName"/> starts with the reserved prefix "config" or "lock".
        ///                 </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            var holder = new DeserializedElementHolder();
            var helper = new ValueElementHelper(holder);

            if(helper.DeserializeUnknownElement(elementName, reader))
            {
                Add(holder.Value);
                return true;
            }

            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }


        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            throw new InvalidOperationException(Resources.CannotCreateParameterValueElement);
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. 
        ///                 </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterValueElement) element).Key;
        }
    }
}
