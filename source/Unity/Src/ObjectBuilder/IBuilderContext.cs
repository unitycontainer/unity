// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity;
using Unity.Utility;

namespace ObjectBuilder2
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    public interface IBuilderContext
    {
        /// <summary>
        /// Gets the head of the strategy chain.
        /// </summary>
        /// <returns>
        /// The strategy that's first in the chain; returns null if there are no
        /// strategies in the chain.
        /// </returns>
        IStrategyChain Strategies { get; }

        /// <summary>
        /// Gets the <see cref="ILifetimeContainer"/> associated with the build.
        /// </summary>
        /// <value>
        /// The <see cref="ILifetimeContainer"/> associated with the build.
        /// </value>
        ILifetimeContainer Lifetime { get; }

        /// <summary>
        /// Gets the original build key for the build operation.
        /// </summary>
        /// <value>
        /// The original build key for the build operation.
        /// </value>
        NamedTypeBuildKey OriginalBuildKey { get; }

        /// <summary>
        /// Get the current build key for the current build operation.
        /// </summary>
        NamedTypeBuildKey BuildKey { get; set; }

        /// <summary>
        /// The set of policies that were passed into this context.
        /// </summary>
        /// <remarks>This returns the policies passed into the context.
        /// Policies added here will remain after buildup completes.</remarks>
        /// <value>The persistent policies for the current context.</value>
        IPolicyList PersistentPolicies { get; }

        /// <summary>
        /// Gets the policies for the current context. 
        /// </summary>
        /// <remarks>Any policies added to this object are transient
        /// and will be erased at the end of the buildup.</remarks>
        /// <value>
        /// The policies for the current context.
        /// </value>
        IPolicyList Policies { get; }

        /// <summary>
        /// Gets the collection of <see cref="IRequiresRecovery"/> objects
        /// that need to execute in event of an exception.
        /// </summary>
        IRecoveryStack RecoveryStack { get; }

        /// <summary>
        /// The current object being built up or torn down.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        object Existing { get; set; }

        /// <summary>
        /// Flag indicating if the build operation should continue.
        /// </summary>
        /// <value>true means that building should not call any more
        /// strategies, false means continue to the next strategy.</value>
        bool BuildComplete { get; set; }

        /// <summary>
        /// An object representing what is currently being done in the
        /// build chain. Used to report back errors if there's a failure.
        /// </summary>
        object CurrentOperation { get; set; }

        /// <summary>
        /// The build context used to resolve a dependency during the build operation represented by this context.
        /// </summary>
        IBuilderContext ChildContext { get; }

        /// <summary>
        /// Add a new set of resolver override objects to the current build operation.
        /// </summary>
        /// <param name="newOverrides"><see cref="ResolverOverride"/> objects to add.</param>
        void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides);

        /// <summary>
        /// Get a <see cref="IDependencyResolverPolicy"/> object for the given <paramref name="dependencyType"/>
        /// or null if that dependency hasn't been overridden.
        /// </summary>
        /// <param name="dependencyType">Type of the dependency.</param>
        /// <returns>Resolver to use, or null if no override matches for the current operation.</returns>
        IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType);

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context.
        /// </summary>
        /// <param name="newBuildKey">Key to use to build up.</param>
        /// <returns>Created object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp", Justification = "BuildUp is correct.")]
        object NewBuildUp(NamedTypeBuildKey newBuildKey);

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context. This
        /// overload allows you to specify extra policies which will be in effect for the duration
        /// of the build.
        /// </summary>
        /// <param name="newBuildKey">Key defining what to build up.</param>
        /// <param name="childCustomizationBlock">A delegate that takes a <see cref="IBuilderContext"/>. This
        /// is invoked with the new child context before the build up process starts. This gives callers
        /// the opportunity to customize the context for the build process.</param>
        /// <returns>Created object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp", Justification = "BuildUp is correct.")]
        object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock);
    }

    /// <summary>
    /// Extension methods to provide convenience overloads over the
    /// <see cref="IBuilderContext"/> interface.
    /// </summary>
    public static class BuilderContextExtensions
    {
        /// <summary>
        /// Start a recursive build up operation to retrieve the default
        /// value for the given <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of object to build.</typeparam>
        /// <param name="context">Parent context.</param>
        /// <returns>Resulting object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp", Justification = "BuildUp is correct.")]
        public static TResult NewBuildUp<TResult>(this IBuilderContext context)
        {
            return context.NewBuildUp<TResult>(null);
        }

        /// <summary>
        /// Start a recursive build up operation to retrieve the named
        /// implementation for the given <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type to resolve.</typeparam>
        /// <param name="context">Parent context.</param>
        /// <param name="name">Name to resolve with.</param>
        /// <returns>The resulting object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp", Justification = "BuildUp is correct.")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Checked with Guard class")]
        public static TResult NewBuildUp<TResult>(this IBuilderContext context, string name)
        {
            Guard.ArgumentNotNull(context, "context");
            return (TResult)context.NewBuildUp(NamedTypeBuildKey.Make<TResult>(name));
        }

        /// <summary>
        /// Add a set of <see cref="ResolverOverride"/>s to the context, specified as a 
        /// variable argument list.
        /// </summary>
        /// <param name="context">Context to add overrides to.</param>
        /// <param name="overrides">The overrides.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Checked with Guard class")]
        public static void AddResolverOverrides(this IBuilderContext context, params ResolverOverride[] overrides)
        {
            Guard.ArgumentNotNull(context, "context");

            context.AddResolverOverrides(overrides);
        }
    }
}
