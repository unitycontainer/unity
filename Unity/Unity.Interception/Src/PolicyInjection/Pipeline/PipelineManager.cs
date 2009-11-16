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

        private static readonly HandlerPipeline emptyPipeline = new HandlerPipeline();

        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        public HandlerPipeline GetPipeline(MethodBase method)
        {
            return GetPipeline(HandlerPipelineKey.ForMethod(method));
        }

        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given key. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        public HandlerPipeline GetPipeline(HandlerPipelineKey key)
        {
            HandlerPipeline pipeline = emptyPipeline;
            if(pipelines.ContainsKey(key))
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
            SetPipeline(HandlerPipelineKey.ForMethod(method), pipeline);
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="key">The key on which the pipeline should be set.</param>
        /// <param name="pipeline">The new pipeline.</param>
        public void SetPipeline(HandlerPipelineKey key, HandlerPipeline pipeline)
        {
            pipelines[key] = pipeline;
        }
    }
}