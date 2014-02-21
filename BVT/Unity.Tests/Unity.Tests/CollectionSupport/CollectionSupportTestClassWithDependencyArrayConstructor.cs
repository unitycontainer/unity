// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.CollectionSupport
{
    public class CollectionSupportTestClassWithDependencyArrayConstructor
    {
        public CollectionSupportTestClass[] Dependency { get; set; }

        public CollectionSupportTestClassWithDependencyArrayConstructor(CollectionSupportTestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}