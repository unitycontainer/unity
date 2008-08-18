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

using System;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A custom collection over <see cref="IBuilderPolicy"/> objects.
    /// </summary>
    public interface IPolicyList
    {
        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        void Clear<TPolicyInterface>(object buildKey);

        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <param name="policyInterface">The type of policy to remove.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        void Clear(Type policyInterface,
                   object buildKey);

        /// <summary>
        /// Removes all policies from the list.
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        void ClearDefault<TPolicyInterface>();

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <param name="policyInterface">The type the policy was registered as.</param>
        void ClearDefault(Type policyInterface);

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        TPolicyInterface Get<TPolicyInterface>(object buildKey)
            where TPolicyInterface : IBuilderPolicy;


        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        IBuilderPolicy Get(Type policyInterface,
                           object buildKey);

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        TPolicyInterface Get<TPolicyInterface>(object buildKey,
                                               bool localOnly)
            where TPolicyInterface : IBuilderPolicy;

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        IBuilderPolicy Get(Type policyInterface,
                           object buildKey,
                           bool localOnly);

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        TPolicyInterface GetNoDefault<TPolicyInterface>(object buildKey,
                                                        bool localOnly)
            where TPolicyInterface : IBuilderPolicy;

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        IBuilderPolicy GetNoDefault(Type policyInterface,
                                    object buildKey,
                                    bool localOnly);

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        void Set<TPolicyInterface>(TPolicyInterface policy,
                                   object buildKey)
            where TPolicyInterface : IBuilderPolicy;

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The <see cref="Type"/> of the policy.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        void Set(Type policyInterface,
                 IBuilderPolicy policy,
                 object buildKey);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface to register the policy under.</typeparam>
        /// <param name="policy">The default policy to be registered.</param>
        void SetDefault<TPolicyInterface>(TPolicyInterface policy)
            where TPolicyInterface : IBuilderPolicy;

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policyInterface">The interface to register the policy under.</param>
        /// <param name="policy">The default policy to be registered.</param>
        void SetDefault(Type policyInterface,
                        IBuilderPolicy policy);
    }
}
