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
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Collection of <see cref="InterceptorTargetConfigurationElementBase"/> elements
    /// from the configuration file.
    /// </summary>
    /// <seealso cref="InterceptorTargetConfigurationElementBase"/>
    /// <seealso cref="KeyInterceptorTargetConfigurationElement"/>
    /// <seealso cref="DefaultInterceptorTargetConfigurationElement"/>
    // FxCop suppression: This is not a normal collection, not going to implement generic interfaces
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class InterceptorTargetConfigurationElementCollection : TypeResolvingConfigurationElementCollection
    {
        ///<summary>
        ///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</summary>
        ///<returns>
        ///A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            throw new InvalidOperationException();
        }

        ///<summary>
        ///Gets the element key for a specified configuration element when overridden in a derived class.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        ///
        ///<param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InterceptorTargetConfigurationElementBase)element).GetKey();
        }

        ///<summary>
        /// Figure out the concrete type to create given the type attribute on the given
        /// xml reader. If not present, throws an exception.
        ///</summary>
        ///
        ///<returns>
        ///true if the unrecognized element was deserialized successfully; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="reader">An input stream that reads XML from the configuration file. </param>
        ///<param name="elementName">The name of the unrecognized element. </param>
        ///<exception cref="T:System.ArgumentException">elementName starts with the reserved prefix "config" or "lock".</exception>
        ///<exception cref="T:System.Configuration.ConfigurationErrorsException">The element specified in elementName is the &lt;clear&gt; element.</exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            InterceptorTargetConfigurationElementBase element = null;

            if (elementName == "default")
            {
                element = new DefaultInterceptorTargetConfigurationElement();
            }
            else if (elementName == "key")
            {
                element = new KeyInterceptorTargetConfigurationElement();
            }

            if (element != null)
            {
                element.DeserializeElement(reader);
                BaseAdd(element);
                return true;
            }

            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}
