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

using System.Collections.Generic;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Base class for return of selector policies that need
    /// to keep track of a set of parameter keys.
    /// </summary>
    public class SelectedMemberWithParameters
    {
        private List<string> parameterKeys = new List<string>();

        /// <summary>
        /// Add a new parameter key to this object. Keys are assumed
        /// to be in the order of the parameters to the constructor.
        /// </summary>
        /// <param name="newKey">Key for the next parameter to look up.</param>
        public void AddParameterKey(string newKey)
        {
            parameterKeys.Add(newKey);
        }

        /// <summary>
        /// The set of keys for the constructor parameters.
        /// </summary>
        public string[] GetParameterKeys()
        {
            return parameterKeys.ToArray();
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
