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
using System.Globalization;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Properties;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Base class for the default and key elements that can occur
    /// inside the &lt;interceptor&gt; element.
    /// </summary>
    public abstract class InterceptorRegistrationElement : DeserializableConfigurationElement
    {
        private const string TypeNamePropertyName = "type";
        /// <summary>
        /// Type name that this interceptor will be registered for.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true, IsKey = true)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        internal abstract string Key
        {
            get;
        }

        /// <summary>
        /// Actually register the interceptor against this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="interceptor">interceptor to register.</param>
        internal abstract void RegisterInterceptor(IUnityContainer container, IInterceptor interceptor);

        /// <summary>
        /// Return the type object that is resolved from the <see cref="TypeName"/> property.
        /// </summary>
        /// <returns>The type object.</returns>
        protected Type ResolvedType
        {
            get { return TypeResolver.ResolveType(TypeName); }
        }
    }
}
