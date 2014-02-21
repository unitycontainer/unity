// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Unity;

namespace Unity.Tests.CollectionSupport
{
    public class CollectionSupportTestClassWithDependencyTypeMethod
    {
        public CollectionSupportTestClass[] Dependency { get; set; }

        [InjectionMethod]
        public void Injector(CollectionSupportTestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}
