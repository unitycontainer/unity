// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;

namespace Unity.Tests.CollectionSupport
{
    public class CollectionSupportTestClassWithDependencyArrayProperty
    {
        [Dependency]
        public CollectionSupportTestClass[] Dependency { get; set; }
    }
}