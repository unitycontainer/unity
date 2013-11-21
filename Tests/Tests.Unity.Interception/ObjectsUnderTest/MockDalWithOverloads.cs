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
namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    partial class MockDalWithOverloads
    {
        public int DoSomething(string s)
        {
            return 42; 
        }

        [Tag("NullString")]
        public string DoSomething(int i)
        {
            return ( i * 2 ).ToString();
        }
    }
}
