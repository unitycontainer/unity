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
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A Unity container extension that allows you to configure
    /// whether an object should be intercepted and which mechanism should
    /// be used to do it, and also provides a convenient set of methods for
    /// configuring injection for <see cref="RuleDrivenPolicy"/> instances.
    /// </summary>
    /// <seealso cref="O:Interception.SetDefaultInjectorFor"/>
    /// <seealso cref="O:Interception.SetInjectorFor"/>
    /// <seealso cref="Interception.AddPolicy"/>
    public class Interception : UnityContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// This extension adds the <see cref="TransparentProxyInterceptionStrategy"/> to the
        /// <see cref="UnityBuildStage.Setup"/> stage.
        /// </remarks>
        protected override void Initialize()
        {
            // The TransparentProxyInterceptionStrategy is added to the Setup (first) stage.
            // This means that instances will be intercepted after type mapping and lifetime management
            // have taken place.
            Context.Strategies
                .AddNew<TransparentProxyInterceptionStrategy>(UnityBuildStage.Setup);
            Context.Container
                .RegisterInstance<InjectionPolicy>(
                    typeof(AttributeDrivenPolicy).AssemblyQualifiedName,
                    new AttributeDrivenPolicy());
        }

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type the interception is being configured for.</typeparam>
        /// <param name="policyInjector">The injector to use by default.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInjectorFor<TTypeToIntercept>(PolicyInjector policyInjector)
        {
            return SetDefaultInjectorFor(typeof(TTypeToIntercept), policyInjector);
        }

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type the interception is being configured for.</param>
        /// <param name="policyInjector">The injector to use by default.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInjectorFor(Type typeToIntercept, PolicyInjector policyInjector)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(policyInjector, "policyInjector");
            GuardTypeInterceptable(typeToIntercept, policyInjector);

            this.Context.Policies
                .Set<IInterceptionPolicy>(new InterceptionPolicy(policyInjector), typeToIntercept);

            return this;
        }

        /// <summary>
        /// API to configure interception settings for an unnamed instance of a particular type.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type the interception is being configured for.</typeparam>
        /// <param name="policyInjector">The injector to use when intercepting the instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInjectorFor<TTypeToIntercept>(PolicyInjector policyInjector)
        {
            return SetInjectorFor<TTypeToIntercept>(null, policyInjector);
        }

        /// <summary>
        /// API to configure interception settings for a named instance of a particular type.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type the interception is being configured for.</typeparam>
        /// <param name="name">Name of the instance to configure interception for.</param>
        /// <param name="policyInjector">The injector to use when intercepting the instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInjectorFor<TTypeToIntercept>(string name, PolicyInjector policyInjector)
        {
            return SetInjectorFor(typeof(TTypeToIntercept), name, policyInjector);
        }

        /// <summary>
        /// API to configure interception settings for an unnamed instance of a particular type.
        /// </summary>
        /// <param name="typeToIntercept">Type the interception is being configured for.</param>
        /// <param name="policyInjector">The injector to use when intercepting the instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInjectorFor(Type typeToIntercept, PolicyInjector policyInjector)
        {
            return SetInjectorFor(typeToIntercept, null, policyInjector);
        }

        /// <summary>
        /// API to configure interception settings for a named instance of a particular type.
        /// </summary>
        /// <param name="typeToIntercept">Type the interception is being configured for.</param>
        /// <param name="name">Name of the instance to configure interception for.</param>
        /// <param name="policyInjector">The injector to use when intercepting the instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInjectorFor(Type typeToIntercept, string name, PolicyInjector policyInjector)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(policyInjector, "policyInjector");
            GuardTypeInterceptable(typeToIntercept, policyInjector);

            this.Context.Policies
                .Set<IInterceptionPolicy>(
                    new InterceptionPolicy(policyInjector),
                    new Microsoft.Practices.ObjectBuilder2.NamedTypeBuildKey(typeToIntercept, name));

            return this;
        }

        private static void GuardTypeInterceptable(Type typeToIntercept, PolicyInjector policyInjector)
        {
            if (!policyInjector.TypeSupportsInterception(typeToIntercept))
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
        /// <param name="policyName"></param>
        /// <returns></returns>
        /// <remarks>This is a convenient way for defining a new policy and the <see cref="IMatchingRule"/>
        /// instances and <see cref="ICallHandler"/> instances that are required by a policy.
        /// <para/>
        /// This mechanism is just a shortcut for what can be natively expressed by wiring up together objects
        /// with repeated calls to the <see cref="O:InjectedMembers.ConfigureInjectionFor"/> method.</remarks>
        public PolicyDefinition AddPolicy(string policyName)
        {
            Guard.ArgumentNotNullOrEmpty(policyName, "policyName");
            return new PolicyDefinition(policyName, this);
        }
    }
}