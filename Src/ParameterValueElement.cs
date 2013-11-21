// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Base class for configuration elements that describe a value that will
    /// be injected.
    /// </summary>
    public abstract class ParameterValueElement : DeserializableConfigurationElement
    {
        private static int valueElementCount;
        private readonly int valueNumber;

        /// <summary>
        /// Initialize a new instance of <see cref="ParameterValueElement"/>.
        /// </summary>
        protected ParameterValueElement()
        {
            valueNumber = Interlocked.Increment(ref valueElementCount);
        }

        /// <summary>
        /// Return a unique string that can be used to identify this object. Used
        /// by the configuration collection support.
        /// </summary>
        public string Key { get { return string.Format(CultureInfo.InvariantCulture, "value:{0}", valueNumber); } }

        /// <summary>
        /// Generate an <see cref="InjectionParameterValue"/> object
        /// that will be used to configure the container for a type registration.
        /// </summary>
        /// <param name="container">Container that is being configured. Supplied in order
        /// to let custom implementations retrieve services; do not configure the container
        /// directly in this method.</param>
        /// <param name="parameterType">Type of the </param>
        /// <returns></returns>
        public abstract InjectionParameterValue GetInjectionParameterValue(
            IUnityContainer container, Type parameterType);

        /// <summary>
        /// Validate that an expected attribute is present in the given
        /// dictionary and that it has a non-empty value.
        /// </summary>
        /// <param name="propertyValues">Dictionary of name/value pairs to check.</param>
        /// <param name="requiredProperty">attribute name to check.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        protected static void GuardPropertyValueIsPresent(IDictionary<string, string> propertyValues, string requiredProperty)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(propertyValues, "propertyValues");

            if(!propertyValues.ContainsKey(requiredProperty) ||
                string.IsNullOrEmpty(propertyValues[requiredProperty]))
            {
                throw new ConfigurationErrorsException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.RequiredPropertyMissing, requiredProperty));
            }
        }
    }
}
