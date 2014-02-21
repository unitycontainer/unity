// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

// Order of these class definitions seem to matter for the test.
namespace RegistrationByConventionAssembly1
{
    public interface ITypeImplementingI1
    {
        void MethodInI1();
    }

    public interface IInterface2
    {
        void MethodInI2();
    }

    public class TypeImplementingI1 : ITypeImplementingI1
    {
        public void MethodInI1()
        {
        }
    }

    public class TypeWithNoInterface
    {
        public void MethodInI1()
        {
        }
    }

    public class TypeImplementingI2 : IInterface2
    {
        public void MethodInI2()
        {
        }
    }

    public class TypeImplementingI12 : ITypeImplementingI1, IInterface2
    {
        public void MethodInI2()
        {
            throw new NotImplementedException();
        }

        public void MethodInI1()
        {
            throw new NotImplementedException();
        }
    }
}