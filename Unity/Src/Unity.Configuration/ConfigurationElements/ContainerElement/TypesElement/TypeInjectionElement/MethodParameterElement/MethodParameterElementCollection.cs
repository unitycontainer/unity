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
using System.Text;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration collection of parameters for a method or constructor.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class MethodParameterElementCollection : TypeResolvingConfigurationElementCollection
    {
        /// <summary>
        /// Get the element at the given index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Element at index.</returns>
        public MethodParameterElement this[int index]
        {
            get { return (MethodParameterElement)Get(index); }
        }

        /// <summary>
        /// Get the element with the given key.
        /// </summary>
        /// <param name="name">Key to look up</param>
        /// <returns>Element at that key.</returns>
        public new MethodParameterElement this[string name]
        {
            get { return (MethodParameterElement)Get(name); }
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
            return new MethodParameterElement();
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
            MethodParameterElement mpe = (MethodParameterElement)element;
            return mpe.Name;
        }
    }
}
