// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Generics
{
    public class HaveManyGenericTypes<T1, T2, T3, T4> : IHaveManyGenericTypes<T1, T2, T3, T4>
    {
        public HaveManyGenericTypes()
        { }

        public HaveManyGenericTypes(T1 t1Value)
        {
            PropT1 = t1Value;
        }

        public HaveManyGenericTypes(T2 t2Value)
        {
            PropT2 = t2Value;
        }

        public HaveManyGenericTypes(T2 t2Value, T1 t1Value)
        {
            PropT2 = t2Value;
            PropT1 = t1Value;
        }

        private T1 propT1;

        public T1 PropT1
        {
            get { return propT1; }
            set { propT1 = value; }
        }

        private T2 propT2;

        public T2 PropT2
        {
            get { return propT2; }
            set { propT2 = value; }
        }

        private T3 propT3;

        public T3 PropT3
        {
            get { return propT3; }
            set { propT3 = value; }
        }

        private T4 propT4;

        public T4 PropT4
        {
            get { return propT4; }
            set { propT4 = value; }
        }

        public void Set(T1 t1Value)
        {
            PropT1 = t1Value;
        }

        public void Set(T2 t2Value)
        {
            PropT2 = t2Value;
        }

        public void Set(T3 t3Value)
        {
            PropT3 = t3Value;
        }

        public void Set(T4 t4Value)
        {
            PropT4 = t4Value;
        }

        public void SetMultiple(T4 t4Value, T3 t3Value)
        {
            PropT4 = t4Value;
            PropT3 = t3Value;
        }
    }
}