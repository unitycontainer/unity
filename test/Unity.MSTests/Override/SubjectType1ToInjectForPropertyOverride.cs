// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Unity.Tests.Override
{
    public class SubjectType1ToInjectForPropertyOverride : ISubjectTypeToInjectForPropertyOverride
    {
        public int X { get; set; }
        public string Y { get; set; }
        [Dependency]
        public IInterfaceForTypesToInjectForPropertyOverride InjectedObject { get; set; }
    }
}