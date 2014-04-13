// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestObjects
{
    internal class ObjectWithAmbiguousMarkedConstructor
    {
        public ObjectWithAmbiguousMarkedConstructor()
        {
        }

        public ObjectWithAmbiguousMarkedConstructor(int first, string second, float third)
        {
        }

        [InjectionConstructor]
        public ObjectWithAmbiguousMarkedConstructor(string first, string second, int third)
        {
        }
    }
}
