// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// The &lt;default&gt; element that appears inside an &lt;interceptor&gt; element.
    /// </summary>
    public class DefaultElement : InterceptorRegistrationElement
    {
        internal override string Key
        {
            get { return "default:" + this.TypeName; }
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
            if (typeInterceptor != null)
            {
                container.Configure<Interception>().SetDefaultInterceptorFor(
                    this.ResolvedType,
                    typeInterceptor);
            }
            else
            {
                container.Configure<Interception>().SetDefaultInterceptorFor(
                    this.ResolvedType,
                    (IInstanceInterceptor)interceptor);
            }
        }
    }
}
