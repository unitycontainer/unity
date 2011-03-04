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

using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
