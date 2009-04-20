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

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElementCollection"/> that holds the collection of
    /// extension elements specified in the config file.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    // FxCop warning suppressed: This is not a normal collection, no need for ICollection<T>
    public class UnityContainerExtensionElementCollection : TypeResolvingConfigurationElementCollection
    {
        /// <summary>
        /// Resolve or set the extension element stored at the given index.
        /// </summary>
        /// <param name="index">Index to get or set the element at.</param>
        /// <returns>The extension element.</returns>
        public UnityContainerExtensionElement this[int index]
        {
            get { return (UnityContainerExtensionElement)Get(index); }
            set
            {
                if(BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        ///<summary>
        ///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        ///
        protected override ConfigurationElement CreateNewElement()
        {
            return new UnityContainerExtensionElement();
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
            UnityContainerExtensionElement extensionElement =
                (UnityContainerExtensionElement)element;

            return extensionElement.TypeName;
        }
    }
}
