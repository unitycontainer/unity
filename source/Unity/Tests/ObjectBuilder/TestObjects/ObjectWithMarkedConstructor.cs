// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestObjects
{
    internal class ObjectWithMarkedConstructor
    {
        public ObjectWithMarkedConstructor(int notTheInjectionConstructor)
        {
        }

        [InjectionConstructor]
        public ObjectWithMarkedConstructor(string theInjectionConstructor)
        {
        }
    }
}
