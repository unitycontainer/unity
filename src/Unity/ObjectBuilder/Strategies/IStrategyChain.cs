// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace ObjectBuilder2
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public interface IStrategyChain : IEnumerable<IBuilderStrategy>
    {
        /// <summary>
        /// Reverse the order of the strategy chain.
        /// </summary>
        /// <returns>The reversed strategy chain.</returns>
        IStrategyChain Reverse();

        /// <summary>
        /// Execute this strategy chain against the given context,
        /// calling the Buildup methods on the strategies.
        /// </summary>
        /// <param name="context">Context for the build process.</param>
        /// <returns>The build up object</returns>
        object ExecuteBuildUp(IBuilderContext context);

        /// <summary>
        /// Execute this strategy chain against the given context,
        /// calling the TearDown methods on the strategies.
        /// </summary>
        /// <param name="context">Context for the teardown process.</param>
        void ExecuteTearDown(IBuilderContext context);
    }
}
