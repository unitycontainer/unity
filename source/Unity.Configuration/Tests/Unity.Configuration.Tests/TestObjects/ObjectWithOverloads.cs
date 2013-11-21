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

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    class ObjectWithOverloads
    {
        public int FirstOverloadCalls;
        public int SecondOverloadCalls;


        public void CallMe(int param)
        {
            ++FirstOverloadCalls;
        }

        public void CallMe(string param)
        {
            ++SecondOverloadCalls;
        }
    }
}
