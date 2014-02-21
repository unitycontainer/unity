// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;

namespace Unity.Tests.Override
{
    public class SubjectType1ToInject : ISubjectTypeToInject
    {
        [InjectionConstructor]
        public SubjectType1ToInject(IInterfaceForTypesToInject injectedObject)
        {
            InjectedObject = injectedObject;
        }

        public SubjectType1ToInject(int x, string y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public string Y { get; set; }
        public IInterfaceForTypesToInject InjectedObject { get; set; }
    }
}