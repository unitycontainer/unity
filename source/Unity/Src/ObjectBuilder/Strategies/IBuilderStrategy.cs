// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
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
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
        void PreBuildUp(IBuilderContext context);

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
        void PostBuildUp(IBuilderContext context);

        /// <summary>
        /// Called during the chain of responsibility for a teardown operation. The
        /// PreTearDown method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the teardown operation.</param>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
        void PreTearDown(IBuilderContext context);

        /// <summary>
        /// Called during the chain of responsibility for a teardown operation. The
        /// PostTearDown method is called when the chain has finished the PreTearDown
        /// phase and executes in reverse order from the PreTearDown calls.
        /// </summary>
        /// <param name="context">Context of the teardown operation.</param>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
        void PostTearDown(IBuilderContext context);
    }
}
