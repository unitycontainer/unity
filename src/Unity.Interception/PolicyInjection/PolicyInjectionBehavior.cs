// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Utility;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// Interceptor that performs policy injection.
    /// </summary>
    public class PolicyInjectionBehavior : IInterceptionBehavior
    {
        private readonly PipelineManager pipelineManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyInjectionBehavior"/> with a pipeline manager.
        /// </summary>
        /// <param name="pipelineManager">The <see cref="PipelineManager"/> for the new instance.</param>
        public PolicyInjectionBehavior(PipelineManager pipelineManager)
        {
            this.pipelineManager = pipelineManager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyInjectionBehavior"/> with the given information
        /// about what's being intercepted and the current set of injection policies.
        /// </summary>
        /// <param name="interceptionRequest">Information about what will be injected.</param>
        /// <param name="policies">Current injection policies.</param>
        /// <param name="container">Unity container that can be used to resolve call handlers.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public PolicyInjectionBehavior(CurrentInterceptionRequest interceptionRequest, InjectionPolicy[] policies,
            IUnityContainer container)
        {
            Unity.Utility.Guard.ArgumentNotNull(interceptionRequest, "interceptionRequest");

            var allPolicies = new PolicySet(policies);
            bool hasHandlers = false;

            var manager = new PipelineManager();

            foreach (MethodImplementationInfo method in
                interceptionRequest.Interceptor.GetInterceptableMethods(
                    interceptionRequest.TypeToIntercept, interceptionRequest.ImplementationType))
            {
                bool hasNewHandlers = manager.InitializePipeline(method,
                    allPolicies.GetHandlersFor(method, container));
                hasHandlers = hasHandlers || hasNewHandlers;
            }
            pipelineManager = hasHandlers ? manager : null;
        }

        /// <summary>
        /// Applies the policy injection handlers configured for the invoked method.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the handler
        /// chain.</param>
        /// <returns>Return value from the target.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            Guard.ArgumentNotNull(input, "input");
            Guard.ArgumentNotNull(getNext, "getNext");

            HandlerPipeline pipeline = GetPipeline(input.MethodBase);

            return pipeline.Invoke(
                    input,
                    delegate(IMethodInvocation policyInjectionInput, GetNextHandlerDelegate policyInjectionInputGetNext)
                    {
                        try
                        {
                            return getNext()(policyInjectionInput, getNext);
                        }
                        catch (TargetInvocationException ex)
                        {
                            // The outer exception will always be a reflection exception; we want the inner, which is
                            // the underlying exception.
                            return policyInjectionInput.CreateExceptionMethodReturn(ex.InnerException);
                        }
                    });
        }

        private HandlerPipeline GetPipeline(MethodBase method)
        {
            return this.pipelineManager.GetPipeline(method);
        }

        /// <summary>
        /// Returns the interfaces required by the behavior for the objects it intercepts.
        /// </summary>
        /// <returns>An empty array of interfaces.</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// Returns a flag indicating if this behavior will actually do anything when invoked.
        /// </summary>
        /// <remarks>This is used to optimize interception. If the behaviors won't actually
        /// do anything (for example, PIAB where no policies match) then the interception
        /// mechanism can be skipped completely.</remarks>
        public bool WillExecute
        {
            get { return pipelineManager != null; }
        }
    }
}
