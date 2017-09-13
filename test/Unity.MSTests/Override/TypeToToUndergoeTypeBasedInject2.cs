// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Override
{
    public class TypeToToUndergoeTypeBasedInject2 : IForToUndergoeInject
    {
        public TypeToToUndergoeTypeBasedInject2(TypeToInject2ForTypeOverride injectedObject)
        {
            IForTypeToInject = injectedObject;
        }

        public IForTypeToInject IForTypeToInject { get; set; }
        public TypeToInject2ForTypeOverride TypeToInject2ForTypeOverride { get; set; }
    }
}