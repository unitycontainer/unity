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
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element for configuring constructor injection.
    /// </summary>
    public class InjectionConstructorElement : InjectionMemberElement
    {
        /// <summary>
        /// Name of the element.
        /// </summary>
        public override string Name
        {
            get { return ""; }
            set {  }
        }


        /// <summary>
        /// Name of this element - used when calculating the collection key.
        /// </summary>
        public override string ElementName
        {
            get { return "constructor"; }
        }

        /// <summary>
        /// The collection of <see cref="InjectionParameterValueElement"/> elements
        /// that are children of this node.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection=true)]
        [ConfigurationCollection(typeof(MethodParameterElementCollection), AddItemName = "param")]
        public MethodParameterElementCollection Parameters
        {
            get
            {
                MethodParameterElementCollection parameters =
                    (MethodParameterElementCollection)this[""];
                parameters.TypeResolver = TypeResolver;
                return parameters;
            }
        }
        /// <summary>
        /// Return the InjectionMember object represented by this configuration
        /// element.
        /// </summary>
        /// <returns>The injection member object.</returns>
        public override InjectionMember CreateInjectionMember()
        {
            InjectionParameterValue[] values = new InjectionParameterValue[Parameters.Count];
            int i = 0;
            foreach(MethodParameterElement element in Parameters)
            {
                values[i++] = element.CreateInjectionParameterValue();
            }
            return new InjectionConstructor(values);
        }
    }
}
