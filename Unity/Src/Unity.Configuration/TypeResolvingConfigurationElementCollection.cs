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
using System.Collections;
using System.Configuration;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element collection base class that makes sure
    /// that it's provided elements have a type resolver associated with them.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public abstract class TypeResolvingConfigurationElementCollection : ConfigurationElementCollection, IResolvesTypeAliases, IEnumerable
    {
        private UnityTypeResolver typeResolver;

        /// <summary>
        /// Get or set the type resolver.
        /// </summary>
        public UnityTypeResolver TypeResolver
        {
            get { return typeResolver; }
            set { typeResolver = value; }
        }

        /// <summary>
        /// Gets the configuration element at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the <see cref="ConfigurationElement"/> to return.</param>
        /// <returns>The <see cref="ConfigurationElement"/> at the specified index.</returns>
        /// <exception cref="ConfigurationErrorsException">index is less than 0.
        /// - or -There is no <see cref="ConfigurationElement"/> at the specified index.</exception>
        protected virtual ConfigurationElement Get(int index)
        {
            IResolvesTypeAliases element = (IResolvesTypeAliases)BaseGet(index);
            if(element != null)
            {
                element.TypeResolver = typeResolver;
            }
            return (ConfigurationElement)element;
        }

        /// <summary>
        /// Returns the configuration element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to return.</param>
        /// <returns>The <see cref="ConfigurationElement"/> with the specified key; otherwise, null.</returns>
        protected virtual ConfigurationElement Get(object key)
        {
            IResolvesTypeAliases element = (IResolvesTypeAliases)BaseGet(key);
            if(element != null)
            {
                element.TypeResolver = typeResolver;
            }
            return (ConfigurationElement)element;
        }


        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public new IEnumerator GetEnumerator()
        {
            IEnumerator enumerator = base.GetEnumerator();
            while(enumerator.MoveNext())
            {
                object current = enumerator.Current;
                IResolvesTypeAliases element = current as IResolvesTypeAliases;
                if(element != null)
                {
                    element.TypeResolver = typeResolver;
                }
                yield return current;
            }
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
