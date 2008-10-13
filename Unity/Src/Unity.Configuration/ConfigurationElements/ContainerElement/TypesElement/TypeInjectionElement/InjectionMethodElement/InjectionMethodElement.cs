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

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element for configuring method injection.
    /// </summary>
    public class InjectionMethodElement : InjectionMemberElement
    {
        /// <summary>
        /// Name of this element - used when calculating the collection key.
        /// </summary>
        public override string ElementName
        {
            get { return "method"; }
        }

        /// <summary>
        /// Key of the element.
        /// </summary>
        /// <remarks>
        /// This property is used to allow configuration for multiple calls on the same method.
        /// </remarks>
        [ConfigurationProperty("key", IsRequired = false, DefaultValue = "")]
        public virtual string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        /// <summary>
        /// The collection of <see cref="InjectionParameterValueElement"/> elements
        /// that are children of this node.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
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
            foreach (MethodParameterElement element in Parameters)
            {
                values[i++] = element.CreateInjectionParameterValue();
            }
            return new InjectionMethod(Name, values);
        }

        /// <summary>
        /// Return the key that represents this configuration element in a collection.
        /// </summary>
        /// <returns>The key.</returns>
        protected internal override object GetElementKey()
        {
            return base.GetElementKey() + ":" + Key;
        }
    }
}
