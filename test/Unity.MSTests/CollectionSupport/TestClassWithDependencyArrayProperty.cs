// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithDependencyArrayProperty
    {
        [Dependency]
        public TestClass[] Dependency { get; set; }
    }
}