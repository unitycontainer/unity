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
using System.Globalization;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Stores information about the <see cref="IInterceptor"/> to be used to intercept an object and
    /// configures a container accordingly.
    /// </summary>
    /// <seealso cref="IInterceptionBehavior"/>
    public class Interceptor : InterceptionMember
    {
        private readonly IInterceptor interceptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interceptor"/> class with an interceptor instance.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptor"/> to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is
        /// <see langword="null"/>.</exception>
        public Interceptor(IInterceptor interceptor)
        {
            Guard.ArgumentNotNull(interceptor, "interceptor");

            this.interceptor = interceptor;
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the container to use the represented 
        /// <see cref="IInterceptor"/> for the supplied parameters.
        /// </summary>
        /// <param name="serviceType">Interface being registered.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            NamedTypeBuildKey key = new NamedTypeBuildKey(implementationType, name);

            IInstanceInterceptor instanceInterceptor = this.interceptor as IInstanceInterceptor;
            if (instanceInterceptor != null)
            {
                InstanceInterceptionPolicy policy = new InstanceInterceptionPolicy(instanceInterceptor);
                policies.Set<IInstanceInterceptionPolicy>(policy, key);
            }
            else
            {
                ITypeInterceptionPolicy policy = new TypeInterceptionPolicy((ITypeInterceptor)this.interceptor);
                policies.Set<ITypeInterceptionPolicy>(policy, key);
            }
        }
    }
}
