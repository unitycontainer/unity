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
using System.Reflection;
namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Interceptor that performs policy injection.
    /// </summary>
    public class PolicyInjectionBehavior : IInterceptionBehavior
    {
        private PipelineManager pipelineManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyInjectionBehavior"/> with a pipeline manager.
        /// </summary>
        /// <param name="pipelineManager">The <see cref="PipelineManager"/> for the new instance.</param>
        public PolicyInjectionBehavior(PipelineManager pipelineManager)
        {
            this.pipelineManager = pipelineManager;
        }

        /// <summary>
        /// Applies the policy injection handlers configured for the invoked method.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the handler
        /// chain.</param>
        /// <returns>Return value from the target.</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
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
            return this.pipelineManager.GetPipeline(method.MetadataToken);
        }

        /// <summary>
        /// Returns the interfaces required by the behavior for the objects it intercepts.
        /// </summary>
        /// <returns>An empty array of interfaces.</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }
    }
}
