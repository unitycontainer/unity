// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ObjectBuilder2;

namespace Unity.ObjectBuilder
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
            Unity.Utility.Guard.ArgumentNotNull(policies, "policies");
            Unity.Utility.Guard.ArgumentNotNull(parameterValues, "parameterValues");
            Unity.Utility.Guard.ArgumentNotNull(result, "result");

            foreach (InjectionParameterValue parameterValue in parameterValues)
            {
                var resolver = parameterValue.GetResolverPolicy(typeToBuild);
                result.AddParameterResolver(resolver);
            }
        }
    }
}
