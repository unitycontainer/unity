// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Base class for return of selector policies that need
    /// to keep track of a set of parameter resolvers.
    /// </summary>
    public class SelectedMemberWithParameters
    {
        private readonly List<IDependencyResolverPolicy> parameterResolvers = new List<IDependencyResolverPolicy>();

        /// <summary>
        /// Adds the parameter resolver. Resolvers are assumed
        /// to be in the order of the parameters to the member.
        /// </summary>
        /// <param name="newResolver">The new resolver.</param>
        public void AddParameterResolver(IDependencyResolverPolicy newResolver)
        {
            parameterResolvers.Add(newResolver);
        }

        /// <summary>
        /// Gets the parameter resolvers.
        /// </summary>
        /// <returns>An array with the parameter resolvers.</returns>
        public IDependencyResolverPolicy[] GetParameterResolvers()
        {
            return parameterResolvers.ToArray();
        }
    }

    /// <summary>
    /// Base class for return values from selector policies that
    /// return a memberinfo of some sort plus a list of parameter
    /// keys to look up the parameter resolvers.
    /// </summary>
    public class SelectedMemberWithParameters<TMemberInfoType> : SelectedMemberWithParameters
    {
        private TMemberInfoType memberInfo;

        /// <summary>
        /// Construct a new <see cref="SelectedMemberWithParameters{TMemberInfoType}"/>, storing
        /// the given member info.
        /// </summary>
        /// <param name="memberInfo">Member info to store.</param>
        protected SelectedMemberWithParameters(TMemberInfoType memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        /// <summary>
        /// The member info stored.
        /// </summary>
        protected TMemberInfoType MemberInfo
        {
            get { return memberInfo; }
        }
    }
}
