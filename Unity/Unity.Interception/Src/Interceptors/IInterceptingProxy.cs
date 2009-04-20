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

using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This interface is implemented by all proxy objects, type or instance based.
    /// It gives access to the handler pipelines for each method so that they can
    /// be set.
    /// </summary>
    public interface IInterceptingProxy
    { 
        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">Method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        HandlerPipeline GetPipeline(MethodBase method);

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="method">Method to apply the pipeline to.</param>
        /// <param name="pipeline">The new pipeline.</param>
        void SetPipeline(MethodBase method, HandlerPipeline pipeline);
    }
}
