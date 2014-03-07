// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Base class for return of selector policies that need
    /// to keep track of a set of parameter keys.
    /// </summary>
    public class SelectedMemberWithParameters
    {
        private readonly List<IDependencyResolverPolicy> parameterResolvers = new List<IDependencyResolverPolicy>();

        /// <summary>
        /// Adds the parameter resolver.
        /// </summary>
        /// <param name="newKey">The new key.</param>
        public void AddParameterResolver(IDependencyResolverPolicy newKey)
        {
            parameterResolvers.Add(newKey);
        }

        /// <summary>
        /// Gets the parameter resolvers.
        /// </summary>
        /// <returns></returns>
        public IDependencyResolverPolicy[] GetParameterResolvers()
        {
            return parameterResolvers.ToArray();
        }
    }
}

namespace Microsoft.Practices.ObjectBuilder2
{
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
