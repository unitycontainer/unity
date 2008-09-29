using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A collection of <see cref="HandlerPipeline"/> objects, indexed
    /// by <see cref="MethodBase"/>. Returns an empty pipeline if a
    /// MethodBase is requested that isn't in the dictionary.
    /// </summary>
    public class PipelineManager : IHandlerPipelineManager
    {
        private readonly Dictionary<int, HandlerPipeline> pipelines =
            new Dictionary<int, HandlerPipeline>();

        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="methodToken"/>.
        /// </summary>
        /// <param name="methodToken">Metadata token for the method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        public HandlerPipeline GetPipeline(int methodToken)
        {
            return pipelines.ContainsKey(methodToken) ? pipelines[methodToken] : new HandlerPipeline();
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="methodToken">Metadata token for the method to apply the pipeline to.</param>
        /// <param name="pipeline">The new pipeline.</param>
        public void SetPipeline(int methodToken, HandlerPipeline pipeline)
        {
            pipelines[methodToken] = pipeline;
        }
    }
}