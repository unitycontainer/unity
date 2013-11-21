// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

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
        /// <param name="policyInterface">The type the policy was registered as.</param>
        void ClearDefault(Type policyInterface);

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <param name="containingPolicyList">The policy list in the chain that the searched for policy was found in, null if the policy was
        /// not found.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get",
            Justification = "Back compat with ObjectBuilder")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#")]
        IBuilderPolicy Get(Type policyInterface,
            object buildKey,
            bool localOnly,
            out IPolicyList containingPolicyList);

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies to.</param>
        /// <param name="localOnly">True if the search should be in the local policy list only; otherwise false to search up the parent chain.</param>
        /// <param name="containingPolicyList">The policy list in the chain that the searched for policy was found in, null if the policy was
        /// not found.</param>
        /// <returns>The policy in the list if present; returns null otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#")]
        IBuilderPolicy GetNoDefault(Type policyInterface, object buildKey, bool localOnly,
            out IPolicyList containingPolicyList);

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The <see cref="Type"/> of the policy.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set",
            Justification = "Back compat with ObjectBuilder")]
        void Set(Type policyInterface,
                 IBuilderPolicy policy,
                 object buildKey);

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
