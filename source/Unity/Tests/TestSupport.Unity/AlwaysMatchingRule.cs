// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Microsoft.Practices.Unity.TestSupport
{
    /// <summary>
    /// A simple matching rule class that always matches. Useful when you want
    /// a policy to apply across the board.
    /// </summary>
    public class AlwaysMatchingRule : IMatchingRule
    {
        [InjectionConstructor]
        public AlwaysMatchingRule()
        {
        }

        public bool Matches(MethodBase member)
        {
            return true;
        }
    }
}
