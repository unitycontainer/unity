// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Override
{
    public class TypeToUndergoeTypeBasedInject1 : IForToUndergoeInject
    {
        public TypeToUndergoeTypeBasedInject1(IForTypeToInject injectedObject)
        {
            IForTypeToInject = injectedObject;
        }

        public IForTypeToInject IForTypeToInject { get; set; }
        public TypeToInject1ForTypeOverride TypeToInject1ForTypeOverride { get; set; }
    }
}