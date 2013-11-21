// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestObjects
{
    class ObjectWithAmbiguousConstructors
    {
        public ObjectWithAmbiguousConstructors()
        {
        }

        public ObjectWithAmbiguousConstructors(int first, string second, float third)
        {
        }

        public ObjectWithAmbiguousConstructors(string first, string second, int third)
        {
            
        }
    }
}
