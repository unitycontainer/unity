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
using Microsoft.Practices.Unity.ObjectBuilder;
using Guard = Microsoft.Practices.Unity.Utility.Guard;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A Unity container extension that allows you to configure
    /// whether an object should be intercepted and which mechanism should
    /// be used to do it, and also provides a convenient set of methods for
    /// configuring injection for <see cref="RuleDrivenPolicy"/> instances.
    /// </summary>
    /// <seealso cref="Interception.SetDefaultInterceptorFor(Type, IInstanceInterceptor)"/>
    /// <seealso cref="Interception.SetDefaultInterceptorFor(Type, ITypeInterceptor)"/>
    /// <seealso cref="Interception.SetInterceptorFor(Type, string, IInstanceInterceptor)"/>
    /// <seealso cref="Interception.SetInterceptorFor(Type, string, ITypeInterceptor)"/>
    /// <seealso cref="Interception.AddPolicy"/>
    public class Interception : UnityContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        protected override void Initialize()
        {
            // The TransparentProxyInterceptionStrategy is added to the Setup (first) stage.
            // This means that instances will be intercepted after type mapping and lifetime management
            // have taken place.
            Context.Strategies.AddNew<InstanceInterceptionStrategy>(UnityBuildStage.Setup);
            Context.Strategies.AddNew<TypeInterceptionStrategy>(UnityBuildStage.PreCreation);
            Context.Container
                .RegisterInstance<InjectionPolicy>(
                    typeof(AttributeDrivenPolicy).AssemblyQualifiedName,
                    new AttributeDrivenPolicy());
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, string name, ITypeInterceptor interceptor)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            GuardTypeInterceptable(typeToIntercept, interceptor);

            var key = new NamedTypeBuildKey(typeToIntercept, name);

            var policy = new FixedTypeInterceptionPolicy(interceptor);
            Context.Policies.Set<ITypeInterceptionPolicy>(policy, key);

            // add policy injection behavior if using this configuration API to set the interceptor
            var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            Context.Policies.Set<IInterceptionBehaviorsPolicy>(interceptionBehaviorsPolicy, key);

            return this;
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="interceptor">Interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, ITypeInterceptor interceptor)
        {
            return SetInterceptorFor(typeToIntercept, null, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept</typeparam>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Interceptor object to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(string name, ITypeInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), name, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept</typeparam>
        /// <param name="interceptor">Interceptor object to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(ITypeInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), null, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, string name, IInstanceInterceptor interceptor)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            GuardTypeInterceptable(typeToIntercept, interceptor);

            var key = new NamedTypeBuildKey(typeToIntercept, name);

            var policy = new FixedInstanceInterceptionPolicy(interceptor);
            Context.Policies.Set<IInstanceInterceptionPolicy>(policy, key);

            // add policy injection behavior if using this configuration API to set the interceptor
            var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            Context.Policies.Set<IInterceptionBehaviorsPolicy>(interceptionBehaviorsPolicy, key);

            return this;
        }

        /// <summary>
        /// Set the interceptor for a type, regardless of what name is used to resolve the instances.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept</param>
        /// <param name="interceptor">Interceptor instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor(Type typeToIntercept, ITypeInterceptor interceptor)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            GuardTypeInterceptable(typeToIntercept, interceptor);

            Context.Policies.Set<ITypeInterceptionPolicy>(new FixedTypeInterceptionPolicy(interceptor), typeToIntercept);

            // add policy injection behavior if using this configuration API to set the interceptor
            var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            Context.Policies.Set<IInterceptionBehaviorsPolicy>(interceptionBehaviorsPolicy, typeToIntercept);

            return this;
        }

        /// <summary>
        /// Set the interceptor for a type, regardless of what name is used to resolve the instances.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type to intercept</typeparam>
        /// <param name="interceptor">Interceptor instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor<TTypeToIntercept>(ITypeInterceptor interceptor)
        {
            return SetDefaultInterceptorFor(typeof(TTypeToIntercept), interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, IInstanceInterceptor interceptor)
        {
            return SetInterceptorFor(typeToIntercept, null, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept.</typeparam>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(string name, IInstanceInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), name, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept.</typeparam>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(IInstanceInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), null, interceptor);
        }

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type the interception is being configured for.</param>
        /// <param name="interceptor">The interceptor to use by default.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor(Type typeToIntercept, IInstanceInterceptor interceptor)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            GuardTypeInterceptable(typeToIntercept, interceptor);

            Context.Policies.Set<IInstanceInterceptionPolicy>(new FixedInstanceInterceptionPolicy(interceptor), typeToIntercept);

            // add policy injection behavior if using this configuration API to set the interceptor
            var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            Context.Policies.Set<IInterceptionBehaviorsPolicy>(interceptionBehaviorsPolicy, typeToIntercept);

            return this;
        }

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type the interception is being configured for.</typeparam>
        /// <param name="interceptor">The interceptor to use by default.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor<TTypeToIntercept>(IInstanceInterceptor interceptor)
        {
            return SetDefaultInterceptorFor(typeof(TTypeToIntercept), interceptor);
        }

        private static void GuardTypeInterceptable(Type typeToIntercept, IInterceptor interceptor)
        {
            if (!interceptor.CanIntercept(typeToIntercept))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.InterceptionNotSupported,
                        typeToIntercept.FullName),
                    "typeToIntercept");
            }
        }

        /// <summary>
        /// Starts the definition of a new <see cref="RuleDrivenPolicy"/>.
        /// </summary>
        /// <param name="policyName">The policy name.</param>
        /// <returns></returns>
        /// <remarks>This is a convenient way for defining a new policy and the <see cref="IMatchingRule"/>
        /// instances and <see cref="ICallHandler"/> instances that are required by a policy.
        /// <para/>
        /// This mechanism is just a shortcut for what can be natively expressed by wiring up together objects
        /// with repeated calls to the <see cref="IUnityContainer.RegisterType"/> method.
        /// </remarks>
        public PolicyDefinition AddPolicy(string policyName)
        {
            Guard.ArgumentNotNullOrEmpty(policyName, "policyName");
            return new PolicyDefinition(policyName, this);
        }
    }
}
