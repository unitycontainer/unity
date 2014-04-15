// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="InjectionMember"/> that can be passed to the
    /// <see cref="IUnityContainer.RegisterType"/> method to specify
    /// which interceptor to use. This member sets up the default
    /// interceptor for a type - this will be used regardless of which 
    /// name is used to resolve the type.
    /// </summary>
    public class DefaultInterceptor : InjectionMember
    {
        private readonly IInterceptor interceptor;
        private readonly NamedTypeBuildKey interceptorKey;

        /// <summary>
        /// Construct a new <see cref="DefaultInterceptor"/> instance that,
        /// when applied to a container, will register the given
        /// interceptor as the default one.
        /// </summary>
        /// <param name="interceptor">Interceptor to use.</param>
        public DefaultInterceptor(IInterceptor interceptor)
        {
            Guard.ArgumentNotNull(interceptor, "intereptor");

            this.interceptor = interceptor;
        }

        /// <summary>
        /// Construct a new <see cref="DefaultInterceptor"/> that, when
        /// applied to a container, will register the given type as
        /// the default interceptor. 
        /// </summary>
        /// <param name="interceptorType">Type of interceptor.</param>
        /// <param name="name">Name to use to resolve the interceptor.</param>
        public DefaultInterceptor(Type interceptorType, string name)
        {
            Guard.ArgumentNotNull(interceptorType, "interceptorType");
            Guard.TypeIsAssignable(typeof(IInterceptor), interceptorType, "interceptorType");

            this.interceptorKey = new NamedTypeBuildKey(interceptorType, name);
        }

        /// <summary>
        /// Construct a new <see cref="DefaultInterceptor"/> that, when
        /// applied to a container, will register the given type as
        /// the default interceptor. 
        /// </summary>
        /// <param name="interceptorType">Type of interceptor.</param>
        public DefaultInterceptor(Type interceptorType)
            : this(interceptorType, null)
        {
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Type of interface being registered. If no interface,
        /// this will be null.</param>
        /// <param name="implementationType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            if (this.IsInstanceInterceptor)
            {
                this.AddDefaultInstanceInterceptor(implementationType, policies);
            }
            else
            {
                this.AddDefaultTypeInterceptor(implementationType, policies);
            }
        }

        private bool IsInstanceInterceptor
        {
            get
            {
                if (this.interceptor != null)
                {
                    return this.interceptor is IInstanceInterceptor;
                }
                return typeof(IInstanceInterceptor).IsAssignableFrom(this.interceptorKey.Type);
            }
        }

        private void AddDefaultInstanceInterceptor(Type typeToIntercept, IPolicyList policies)
        {
            IInstanceInterceptionPolicy policy;

            if (this.interceptor != null)
            {
                policy = new FixedInstanceInterceptionPolicy((IInstanceInterceptor)this.interceptor);
            }
            else
            {
                policy = new ResolvedInstanceInterceptionPolicy(this.interceptorKey);
            }

            policies.Set<IInstanceInterceptionPolicy>(policy, typeToIntercept);
        }

        private void AddDefaultTypeInterceptor(Type typeToIntercept, IPolicyList policies)
        {
            ITypeInterceptionPolicy policy;

            if (this.interceptor != null)
            {
                policy = new FixedTypeInterceptionPolicy((ITypeInterceptor)this.interceptor);
            }
            else
            {
                policy = new ResolvedTypeInterceptionPolicy(this.interceptorKey);
            }

            policies.Set<ITypeInterceptionPolicy>(policy, typeToIntercept);
        }
    }

    /// <summary>
    /// A generic version of <see cref="DefaultInterceptor"/> so that
    /// you can specify the interceptor type using generics.
    /// </summary>
    /// <typeparam name="TInterceptor"></typeparam>
    public class DefaultInterceptor<TInterceptor> : DefaultInterceptor
        where TInterceptor : ITypeInterceptor
    {
        /// <summary>
        /// Create a new instance of <see cref="DefaultInterceptor{TInterceptor}"/>.
        /// </summary>
        /// <param name="name">Name to use when resolving interceptor.</param>
        public DefaultInterceptor(string name)
            : base(typeof(TInterceptor), name)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="DefaultInterceptor{TInterceptor}"/>.
        /// </summary>
        public DefaultInterceptor()
            : base(typeof(TInterceptor))
        {
        }
    }
}