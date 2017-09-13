// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Generics
{
    public interface IHaveManyGenericTypes<T1, T2, T3, T4>
    {
        T1 PropT1 { get; set; }
        T2 PropT2 { get; set; }
        T3 PropT3 { get; set; }
        T4 PropT4 { get; set; }

        void Set(T1 value);
        void Set(T2 value);
        void Set(T3 value);
        void Set(T4 value);

        void SetMultiple(T4 t4Value, T3 t3Value);
    }
}