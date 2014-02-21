// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Unity;
namespace Unity.Tests
{
    internal interface IGenerics
    {
    }

    public class ObjectsWithGenerics : IGenerics
    {
        private ObjectsWithGenerics2<String> local;

        [InjectionConstructor]
        public ObjectsWithGenerics(ObjectsWithGenerics2<String> genType)
        {
            local = genType;
        }

        [InjectionMethod]
        public void TestMethod<T>()
        {
            string t = string.Empty;
            if (typeof(T) == typeof(String))
            {
                t = "string";
            }
            else
            {
                t = "int";
            }
        }
    }

    public class ObjectsWithGenerics2<T> : IGenerics
    {
        [InjectionConstructor]
        public ObjectsWithGenerics2()
        {
            string t = string.Empty;

            if (typeof(T) == typeof(String))
            {
                t = "string";
            }
            else
            {
                t = "int";
            }
        }
    }
}