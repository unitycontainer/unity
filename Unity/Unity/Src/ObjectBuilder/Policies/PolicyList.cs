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
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{

    /// <summary>
    /// A custom collection wrapper over <see cref="IBuilderPolicy"/> objects.
    /// </summary>
    public class PolicyList : IPolicyList
    {
        readonly IPolicyList innerPolicyList;
        readonly object lockObject = new object();
        private Dictionary<PolicyKey, IBuilderPolicy> policies = new Dictionary<PolicyKey, IBuilderPolicy>();

        /// <summary>
        /// Initialize a new instance of a <see cref="PolicyList"/> class.
        /// </summary>
        public PolicyList()
            : this(null) {}

        /// <summary>
        /// Initialize a new instance of a <see cref="PolicyList"/> class with another policy list.
        /// </summary>
        /// <param name="innerPolicyList">An inner policy list to search.</param>
        public PolicyList(IPolicyList innerPolicyList)
        {
            this.innerPolicyList = innerPolicyList ?? new NullPolicyList();
        }

        /// <summary>
        /// Gets the number of items in the locator.
        /// </summary>
        /// <value>
        /// The number of items in the locator.
        /// </value>
        public int Count
        {
            get
            {
                return policies.Count;
            }
        }

        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        public void Clear<TPolicyInterface>(object buildKey)
        {
            Clear(typeof(TPolicyInterface), buildKey);
        }

        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <param name="policyInterface">The type of policy to remove.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public void Clear(Type policyInterface,
                          object buildKey)
        {
            lock (lockObject)
            {
                Dictionary<PolicyKey, IBuilderPolicy> newPolicies = ClonePolicies();
                newPolicies.Remove(new PolicyKey(policyInterface, buildKey));
                SwapPolicies(newPolicies);
            }
        }

        /// <summary>
        /// Removes all policies from the list.
        /// </summary>
        public void ClearAll()
        {
            lock (lockObject)
            {
                SwapPolicies(new Dictionary<PolicyKey, IBuilderPolicy>());
            }
        }

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        public void ClearDefault<TPolicyInterface>()
        {
            Clear(typeof(TPolicyInterface), null);
        }

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <param name="policyInterface">The type the policy was registered as.</param>
        public void ClearDefault(Type policyInterface)
        {
            Clear(policyInterface, null);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public TPolicyInterface Get<TPolicyInterface>(object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)Get(typeof(TPolicyInterface), buildKey, false);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public IBuilderPolicy Get(Type policyInterface,
                                  object buildKey)
        {
            return Get(policyInterface, buildKey, false);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public TPolicyInterface Get<TPolicyInterface>(object buildKey,
                                                      bool localOnly)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)Get(typeof(TPolicyInterface), buildKey, localOnly);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public IBuilderPolicy Get(Type policyInterface,
                                  object buildKey,
                                  bool localOnly)
        {
            Type buildType;

            if (!TryGetType(buildKey, out buildType) || !buildType.IsGenericType)
                return
                    GetNoDefault(policyInterface, buildKey, localOnly) ??
                    GetNoDefault(policyInterface, null, localOnly);

            return
                GetNoDefault(policyInterface, buildKey, localOnly) ??
                GetNoDefault(policyInterface, ReplaceType(buildKey, buildType.GetGenericTypeDefinition()), localOnly) ??
                GetNoDefault(policyInterface, null, localOnly);
        }

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public TPolicyInterface GetNoDefault<TPolicyInterface>(object buildKey,
                                                               bool localOnly)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)GetNoDefault(typeof(TPolicyInterface), buildKey, localOnly);
        }

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to seach up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public IBuilderPolicy GetNoDefault(Type policyInterface,
                                           object buildKey,
                                           bool localOnly)
        {
            IBuilderPolicy policy;
            if (policies.TryGetValue(new PolicyKey(policyInterface, buildKey), out policy))
                return policy;

            if (localOnly)
                return null;

            return innerPolicyList.GetNoDefault(policyInterface, buildKey, localOnly);
        }

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public void Set<TPolicyInterface>(TPolicyInterface policy,
                                          object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            Set(typeof(TPolicyInterface), policy, buildKey);
        }

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The <see cref="Type"/> of the policy.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public void Set(Type policyInterface,
                        IBuilderPolicy policy,
                        object buildKey)
        {
            lock (lockObject)
            {
                Dictionary<PolicyKey, IBuilderPolicy> newPolicies = ClonePolicies();
                newPolicies[new PolicyKey(policyInterface, buildKey)] = policy;
                SwapPolicies(newPolicies);
            }
        }

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface to register the policy under.</typeparam>
        /// <param name="policy">The default policy to be registered.</param>
        public void SetDefault<TPolicyInterface>(TPolicyInterface policy)
            where TPolicyInterface : IBuilderPolicy
        {
            Set(typeof(TPolicyInterface), policy, null);
        }

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policyInterface">The interface to register the policy under.</param>
        /// <param name="policy">The default policy to be registered.</param>
        public void SetDefault(Type policyInterface,
                               IBuilderPolicy policy)
        {
            Set(policyInterface, policy, null);
        }

        private Dictionary<PolicyKey, IBuilderPolicy> ClonePolicies()
        {
            return new Dictionary<PolicyKey, IBuilderPolicy>(policies);
        }

        private void SwapPolicies(Dictionary<PolicyKey, IBuilderPolicy> newPolicies)
        {
            policies = newPolicies;
            Thread.MemoryBarrier();
        }

        private static bool TryGetType(object buildKey, out Type type)
        {
            type = buildKey as Type;

            if (type == null)
            {
                var basedBuildKey = buildKey as NamedTypeBuildKey;
                if (basedBuildKey != null)
                    type = basedBuildKey.Type;
            }

            return type != null;
        }

        private static object ReplaceType(object buildKey, Type newType)
        {
            var typeKey = buildKey as Type;
            if (typeKey != null)
            {
                return newType;
            }

            var originalKey = buildKey as NamedTypeBuildKey;
            if (originalKey != null)
            {
                return new NamedTypeBuildKey(newType, originalKey.Name);
            }

            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.CannotExtractTypeFromBuildKey,
                    buildKey),
                "buildKey");
        }

        class NullPolicyList : IPolicyList
        {
            public void Clear<TPolicyInterface>(object buildKey)
            {
                throw new NotImplementedException();
            }

            public void Clear(Type policyInterface,
                              object buildKey)
            {
                throw new NotImplementedException();
            }

            public void ClearAll()
            {
                throw new NotImplementedException();
            }

            public void ClearDefault<TPolicyInterface>()
            {
                throw new NotImplementedException();
            }

            public void ClearDefault(Type policyInterface)
            {
                throw new NotImplementedException();
            }

            public TPolicyInterface Get<TPolicyInterface>(object buildKey) where TPolicyInterface : IBuilderPolicy
            {
                return default(TPolicyInterface);
            }

            public IBuilderPolicy Get(Type policyInterface,
                                      object buildKey)
            {
                return null;
            }

            public TPolicyInterface Get<TPolicyInterface>(object buildKey,
                                                          bool localOnly)
                where TPolicyInterface : IBuilderPolicy
            {
                return default(TPolicyInterface);
            }

            public IBuilderPolicy Get(Type policyInterface,
                                      object buildKey,
                                      bool localOnly)
            {
                return null;
            }

            public TPolicyInterface GetNoDefault<TPolicyInterface>(object buildKey,
                                                                   bool localOnly)
                where TPolicyInterface : IBuilderPolicy
            {
                return default(TPolicyInterface);
            }

            public IBuilderPolicy GetNoDefault(Type policyInterface,
                                               object buildKey,
                                               bool localOnly)
            {
                return null;
            }

            public void Set<TPolicyInterface>(TPolicyInterface policy,
                                              object buildKey)
                where TPolicyInterface : IBuilderPolicy
            {
                throw new NotImplementedException();
            }

            public void Set(Type policyInterface,
                            IBuilderPolicy policy,
                            object buildKey)
            {
                throw new NotImplementedException();
            }

            public void SetDefault<TPolicyInterface>(TPolicyInterface policy)
                where TPolicyInterface : IBuilderPolicy
            {
                throw new NotImplementedException();
            }

            public void SetDefault(Type policyInterface,
                                   IBuilderPolicy policy)
            {
                throw new NotImplementedException();
            }
        }

        struct PolicyKey
        {
#pragma warning disable 219
            public readonly object BuildKey;
            public readonly Type PolicyType;
#pragma warning restore 219

            public PolicyKey(Type policyType,
                             object buildKey)
            {
                PolicyType = policyType;
                BuildKey = buildKey;
            }


            public override bool Equals(object obj)
            {
                if(obj != null && obj.GetType() == typeof(PolicyKey))
                {
                    return this == (PolicyKey) obj;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (SafeGetHashCode(PolicyType))*37 +
                       SafeGetHashCode(BuildKey);
            }

            public static bool operator ==(PolicyKey left, PolicyKey right)
            {
                return left.PolicyType == right.PolicyType &&
                    Equals(left.BuildKey, right.BuildKey);
            }

            public static bool operator !=(PolicyKey left, PolicyKey right)
            {
                return !(left == right);
            }

            private static int SafeGetHashCode(object obj)
            {
                return obj != null ? obj.GetHashCode() : 0;
            }
        }
    }
}
