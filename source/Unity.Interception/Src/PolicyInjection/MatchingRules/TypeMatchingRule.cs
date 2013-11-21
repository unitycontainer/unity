// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A matching rule that matches when the member is declared
    /// in the given type.
    /// </summary>
    public class TypeMatchingRule : IMatchingRule
    {
        private readonly List<MatchingInfo> matches;
        private readonly bool matchesTypelessMembers;

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that matches the
        /// given type.
        /// </summary>
        /// <param name="type">The type to match.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public TypeMatchingRule(Type type)
            : this(SafeGetTypeName(type), false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that matches types
        /// with the given name.
        /// </summary>
        /// <remarks>Comparisons are case sensitive.</remarks>
        /// <param name="typeName">Type name to match.</param>
        public TypeMatchingRule(string typeName)
            : this(typeName, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that matches types
        /// with the given name, using the given case sensitivity.
        /// </summary>
        /// <param name="typeName">Type name to match.</param>
        /// <param name="ignoreCase">if false, do case-sensitive comparison. If true, do case-insensitive.</param>
        public TypeMatchingRule(string typeName, bool ignoreCase)
            : this(new MatchingInfo[] { new MatchingInfo(typeName, ignoreCase) })
        {
        }

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that will match
        /// any of the type names given in the collection of match information.
        /// </summary>
        /// <param name="matches">The match information to match.</param>
        public TypeMatchingRule(IEnumerable<MatchingInfo> matches)
        {
            this.matches = new List<MatchingInfo>(matches);
            matchesTypelessMembers = this.matches.Exists(delegate(MatchingInfo match)
            {
                return
                    string.IsNullOrEmpty(
                        match.Match);
            });
        }

        /// <summary>
        /// Checks if the given member matches any of this object's matches.
        /// </summary>
        /// <param name="member">Member to match.</param>
        /// <returns>True if match, false if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            if (member.DeclaringType == null)
            {
                return matchesTypelessMembers;
            }
            bool doesMatch = Matches(member.DeclaringType);
            return doesMatch;
        }

        /// <summary>
        /// Checks if the given type matches any of this object's matches.
        /// </summary>
        /// <remarks>Matches may be on the namespace-qualified type name or just the type name.</remarks>
        /// <param name="t">Type to check.</param>
        /// <returns>True if it matches, false if it doesn't.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public bool Matches(Type t)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(t, "t");

            foreach (MatchingInfo match in matches)
            {
                if (string.Compare(t.FullName, match.Match, Comparison(match.IgnoreCase)) == 0)
                {
                    return true;
                }
                else if (string.Compare(t.Name, match.Match, Comparison(match.IgnoreCase)) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static StringComparison Comparison(bool ignoreCase)
        {
            return ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }

        private static string SafeGetTypeName(Type type)
        {
            Guard.ArgumentNotNull(type, "type");
            return type.Name;
        }
    }
}
