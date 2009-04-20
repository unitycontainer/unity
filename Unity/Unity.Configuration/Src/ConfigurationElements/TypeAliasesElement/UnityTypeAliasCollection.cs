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
    /// A <see cref="ConfigurationElementCollection"/> that stores the set of
    /// type elements in the config file.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    // FxCop warning suppressed: This is not a normal collection, no need for ICollection<T>
    public class UnityTypeAliasCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Resolve access to a typeAlias element by alias
        /// </summary>
        /// <param name="alias">Name to look up by. This is not just the name element.</param>
        /// <returns>The given element, or null if not in the collection.</returns>
        public new UnityTypeAlias this[string alias]
        {
            get { return (UnityTypeAlias)BaseGet(alias); }
        }

        /// <summary>
        /// Gets access to a type element to look up.
        /// </summary>
        /// <param name="index">The index to retrieve the element from.</param>
        /// <returns>The element.</returns>
        public UnityTypeAlias this[int index]
        {
            get { return (UnityTypeAlias)BaseGet(index); }
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
            return new UnityTypeAlias();
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
            UnityTypeAlias typeAliasElement = (UnityTypeAlias)element;
            return typeAliasElement.Alias;
        }
    }
}
