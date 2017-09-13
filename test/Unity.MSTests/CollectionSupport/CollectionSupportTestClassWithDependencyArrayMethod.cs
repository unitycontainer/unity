// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Tests.CollectionSupport
{
    public class CollectionSupportTestClassWithDependencyArrayMethod
    {
        public CollectionSupportTestClass[] Dependency { get; set; }

        [InjectionMethod]
        public void Injector(CollectionSupportTestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}