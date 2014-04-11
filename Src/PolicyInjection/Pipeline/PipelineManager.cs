// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A collection of <see cref="HandlerPipeline"/> objects, indexed
    /// by <see cref="MethodBase"/>. Returns an empty pipeline if a
    /// MethodBase is requested that isn't in the dictionary.
    /// </summary>
    public class PipelineManager
    {
        private readonly Dictionary<HandlerPipelineKey, HandlerPipeline> pipelines =
            new Dictionary<HandlerPipelineKey, HandlerPipeline>();

        private static readonly HandlerPipeline EmptyPipeline = new HandlerPipeline();

        /// <summary>
        /// Retrieve the pipeline associated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        public HandlerPipeline GetPipeline(MethodBase method)
        {
            HandlerPipelineKey key = HandlerPipelineKey.ForMethod(method);
            HandlerPipeline pipeline = EmptyPipeline;
            if (pipelines.ContainsKey(key))
            {
                pipeline = pipelines[key];
            }
            return pipeline;
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="method">The method on which the pipeline should be set.</param>
        /// <param name="pipeline">The new pipeline.</param>
        public void SetPipeline(MethodBase method, HandlerPipeline pipeline)
        {
            HandlerPipelineKey key = HandlerPipelineKey.ForMethod(method);
            pipelines[key] = pipeline;
        }

        /// <summary>
        /// Get the pipeline for the given method, creating it if necessary.
        /// </summary>
        /// <param name="method">Method to retrieve the pipeline for.</param>
        /// <param name="handlers">Handlers to initialize the pipeline with</param>
        /// <returns>True if the pipeline has any handlers in it, false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public bool InitializePipeline(MethodImplementationInfo method, IEnumerable<ICallHandler> handlers)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(method, "method");

            var pipeline = CreatePipeline(method.ImplementationMethodInfo, handlers);
            if (method.InterfaceMethodInfo != null)
            {
                pipelines[HandlerPipelineKey.ForMethod(method.InterfaceMethodInfo)] = pipeline;
            }

            return pipeline.Count > 0;
        }

        private HandlerPipeline CreatePipeline(MethodInfo method, IEnumerable<ICallHandler> handlers)
        {
            HandlerPipelineKey key = HandlerPipelineKey.ForMethod(method);
            if (pipelines.ContainsKey(key))
            {
                return pipelines[key];
            }

            if (method.GetBaseDefinition() == method)
            {
                pipelines[key] = new HandlerPipeline(handlers);
                return pipelines[key];
            }

            var basePipeline = CreatePipeline(method.GetBaseDefinition(), handlers);
            pipelines[key] = basePipeline;
            return basePipeline;
        }
    }
}
