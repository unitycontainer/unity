// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using Unity.Configuration.ConfigurationHelpers;
using Unity.InterceptionExtension.Configuration.Properties;

namespace Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A collection of <see cref="InterceptorRegistrationElement"/> objects as shown
    /// in configuration.
    /// </summary>
    [ConfigurationCollection(typeof(InterceptorRegistrationElement))]
    public class InterceptorRegistrationElementCollection : DeserializableConfigurationElementCollectionBase<InterceptorRegistrationElement>
    {
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            throw new InvalidOperationException(Resources.CannotCreateInterceptorRegistrationElement);
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InterceptorRegistrationElement)element).Key;
        }
    }
}
