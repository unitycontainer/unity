// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithDependencyTypeConstructor
    {
        public TestClass[] Dependency { get; set; }

        public TestClassWithDependencyTypeConstructor(TestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}