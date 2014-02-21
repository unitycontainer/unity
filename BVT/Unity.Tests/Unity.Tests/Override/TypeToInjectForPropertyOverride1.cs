// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Override
{
    public class TypeToInjectForPropertyOverride1 : IInterfaceForTypesToInjectForPropertyOverride
    {
        public TypeToInjectForPropertyOverride1(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
}