// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Unity.Tests.Override
{
    public interface ISubjectTypeToInjectForPropertyOverride
    {
        int X { get; set; }
        string Y { get; set; }
        [Dependency]
        IInterfaceForTypesToInjectForPropertyOverride InjectedObject { get; set; }
    }
}