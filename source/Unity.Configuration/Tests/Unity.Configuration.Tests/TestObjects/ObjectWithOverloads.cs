// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
