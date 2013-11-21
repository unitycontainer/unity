// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ObjectBuilder2;
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
        private readonly NamedTypeBuildKey buildKey;

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
        /// Initialize a new instance of the <see cref="Interceptor"/> class with a given
        /// name and type that will be resolved to provide interception.
        /// </summary>
        /// <param name="interceptorType">Type of the interceptor</param>
        /// <param name="name">name to use to resolve.</param>
        public Interceptor(Type interceptorType, string name)
        {
            Guard.ArgumentNotNull(interceptorType, "interceptorType");
            Guard.TypeIsAssignable(typeof(IInterceptor), interceptorType, "interceptorType");

            buildKey = new NamedTypeBuildKey(interceptorType, name);
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Interceptor"/> class with
        /// a given type that will be resolved to provide interception.
        /// </summary>
        /// <param name="interceptorType">Type of the interceptor.</param>
        public Interceptor(Type interceptorType)
            : this(interceptorType, null)
        {
            
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
            var key = new NamedTypeBuildKey(implementationType, name);
            if(IsInstanceInterceptor)
            {
                var policy = CreateInstanceInterceptionPolicy();
                policies.Set<IInstanceInterceptionPolicy>(policy, key);
                policies.Clear<ITypeInterceptionPolicy>(key);
            }
            else
            {
                var policy = CreateTypeInterceptionPolicy();
                policies.Set<ITypeInterceptionPolicy>(policy, key);
                policies.Clear<IInstanceInterceptionPolicy>(key);
            }
        }

        private bool IsInstanceInterceptor
        {
            get
            {
                if(interceptor != null)
                {
                    return interceptor is IInstanceInterceptor;
                }
                return typeof (IInstanceInterceptor).IsAssignableFrom(buildKey.Type);
            }
        }

        private IInstanceInterceptionPolicy CreateInstanceInterceptionPolicy()
        {
            if(interceptor != null)
            {
                return new FixedInstanceInterceptionPolicy((IInstanceInterceptor)interceptor);
            }
            return new ResolvedInstanceInterceptionPolicy(buildKey);
        }

        private ITypeInterceptionPolicy CreateTypeInterceptionPolicy()
        {
            if(interceptor != null)
            {
                return new FixedTypeInterceptionPolicy((ITypeInterceptor) interceptor);
            }
            return new ResolvedTypeInterceptionPolicy(buildKey);
        }
    }

    /// <summary>
    /// Generic version of <see cref="Interceptor"/> that lets you specify an interceptor
    /// type using generic syntax.
    /// </summary>
    /// <typeparam name="TInterceptor">Type of interceptor</typeparam>
    public class Interceptor<TInterceptor> : Interceptor
        where TInterceptor : IInterceptor
    {
        /// <summary>
        /// Initialize an instance of <see cref="Interceptor{TInterceptor}"/> that will
        /// resolve the given interceptor type.
        /// </summary>
        public Interceptor() : base(typeof(TInterceptor))
        {
            
        }

        /// <summary>
        /// Initialize an instance of <see cref="Interceptor{TInterceptor}"/> that will
        /// resolve the given interceptor type and name.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the interceptor.</param>
        public Interceptor(string name) : base(typeof(TInterceptor), name)
        {
            
        }
    }
}
