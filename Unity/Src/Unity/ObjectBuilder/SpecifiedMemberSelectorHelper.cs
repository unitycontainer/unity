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
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.ObjectBuilder
{
    /// <summary>
    /// Helper class for implementing selector policies that need to
    /// set up dependency resolver policies.
    /// </summary>
    public static class SpecifiedMemberSelectorHelper
    {
        /// <summary>
        /// Add dependency resolvers to the parameter set.
        /// </summary>
        /// <param name="typeToBuild">Type that's currently being built (used to resolve open generics).</param>
        /// <param name="policies">PolicyList to add the resolvers to.</param>
        /// <param name="parameterValues">Objects supplying the dependency resolvers.</param>
        /// <param name="result">Result object to store the keys in.</param>
        public static void AddParameterResolvers(Type typeToBuild,
            IPolicyList policies,
            IEnumerable<InjectionParameterValue> parameterValues,
            SelectedMemberWithParameters result)
        {
            foreach(InjectionParameterValue parameterValue in parameterValues)
            {
                string key = Guid.NewGuid().ToString();
                policies.Set<IDependencyResolverPolicy>(parameterValue.GetResolverPolicy(typeToBuild), key);
                result.AddParameterKey(key);
            }
        }
    }
}
