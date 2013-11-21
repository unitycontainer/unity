// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
