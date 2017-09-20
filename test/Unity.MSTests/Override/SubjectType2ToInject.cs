// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Unity.Tests.Override
{
    public class SubjectType2ToInject : ISubjectTypeToInject
    {
        [InjectionConstructor]
        public SubjectType2ToInject(IInterfaceForTypesToInject injectedObject)
        {
            InjectedObject = injectedObject;
        }

        public SubjectType2ToInject(int x, string y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public string Y { get; set; }
        public IInterfaceForTypesToInject InjectedObject { get; set; }
    }
}