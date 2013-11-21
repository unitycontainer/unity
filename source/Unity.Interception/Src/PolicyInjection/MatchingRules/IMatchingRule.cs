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

using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This interface is implemented by the matching rule classes.
    /// A Matching rule is used to see if a particular policy should
    /// be applied to a class member.
    /// </summary>
    public interface IMatchingRule
    {
        /// <summary>
        /// Tests to see if this rule applies to the given member.
        /// </summary>
        /// <param name="member">Member to test.</param>
        /// <returns>true if the rule applies, false if it doesn't.</returns>
        bool Matches(MethodBase member);
    }
}
