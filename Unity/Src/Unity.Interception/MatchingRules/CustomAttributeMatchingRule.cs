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
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An implementation of <see cref="IMatchingRule"/> that checks to see if
    /// the member tested has an arbitrary attribute applied.
    /// </summary>
    public class CustomAttributeMatchingRule : IMatchingRule
    {
        private readonly Type attributeType;
        private readonly bool inherited;

        /// <summary>
        /// Constructs a new <see cref="CustomAttributeMatchingRule"/>.
        /// </summary>
        /// <param name="attributeType">Attribute to match.</param>
        /// <param name="inherited">If true, checks the base class for attributes as well.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public CustomAttributeMatchingRule(Type attributeType, bool inherited)
        {
            Guard.ArgumentNotNull(attributeType, "attributeType");
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
            {
                throw new ArgumentException(Resources.ExceptionAttributeNoSubclassOfAttribute, "attributeType");
            }

            this.attributeType = attributeType;
            this.inherited = inherited;
        }

        /// <summary>
        /// Checks to see if the given <paramref name="member"/> matches the rule.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>true if it matches, false if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            object[] attribues = member.GetCustomAttributes(attributeType, inherited);

            return (attribues != null && attribues.Length > 0);
        }
    }
}
