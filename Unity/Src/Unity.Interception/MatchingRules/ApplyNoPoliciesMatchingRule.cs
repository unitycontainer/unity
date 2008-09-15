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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="IMatchingRule"/> implementation that fails to match
    /// if the method in question has the ApplyNoPolicies attribute on it.
    /// </summary>
    internal class ApplyNoPoliciesMatchingRule : IMatchingRule
    {
        /// <summary>
        /// Check if the <paramref name="member"/> matches this rule.
        /// </summary>
        /// <remarks>This rule returns true if the member does NOT have the <see cref="ApplyNoPoliciesAttribute"/>
        /// on it, or a containing type doesn't have the attribute.</remarks>
        /// <param name="member">Member to check.</param>
        /// <returns>True if the rule matches, false if it doesn't.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            bool hasNoPoliciesAttribute =
                (member.GetCustomAttributes(typeof(ApplyNoPoliciesAttribute), false).Length != 0);

            hasNoPoliciesAttribute |=
                (member.DeclaringType.GetCustomAttributes(typeof(ApplyNoPoliciesAttribute), false).
                    Length != 0);
            return !hasNoPoliciesAttribute;
        }
    }
}
