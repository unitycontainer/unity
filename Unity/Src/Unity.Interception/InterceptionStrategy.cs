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
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that will add interception to an object.
    /// </summary>
    /// <remarks>
    /// Interception will be based on the <see cref="InjectionPolicy"/> instances available in the 
    /// <see cref="IUnityContainer"/> to which the strategy belongs and will be performed by
    /// the <see cref="PolicyInjector"/> associated to the <see cref="IInterceptionPolicy"/>
    /// associated to the object being built.
    /// <para/>
    /// The strategy will use the type in <see cref="IBuilderContext.OriginalBuildKey"/>
    /// to determine how to perform interception.
    /// <para/>
    /// This strategy is added by the <see cref="Interception"/> container extension.
    /// </remarks>
    /// <seealso cref="Interception"/>
    /// <seealso cref="IInterceptionPolicy"/>
    /// <seealso cref="InjectionPolicy"/>
    /// <seealso cref="PolicyInjector"/>
    public class InterceptionStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp(IBuilderContext context)
        {
            Type originalType;
            if (BuildKey.TryGetType(context.OriginalBuildKey, out originalType))
            {
                // avoid getting the default policy when looking for policies
                IInterceptionPolicy interceptionPolicy
                    = context.Policies.GetNoDefault<IInterceptionPolicy>(context.BuildKey, false);

                // fallback to the build key's type default configuration
                if (interceptionPolicy == null)
                {
                    Type type;
                    if (BuildKey.TryGetType(context.BuildKey, out type))
                    {
                        interceptionPolicy
                            = context.Policies.GetNoDefault<IInterceptionPolicy>(type, false);
                    }
                }

                // fallback to the original build key's configuration
                if (interceptionPolicy == null)
                {
                    interceptionPolicy
                        = context.Policies.GetNoDefault<IInterceptionPolicy>(context.OriginalBuildKey, false);
                }

                // fallback to the original build key's type configuration
                if (interceptionPolicy == null)
                {
                    interceptionPolicy
                        = context.Policies.GetNoDefault<IInterceptionPolicy>(originalType, false);
                }

                if (interceptionPolicy != null)
                {
                    PolicyInjector injector = interceptionPolicy.Injector;
                    if (injector.TypeSupportsInterception(originalType))
                    {
                        IUnityContainer container = BuilderContext.NewBuildUp<IUnityContainer>(context);
                        InjectionPolicy[] policies = BuilderContext.NewBuildUp<InjectionPolicy[]>(context);
                        PolicySet allPolicies = new PolicySet(policies);
                        context.Existing 
                            = injector.Wrap(context.Existing, originalType, allPolicies, container);
                    }
                }
            }
        }
    }
}
