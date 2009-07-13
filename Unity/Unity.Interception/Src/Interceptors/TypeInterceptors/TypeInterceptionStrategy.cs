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
        private readonly object lockObj = new object();

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

            if(context.Existing != null) return;
            
            Type typeToBuild;
            if(!BuildKey.TryGetType(context.BuildKey, out typeToBuild)) return;

            ITypeInterceptionPolicy interceptionPolicy = GetInterceptionPolicy(context);
            if(interceptionPolicy == null) return;

            if(!interceptionPolicy.Interceptor.CanIntercept(typeToBuild)) return;
            
            lock(lockObj)
            {
                if(interceptionPolicy.ProxyType == null)
                {
                    Type typeToIntercept = typeToBuild;
                    if(typeToIntercept.IsGenericType)
                    {
                        typeToIntercept = typeToIntercept.GetGenericTypeDefinition();
                    }

                    interceptionPolicy.ProxyType = interceptionPolicy.Interceptor.CreateProxyType(typeToIntercept);
                }
            }

            Type interceptingType = interceptionPolicy.ProxyType;
            if(interceptingType.IsGenericTypeDefinition)
            {
                interceptingType = interceptingType.MakeGenericType(typeToBuild.GetGenericArguments());
            }

            SelectedConstructor currentConstructor = GetCurrentConstructor(context);
            SelectedConstructor newConstructor = FindNewConstructor(currentConstructor, interceptingType);
            IConstructorSelectorPolicy newSelector = new ConstructorWithResolverKeysSelectorPolicy(newConstructor);
            context.Policies.Set(newSelector, context.BuildKey);
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
            if(proxy == null) return;

            ITypeInterceptionPolicy interceptionPolicy = GetInterceptionPolicy(context);

            Type typeToIntercept = BuildKey.GetType(context.BuildKey);
            PolicySet interceptionPolicies = new PolicySet(BuilderContext.NewBuildUp<InjectionPolicy[]>(context));
            IUnityContainer currentContainer = BuilderContext.NewBuildUp<IUnityContainer>(context);

            foreach (MethodImplementationInfo item in interceptionPolicy.Interceptor.GetInterceptableMethods(typeToIntercept, typeToIntercept))
            {
                HandlerPipeline pipeline = new HandlerPipeline(
                    interceptionPolicies.GetHandlersFor(item, currentContainer));
                proxy.SetPipeline(interceptionPolicy.Interceptor.MethodInfoForPipeline(item), pipeline);
            }
        }

        private static ITypeInterceptionPolicy GetInterceptionPolicy(IBuilderContext context)
        {
            ITypeInterceptionPolicy policy;
            policy = context.Policies.Get<ITypeInterceptionPolicy>(context.BuildKey, false);
            if(policy == null)
            {
                policy = context.Policies.Get<ITypeInterceptionPolicy>(BuildKey.GetType(context.BuildKey), false);
            }
            return policy;
        }

        private static SelectedConstructor GetCurrentConstructor(IBuilderContext context)
        {
            IConstructorSelectorPolicy originalSelector = context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey);
            return originalSelector.SelectConstructor(context);
        }

        private static SelectedConstructor FindNewConstructor(SelectedConstructor originalConstructor, Type proxyType)
        {
            ParameterInfo[] originalParams = originalConstructor.Constructor.GetParameters();

            ConstructorInfo newConstructorInfo = proxyType.GetConstructor(Seq.Make(originalParams)
                .Map<Type>(delegate(ParameterInfo pi) { return pi.ParameterType; }).ToArray());

            SelectedConstructor newConstructor = new SelectedConstructor(newConstructorInfo);

            foreach(string key in originalConstructor.GetParameterKeys())
            {
                newConstructor.AddParameterKey(key);
            }

            return newConstructor;
        }
    }
}
