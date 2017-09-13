// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Generics
{
    public interface IHaveManyGenericTypesClosed
    {
        GenericA PropT1 { get; set; }
        GenericB PropT2 { get; set; }
        GenericC PropT3 { get; set; }
        GenericD PropT4 { get; set; }

        void Set(GenericA value);
        void Set(GenericB value);
        void Set(GenericC value);
        void Set(GenericD value);

        void SetMultiple(GenericD t4Value, GenericC t3Value);
    }
}