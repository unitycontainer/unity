// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// The InterceptionBehaviorPipeline class encapsulates a list of <see cref="IInterceptionBehavior"/>s
    /// and manages calling them in the proper order with the right inputs.
    /// </summary>
    public class InterceptionBehaviorPipeline
    {
        private readonly List<IInterceptionBehavior> interceptionBehaviors;

        /// <summary>
        /// Creates a new <see cref="HandlerPipeline"/> with an empty pipeline.
        /// </summary>
        public InterceptionBehaviorPipeline()
        {
            interceptionBehaviors = new List<IInterceptionBehavior>();
        }

        /// <summary>
        /// Creates a new <see cref="HandlerPipeline"/> with the given collection
        /// of <see cref="ICallHandler"/>s.
        /// </summary>
        /// <param name="interceptionBehaviors">Collection of interception behaviors to add to the pipeline.</param>
        public InterceptionBehaviorPipeline(IEnumerable<IInterceptionBehavior> interceptionBehaviors)
        {
            Guard.ArgumentNotNull(interceptionBehaviors, "interceptionBehaviors");
            this.interceptionBehaviors = new List<IInterceptionBehavior>(interceptionBehaviors);
        }

        /// <summary>
        /// Get the number of interceptors in this pipeline.
        /// </summary>
        public int Count
        {
            get { return interceptionBehaviors.Count; }
        }

        /// <summary>
        /// Execute the pipeline with the given input.
        /// </summary>
        /// <param name="input">Input to the method call.</param>
        /// <param name="target">The ultimate target of the call.</param>
        /// <returns>Return value from the pipeline.</returns>
        public IMethodReturn Invoke(IMethodInvocation input, InvokeInterceptionBehaviorDelegate target)
        {
            if (interceptionBehaviors.Count == 0)
            {
                return target(input, null);
            }

            int interceptorIndex = 0;

            IMethodReturn result = interceptionBehaviors[0].Invoke(input, delegate
                                      {
                                          ++interceptorIndex;
                                          if (interceptorIndex < interceptionBehaviors.Count)
                                          {
                                              return interceptionBehaviors[interceptorIndex].Invoke;
                                          }
                                          else
                                          {
                                              return target;
                                          }
                                      });
            return result;
        }

        /// <summary>
        /// Adds a <see cref="IInterceptionBehavior"/> to the pipeline.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to add.</param>
        public void Add(IInterceptionBehavior interceptionBehavior)
        {
            Guard.ArgumentNotNull(interceptionBehavior, "interceptionBehavior");
            this.interceptionBehaviors.Add(interceptionBehavior);
        }
    }
}
