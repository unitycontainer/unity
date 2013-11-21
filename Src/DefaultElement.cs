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

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// The &lt;default&gt; element that appears inside an &lt;interceptor&gt; element.
    /// </summary>
    public class DefaultElement : InterceptorRegistrationElement
    {
        internal override string Key
        {
            get { return "default:" + TypeName; }
        }

        internal override string ElementName
        {
            get { return "default"; }
        }

        /// <summary>
        /// Actually register the interceptor against this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="interceptor">interceptor to register.</param>
        internal override void RegisterInterceptor(IUnityContainer container, IInterceptor interceptor)
        {
            var typeInterceptor = interceptor as ITypeInterceptor;
            if(typeInterceptor != null)
            {
                container.Configure<Interception>().SetDefaultInterceptorFor(
                    ResolvedType,
                    typeInterceptor);
            }
            else
            {
                container.Configure<Interception>().SetDefaultInterceptorFor(
                    ResolvedType,
                    (IInstanceInterceptor)interceptor);
            }
        }
    }
}
