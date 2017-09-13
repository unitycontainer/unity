// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace DifferentInterfacesInDifferentNamespace
{
    public interface IInterface3
    {
    }

    public interface IInterface4
    {
    }

    public class TypeImplementingI3 : IInterface3
    {
        public void MethodInI1()
        {
        }
    }

    public class TypeImplementingI4 : IInterface4
    {
        public void MethodInI2()
        {
        }
    }

    public interface IInterface5
    {
    }

    public interface IInterface6
    {
    }

    public class TypeImplementingI56 : IInterface5, IInterface6
    {
        public void MethodInI1()
        {
        }
    }

    public interface ITypeImplementing8
    {
        void MethodInI1();
    }

    public class TypeImplementing8 : ITypeImplementing8
    {
        public void MethodInI1()
        {
        }
    }
}