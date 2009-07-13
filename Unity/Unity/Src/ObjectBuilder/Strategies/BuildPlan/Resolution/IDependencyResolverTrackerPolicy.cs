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

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A builder policy that lets you keep track of the current
    /// resolvers and will remove them from the given policy set.
    /// </summary>
    public interface IDependencyResolverTrackerPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Add a new resolver to track by key.
        /// </summary>
        /// <param name="key">Key that was used to add the resolver to the policy set.</param>
        void AddResolverKey(object key);

        /// <summary>
        /// Remove the currently tracked resolvers from the given policy list.
        /// </summary>
        /// <param name="policies">Policy list to remove the resolvers from.</param>
        void RemoveResolvers(IPolicyList policies);
    }
}
