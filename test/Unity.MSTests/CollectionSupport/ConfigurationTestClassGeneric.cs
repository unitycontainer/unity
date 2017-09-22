// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Tests.CollectionSupport
{
    public class ConfigurationTestClassGeneric<T>
    {
        private T[] arrayProperty;
        public T[] ArrayProperty
        {
            get { return arrayProperty; }
            set { arrayProperty = value; }
        }

        private T[] arrayMethod;
        public T[] ArrayMethod
        {
            get { return arrayMethod; }
            set { arrayMethod = value; }
        }

        private T[] arrayCtor;
        public T[] ArrayCtor
        {
            get { return arrayCtor; }
            set { arrayCtor = value; }
        }

        public void InjectionMethod(T[] arrayMethod)
        {
            ArrayMethod = arrayMethod;
        }

        [InjectionConstructor]
        public ConfigurationTestClassGeneric()
        { }

        public ConfigurationTestClassGeneric(T[] arrayCtor)
        {
            ArrayCtor = arrayCtor;
        }
    }
}