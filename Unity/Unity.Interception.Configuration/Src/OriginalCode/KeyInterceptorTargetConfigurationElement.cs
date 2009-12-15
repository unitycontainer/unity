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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> class used to manage the contents
    /// of a &lt;injector&gt; node in the configuration file for the interception extension's section.
    /// </summary>
    public class KeyInterceptorTargetConfigurationElement : InterceptorTargetConfigurationElementBase
    {
        /// <summary>
        /// Name part of the key to which the injector must be registered.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = null, IsRequired = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Type part of the key to which the injector must be registered.
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
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public Type Type
        {
            get { return this.TypeResolver.ResolveType(this.TypeName); }
        }

        internal override InterceptorTargetKey GetKey()
        {
            return new InterceptorTargetKey(this.Name, this.TypeName, false);
        }

        internal override void DoConfigure(IUnityContainer container, IInstanceInterceptor interceptor)
        {
            container.Configure<Interception>()
                .SetInterceptorFor(
                    this.Type,
                    string.IsNullOrEmpty(this.Name) ? null : this.Name,
                    interceptor);
        }

        internal override void DoConfigure(IUnityContainer container, ITypeInterceptor interceptor)
        {
            container.Configure<Interception>()
                .SetInterceptorFor(
                    this.Type,
                    string.IsNullOrEmpty(this.Name) ? null : this.Name,
                    interceptor);
        }
    }
}
