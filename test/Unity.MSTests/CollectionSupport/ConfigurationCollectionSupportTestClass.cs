// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Tests.CollectionSupport
{
    public class ConfigurationCollectionSupportTestClass
    {
        private CollectionSupportTestClass[] arrayProperty;
        public CollectionSupportTestClass[] ArrayProperty
        {
            get { return arrayProperty; }
            set { arrayProperty = value; }
        }

        private CollectionSupportTestClass[] arrayMethod;
        public CollectionSupportTestClass[] ArrayMethod
        {
            get { return arrayMethod; }
            set { arrayMethod = value; }
        }

        private CollectionSupportTestClass[] arrayCtor;
        public CollectionSupportTestClass[] ArrayCtor
        {
            get { return arrayCtor; }
            set { arrayCtor = value; }
        }

        public void InjectionMethod(CollectionSupportTestClass[] arrayMethod)
        {
            ArrayMethod = arrayMethod;
        }

        [InjectionConstructor]
        public ConfigurationCollectionSupportTestClass()
        { }

        public ConfigurationCollectionSupportTestClass(CollectionSupportTestClass[] arrayCtor)
        {
            ArrayCtor = arrayCtor;
        }
    }
}