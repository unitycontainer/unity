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
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="IBuilderStrategy"/> that hooks up type interception. It looks for
    /// a <see cref="ITypeInterceptionPolicy"/> for the current build key, or the current
    /// build type. If present, it substitutes types so that that proxy class gets
    /// built up instead. On the way back, it hooks up the appropriate handlers.
    ///  </summary>
    public class TypeInterceptionStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <remarks>In this class, PreBuildUp is responsible for figuring out if the
        /// class is proxiable, and if so, replacing it with a proxy class.</remarks>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            if (context.Existing != null) return;

            Type typeToBuild = context.BuildKey.Type;

            var interceptionPolicy = FindInterceptionPolicy<ITypeInterceptionPolicy>(context);
            if (interceptionPolicy == null) return;

            var interceptor = interceptionPolicy.GetInterceptor(context);
            if (!interceptor.CanIntercept(typeToBuild)) return;

            var interceptionBehaviorsPolicy = FindInterceptionPolicy<IInterceptionBehaviorsPolicy>(context);

            IEnumerable<IInterceptionBehavior> interceptionBehaviors =
                interceptionBehaviorsPolicy == null ?
                    Enumerable.Empty<IInterceptionBehavior>() :
                    interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                        context, interceptor, typeToBuild, typeToBuild);

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<IAdditionalInterfacesPolicy>(context);

            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            context.Policies.Set<EffectiveInterceptionBehaviorsPolicy>(
                new EffectiveInterceptionBehaviorsPolicy { Behaviors = interceptionBehaviors },
                context.BuildKey);

            Type[] allAdditionalInterfaces =
                Intercept.GetAllAdditionalInterfaces(interceptionBehaviors, additionalInterfaces);

            Type interceptingType =
                interceptor.CreateProxyType(typeToBuild, allAdditionalInterfaces);

            context.Policies.Set<IConstructorSelectorPolicy>(
                new DerivedTypeConstructorSelectorPolicy(
                    interceptingType,
                    context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey)),
                context.BuildKey);
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <remarks>In this class, PostBuildUp checks to see if the object was proxyable,
        /// and if it was, wires up the handlers.</remarks>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            IInterceptingProxy proxy = context.Existing as IInterceptingProxy;
            if (proxy == null) return;

            EffectiveInterceptionBehaviorsPolicy effectiveInterceptionBehaviorsPolicy =
                context.Policies.Get<EffectiveInterceptionBehaviorsPolicy>(context.BuildKey, true);
            if (effectiveInterceptionBehaviorsPolicy == null) return;

            foreach (var interceptionBehavior in effectiveInterceptionBehaviorsPolicy.Behaviors)
            {
                proxy.AddInterceptionBehavior(interceptionBehavior);
            }
        }

        private static TPolicy FindInterceptionPolicy<TPolicy>(IBuilderContext context)
            where TPolicy : class, IBuilderPolicy
        {
            return context.Policies.Get<TPolicy>(context.BuildKey, false) ??
                context.Policies.Get<TPolicy>(context.BuildKey.Type, false);
        }

        private class EffectiveInterceptionBehaviorsPolicy : IBuilderPolicy
        {
            public EffectiveInterceptionBehaviorsPolicy()
            {
                this.Behaviors = new List<IInterceptionBehavior>();
            }

            public IEnumerable<IInterceptionBehavior> Behaviors { get; set; }
        }

        private class DerivedTypeConstructorSelectorPolicy : IConstructorSelectorPolicy
        {
            private Type interceptingType;
            private IConstructorSelectorPolicy originalConstructorSelectorPolicy;

            public DerivedTypeConstructorSelectorPolicy(
                Type interceptingType,
                IConstructorSelectorPolicy originalConstructorSelectorPolicy)
            {
                this.interceptingType = interceptingType;
                this.originalConstructorSelectorPolicy = originalConstructorSelectorPolicy;
            }

            public SelectedConstructor SelectConstructor(IBuilderContext context)
            {
                SelectedConstructor originalConstructor =
                    this.originalConstructorSelectorPolicy.SelectConstructor(context);

                return FindNewConstructor(originalConstructor, interceptingType);
            }

            private static SelectedConstructor FindNewConstructor(SelectedConstructor originalConstructor, Type interceptingType)
            {
                ParameterInfo[] originalParams = originalConstructor.Constructor.GetParameters();

                ConstructorInfo newConstructorInfo =
                    interceptingType.GetConstructor(originalParams.Select(pi => pi.ParameterType).ToArray());

                SelectedConstructor newConstructor = new SelectedConstructor(newConstructorInfo);

                foreach (string key in originalConstructor.GetParameterKeys())
                {
                    newConstructor.AddParameterKey(key);
                }

                return newConstructor;
            }
        }
    }
}
