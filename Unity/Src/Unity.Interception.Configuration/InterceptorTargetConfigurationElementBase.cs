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
using System.Xml;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Base class for elements in the &lt;transparentProxyInjection&gt; list for the
    /// interception extension configuration.
    /// </summary>
    /// <seealso cref="KeyInterceptorTargetConfigurationElement"/>
    /// <seealso cref="DefaultInterceptorTargetConfigurationElement"/>
    public abstract class InterceptorTargetConfigurationElementBase : TypeResolvingConfigurationElement
    {
        internal abstract InterceptorTargetKey GetKey();

        internal abstract void DoConfigure(IUnityContainer container, IInstanceInterceptor interceptor);

        internal abstract void DoConfigure(IUnityContainer container, ITypeInterceptor interceptor);

        internal void DeserializeElement(XmlReader reader)
        {
            base.DeserializeElement(reader, false);
        }
    }
}