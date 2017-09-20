// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using Unity.Configuration.ConfigurationHelpers;

namespace Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A collection of <see cref="InterceptorsInterceptorElement"/> objects
    /// as stored in configuration.
    /// </summary>
    [ConfigurationCollection(typeof(InterceptorsInterceptorElement), AddItemName = "interceptor")]
    public class InterceptorsInterceptorElementCollection :
        DeserializableConfigurationElementCollection<InterceptorsInterceptorElement>
    {
        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InterceptorsInterceptorElement)element).TypeName;
        }
    }
}
