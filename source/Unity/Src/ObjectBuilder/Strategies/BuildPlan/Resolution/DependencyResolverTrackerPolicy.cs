// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace ObjectBuilder2
{
    /// <summary>
    /// Implementation of <see cref="IDependencyResolverTrackerPolicy"/>.
    /// </summary>
    public class DependencyResolverTrackerPolicy : IDependencyResolverTrackerPolicy
    {
        private List<object> keys = new List<object>();

        /// <summary>
        /// Add a new resolver to track by key.
        /// </summary>
        /// <param name="key">Key that was used to add the resolver to the policy set.</param>
        public void AddResolverKey(object key)
        {
            lock (this.keys)
            {
                keys.Add(key);
            }
        }

        /// <summary>
        /// Remove the currently tracked resolvers from the given policy list.
        /// </summary>
        /// <param name="policies">Policy list to remove the resolvers from.</param>
        public void RemoveResolvers(IPolicyList policies)
        {
            var allKeys = new List<object>();
            lock (this.keys)
            {
                allKeys.AddRange(this.keys);
                keys.Clear();
            }

            foreach (object key in allKeys)
            {
                policies.Clear<IDependencyResolverPolicy>(key);
            }
        }

        // Helper methods for adding and removing the tracker policy.

        /// <summary>
        /// Get an instance that implements <see cref="IDependencyResolverTrackerPolicy"/>,
        /// either the current one in the policy set or creating a new one if it doesn't
        /// exist.
        /// </summary>
        /// <param name="policies">Policy list to look up from.</param>
        /// <param name="buildKey">Build key to track.</param>
        /// <returns>The resolver tracker.</returns>
        public static IDependencyResolverTrackerPolicy GetTracker(IPolicyList policies, object buildKey)
        {
            IDependencyResolverTrackerPolicy tracker =
                policies.Get<IDependencyResolverTrackerPolicy>(buildKey);
            if (tracker == null)
            {
                tracker = new DependencyResolverTrackerPolicy();
                policies.Set<IDependencyResolverTrackerPolicy>(tracker, buildKey);
            }
            return tracker;
        }

        /// <summary>
        /// Add a key to be tracked to the current tracker.
        /// </summary>
        /// <param name="policies">Policy list containing the resolvers and trackers.</param>
        /// <param name="buildKey">Build key for the resolvers being tracked.</param>
        /// <param name="resolverKey">Key for the resolver.</param>
        public static void TrackKey(IPolicyList policies, object buildKey, object resolverKey)
        {
            IDependencyResolverTrackerPolicy tracker = GetTracker(policies, buildKey);
            tracker.AddResolverKey(resolverKey);
        }

        /// <summary>
        /// Remove the resolvers for the given build key.
        /// </summary>
        /// <param name="policies">Policy list containing the build key.</param>
        /// <param name="buildKey">Build key.</param>
        public static void RemoveResolvers(IPolicyList policies, object buildKey)
        {
            IDependencyResolverTrackerPolicy tracker = policies.Get<IDependencyResolverTrackerPolicy>(buildKey);
            if (tracker != null)
            {
                tracker.RemoveResolvers(policies);
            }
        }
    }
}
