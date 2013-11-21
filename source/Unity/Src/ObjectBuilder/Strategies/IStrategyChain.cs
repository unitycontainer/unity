// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    // FxCop suppression: This class is only IEnumerable for testing support.
    // Renaming it to StrategyCollection implies more than it really should.
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
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
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp",
            Justification = "Back compat with ObjectBuilder")]
        object ExecuteBuildUp(IBuilderContext context);

        /// <summary>
        /// Execute this strategy chain against the given context,
        /// calling the TearDown methods on the strategies.
        /// </summary>
        /// <param name="context">Context for the teardown process.</param>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown",
            Justification = "Back compat with ObjectBuilder")]
        void ExecuteTearDown(IBuilderContext context);

    }
}
