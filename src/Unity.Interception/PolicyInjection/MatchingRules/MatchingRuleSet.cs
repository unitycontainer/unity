// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// A <cref see="T:MatchingRuleSet"/> is a matching rule that
    /// is a collection of other matching rules. All the contained
    /// rules much match for the set to match.
    /// </summary>
    public class MatchingRuleSet : List<IMatchingRule>, IMatchingRule
    {
        /// <summary>
        /// Tests the given member against the ruleset. The member matches
        /// if all contained rules in the ruleset match against it.
        /// </summary>
        /// <remarks>If the ruleset is empty, then Matches passes since no rules failed.</remarks>
        /// <param name="member">MemberInfo to test.</param>
        /// <returns>true if all contained rules match, false if any fail.</returns>
        public bool Matches(MethodBase member)
        {
            if (this.Count == 0)
            {
                return false;
            }

            foreach (IMatchingRule rule in this)
            {
                if (!rule.Matches(member))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
