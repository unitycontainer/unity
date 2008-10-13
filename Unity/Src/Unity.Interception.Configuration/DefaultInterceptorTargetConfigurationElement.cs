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
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> class used to manage the contents
    /// of a &lt;defaultInjector&gt; node in the configuration file for the interception extension's section.
    /// </summary>
    public class DefaultInterceptorTargetConfigurationElement : InterceptorTargetConfigurationElementBase
    {
        /// <summary>
        /// Type to which the injector applies.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// The actual <see cref="System.Type"/> object for the 
        /// type this element is registering.
        /// </summary>
        public Type Type
        {
            get { return this.TypeResolver.ResolveType(this.TypeName); }
        }

        internal override InterceptorTargetKey GetKey()
        {
            return new InterceptorTargetKey(string.Empty, this.TypeName, true);
        }

        internal override void DoConfigure(IUnityContainer container, IInstanceInterceptor interceptor)
        {
            container.Configure<Interception>()
                .SetDefaultInterceptorFor(this.Type, interceptor);
        }

        internal override void DoConfigure(IUnityContainer container, ITypeInterceptor interceptor)
        {
            container.Configure<Interception>()
                .SetDefaultInterceptorFor(this.Type, interceptor);
        }
    }
}
