namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This interface provides the hook to get at and manipulate
    /// the pipelines for a virtual-method intercepted object.
    /// </summary>
    public interface IHandlerPipelineManager
    {
        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="methodToken"/>.
        /// </summary>
        /// <param name="methodToken">Metadata token for the method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        HandlerPipeline GetPipeline(int methodToken);

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="methodToken">Metadata token for the method to apply the pipeline to.</param>
        /// <param name="pipeline">The new pipeline.</param>
        void SetPipeline(int methodToken, HandlerPipeline pipeline);
    }
}