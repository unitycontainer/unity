// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace ObjectBuilder2
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown. Although you
    /// can implement this interface directly, you may also choose to use
    /// <see cref="BuilderStrategy"/> as the base class for your strategies, as
    /// this class provides useful helper methods and makes support BuildUp and TearDown
    /// optional.
    /// </summary>
    public interface IBuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        void PreBuildUp(IBuilderContext context);

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        void PostBuildUp(IBuilderContext context);

        /// <summary>
        /// Called during the chain of responsibility for a teardown operation. The
        /// PreTearDown method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the teardown operation.</param>
        void PreTearDown(IBuilderContext context);

        /// <summary>
        /// Called during the chain of responsibility for a teardown operation. The
        /// PostTearDown method is called when the chain has finished the PreTearDown
        /// phase and executes in reverse order from the PreTearDown calls.
        /// </summary>
        /// <param name="context">Context of the teardown operation.</param>
        void PostTearDown(IBuilderContext context);
    }
}
