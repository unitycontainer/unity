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
    /// An implementation of <see cref="IBuilder"/>. It contains all the default strategies shipped
    /// with ObjectBuilder.
    /// </summary>
	public class Builder : IBuilder
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
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation is done via Guard class")]
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType",
            Justification = "Use of the methods will disambiguate via explicit generic syntax")]
        public object BuildUp(IReadWriteLocator locator,
		                      ILifetimeContainer lifetime,
		                      IPolicyList policies,
		                      IStrategyChain strategies,
		                      object buildKey,
		                      object existing)
		{
			Guard.ArgumentNotNull(strategies, "strategies");

			BuilderContext context = new BuilderContext(strategies, locator, lifetime, policies, buildKey, existing);
            return strategies.ExecuteBuildUp(context);
		}

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
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType",
            Justification = "Use of the methods will disambiguate via explicit generic syntax")]
        public TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
		                                          ILifetimeContainer lifetime,
		                                          IPolicyList policies,
		                                          IStrategyChain strategies,
		                                          object buildKey,
		                                          object existing)
		{
			return (TTypeToBuild)BuildUp(locator, lifetime, policies, strategies, buildKey, existing);
		}

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
		public TItem TearDown<TItem>(IReadWriteLocator locator,
		                             ILifetimeContainer lifetime,
		                             IPolicyList policies,
		                             IStrategyChain strategies,
		                             TItem item)
		{
			Guard.ArgumentNotNull(item, "item");
			Guard.ArgumentNotNull(strategies, "strategies");

			BuilderContext context = new BuilderContext(strategies.Reverse(), locator, lifetime, policies, null, item);
            context.Strategies.ExecuteTearDown(context);
            return (TItem)(context.Existing);
		}
	}
}
