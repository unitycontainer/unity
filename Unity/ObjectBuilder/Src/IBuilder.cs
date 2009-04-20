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

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents the main interface for an object builder.
    /// </summary>
	public interface IBuilder
	{
        /// <summary>
        /// Performs a build operation.
        /// </summary>
        /// <remarks>
        /// This operation uses the strategies and permanent policies already configured
        /// into the builder, combined with the optional transient policies, and starts a build
        /// operation. Transient policies override any built-in policies, when present.
        /// </remarks>
        /// <param name="locator">The locator to be used for this build operation.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this build operation.</param>
        /// <param name="policies">
        /// The transient policies to apply to this build. These
        /// policies take precedence over any permanent policies built into the builder.
        /// </param>
        /// <param name="strategies">
        /// The <see cref="IStrategyChain"/> to use for this build operation.
        /// </param>
        /// <param name="buildKey">The key of the object to build.</param>
        /// <param name="existing">
        /// The existing object to run the build chain on, if one exists.
        /// If null is passed, a new object instance will typically be created by some strategy
        /// in the chain.
        /// </param>
        /// <returns>The built object.</returns>
        // FxCop suppression: Disambiguation is done at usage by using explicit generic syntax.
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp",
            Justification="Kept for backward compatibility with ObjectBuilder")]
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
        object BuildUp(IReadWriteLocator locator,
		               ILifetimeContainer lifetime,
		               IPolicyList policies,
		               IStrategyChain strategies,
		               object buildKey,
		               object existing);

        /// <summary>
        /// Performs a build operation.
        /// </summary>
        /// <remarks>
        /// This operation uses the strategies and permanent policies already configured
        /// into the builder, combined with the optional transient policies, and starts a build
        /// operation. Transient policies override any built-in policies, when present.
        /// </remarks>
        /// <typeparam name="TTypeToBuild">The type to build.</typeparam>
        /// <param name="locator">The locator to be used for this build operation.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this build operation.</param>
        /// <param name="policies">
        /// The transient policies to apply to this build. These
        /// policies take precedence over any permanent policies built into the builder.
        /// </param>
        /// <param name="strategies">
        /// The <see cref="IStrategyChain"/> to use for this build operation.
        /// </param>
        /// <param name="buildKey">The key of the object to build.</param>
        /// <param name="existing">
        /// The existing object to run the build chain on, if one exists.
        /// If null is passed, a new object instance will typically be created by some strategy
        /// in the chain.
        /// </param>
        /// <returns>The built object.</returns>
        // FxCop suppression: Disambiguation is done at usage by using explicit generic syntax.
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
        TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
		                                   ILifetimeContainer lifetime,
		                                   IPolicyList policies,
		                                   IStrategyChain strategies,
		                                   object buildKey,
		                                   object existing);

        /// <summary>
        /// Performs an unbuild operation.
        /// </summary>
        /// <typeparam name="TItem">The type to unbuild. If not provided, it will be inferred from the
        /// type of item.
        /// </typeparam>
        /// <param name="locator">The locator to be used for this build operation.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this build operation.</param>
        /// <param name="policies">
        /// The transient policies to apply to this build. These
        /// policies take precedence over any permanent policies built into the builder.
        /// </param>
        /// <param name="strategies">
        /// The <see cref="IStrategyChain"/> to use for this build operation.
        /// </param>
        /// <param name="item">The item to tear down.</param>
        /// <returns>The torn down item.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
		TItem TearDown<TItem>(IReadWriteLocator locator,
		                      ILifetimeContainer lifetime,
		                      IPolicyList policies,
		                      IStrategyChain strategies,
		                      TItem item);
	}
}
