// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Generics
{
    public class HaveManyGenericTypesClosed : IHaveManyGenericTypesClosed
    {
        public HaveManyGenericTypesClosed()
        { }

        public HaveManyGenericTypesClosed(GenericA t1Value)
        {
            PropT1 = t1Value;
        }

        public HaveManyGenericTypesClosed(GenericB t2Value)
        {
            PropT2 = t2Value;
        }

        public HaveManyGenericTypesClosed(GenericB t2Value, GenericA t1Value)
        {
            PropT2 = t2Value;
            PropT1 = t1Value;
        }

        private GenericA propT1;

        public GenericA PropT1
        {
            get { return propT1; }
            set { propT1 = value; }
        }

        private GenericB propT2;

        public GenericB PropT2
        {
            get { return propT2; }
            set { propT2 = value; }
        }

        private GenericC propT3;

        public GenericC PropT3
        {
            get { return propT3; }
            set { propT3 = value; }
        }

        private GenericD propT4;

        public GenericD PropT4
        {
            get { return propT4; }
            set { propT4 = value; }
        }

        public void Set(GenericA t1Value)
        {
            PropT1 = t1Value;
        }

        public void Set(GenericB t2Value)
        {
            PropT2 = t2Value;
        }

        public void Set(GenericC t3Value)
        {
            PropT3 = t3Value;
        }

        public void Set(GenericD t4Value)
        {
            PropT4 = t4Value;
        }

        public void SetMultiple(GenericD t4Value, GenericC t3Value)
        {
            PropT4 = t4Value;
            PropT3 = t3Value;
        }
    }
}
