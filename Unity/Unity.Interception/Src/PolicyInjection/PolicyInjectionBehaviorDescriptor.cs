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

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A descriptor for the <see cref="PolicyInjectionBehavior"/>.
    /// </summary>
    public class PolicyInjectionBehaviorDescriptor : IInterceptionBehaviorDescriptor
    {
        /// <summary>
        /// Returns the <see cref="PolicyInjectionBehavior"/> represented by this descriptor.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptor"/> used to create the proxy which will receive the 
        /// returned behavior.</param>
        /// <param name="interceptedType">The type for which a proxy is created.</param>
        /// <param name="implementationType">The type of the intercepted object.</param>
        /// <param name="container">A <see cref="IUnityContainer"/> from which any necessary objects can be resolved
        /// to create the behavior.</param>
        /// <returns>The represented behavior, or <see langword="null"/> if the represented behavior is not 
        /// applicable for the intercepted type.</returns>
        public IInterceptionBehavior GetInterceptionBehavior(
            IInterceptor interceptor,
            Type interceptedType,
            Type implementationType,
            IUnityContainer container)
        {
            InjectionPolicy[] policies = container.Resolve<InjectionPolicy[]>();
            PolicySet allPolicies = new PolicySet(policies);
            bool hasHandlers = false;

            PipelineManager manager = new PipelineManager();
            foreach (MethodImplementationInfo method in
                interceptor.GetInterceptableMethods(interceptedType, implementationType))
            {
                HandlerPipeline pipeline = new HandlerPipeline(allPolicies.GetHandlersFor(method, container));
                if (pipeline.Count > 0)
                {
                    manager.SetPipeline(method.ImplementationMethodInfo.MetadataToken, pipeline);
                    if (method.InterfaceMethodInfo != null)
                    {
                        manager.SetPipeline(method.InterfaceMethodInfo.MetadataToken, pipeline);
                    }
                    hasHandlers = true;
                }
            }

            return hasHandlers ? new PolicyInjectionBehavior(manager) : null;
        }
    }
}
