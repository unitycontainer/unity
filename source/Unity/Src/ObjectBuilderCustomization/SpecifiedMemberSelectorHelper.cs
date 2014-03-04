// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(policies, "policies");
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(parameterValues, "parameterValues");
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(result, "result");

            foreach(InjectionParameterValue parameterValue in parameterValues)
            {
                string key = Guid.NewGuid().ToString();
                var resolver = parameterValue.GetResolverPolicy(typeToBuild);
                policies.Set(resolver, key);
                result.AddParameterResolver(resolver);
            }
        }
    }
}
