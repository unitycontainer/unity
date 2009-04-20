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
using System.ComponentModel;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element class describing an interceptor and the keys to which it applies.
    /// </summary>
    public class InterceptorConfigurationElement : InstanceDescriptionConfigurationElement
    {
        /// <summary>
        /// Name to use when registering this instance. Optional.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false, DefaultValue = "")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Collection of configuration elements indicating types and keys for which the represented 
        /// interceptor should be configured.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(
            typeof(InterceptorTargetConfigurationElementCollection),
            AddItemName = "target")]
        public InterceptorTargetConfigurationElementCollection Targets
        {
            get
            {
                InterceptorTargetConfigurationElementCollection targets =
                    (InterceptorTargetConfigurationElementCollection)this[""];
                targets.TypeResolver = TypeResolver;
                return targets;
            }
        }

        internal void Configure(IUnityContainer container)
        {
            IInterceptor interceptor = this.CreateInstance<IInterceptor>();

            IInstanceInterceptor instanceInterceptor = interceptor as IInstanceInterceptor;
            if (instanceInterceptor != null)
            {
                foreach (InterceptorTargetConfigurationElementBase target in Targets)
                {
                    target.DoConfigure(container, instanceInterceptor);
                }
            }
            else
            {
                ITypeInterceptor typeInterceptor = (ITypeInterceptor)interceptor;
                foreach (InterceptorTargetConfigurationElementBase target in Targets)
                {
                    target.DoConfigure(container, typeInterceptor);
                }
            }
        }
    }
}
