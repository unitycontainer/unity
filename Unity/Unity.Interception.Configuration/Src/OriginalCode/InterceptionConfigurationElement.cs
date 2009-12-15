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
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element for the <see cref="Interception"/> container extension.
    /// </summary>
    public class InterceptionConfigurationElement : UnityContainerExtensionConfigurationElement
    {
        /// <summary>
        /// Configures policies and injectors using the <see cref="Interception"/> container extension.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        public override void Configure(IUnityContainer container)
        {
            base.Configure(container);

            foreach (InterceptionPolicyConfigurationElement element in Policies)
            {
                element.Configure(container);
            }

            foreach (InterceptorConfigurationElement element in Interceptors)
            {
                element.Configure(container);
            }
        }

        /// <summary>
        /// Collection of policies.
        /// </summary>
        /// <remarks>
        /// Defining policies in this section is equivalent to defining them in using the general purpose
        /// injection configuration, only more convenient.
        /// </remarks>
        [ConfigurationProperty("policies")]
        [ConfigurationCollection(
            typeof(InterceptionPolicyConfigurationElementCollection), 
            AddItemName = "policy")]
        public InterceptionPolicyConfigurationElementCollection Policies
        {
            get
            {
                InterceptionPolicyConfigurationElementCollection policies =
                    (InterceptionPolicyConfigurationElementCollection)this["policies"];
                policies.TypeResolver = TypeResolver;
                return policies;
            }
        }

        /// <summary>
        /// Collection of configuration elements indicating types and keys for which transparent-proxy-based
        /// interception should be performed.
        /// </summary>
        [ConfigurationProperty("interceptors")]
        [ConfigurationCollection(
            typeof(InterceptorConfigurationElementCollection), 
            AddItemName = "interceptor")]
        public InterceptorConfigurationElementCollection Interceptors
        {
            get
            {
                InterceptorConfigurationElementCollection interceptors =
                    (InterceptorConfigurationElementCollection)this["interceptors"];
                interceptors.TypeResolver = TypeResolver;
                return interceptors;
            }
        }
    }
}
