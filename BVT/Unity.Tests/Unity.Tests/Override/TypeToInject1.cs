// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Override
{
    public class TypeToInject1 : IInterfaceForTypesToInject
    {
        public TypeToInject1(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
}