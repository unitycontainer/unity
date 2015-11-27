// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Unity.Utility;

namespace Unity.InterceptionExtension
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
