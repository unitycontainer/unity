//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;

namespace TestSupport.Unity
{
    public class WrappableThroughInterface : Interface, InterfaceA
    {
        public void Method() { }

        public void Method3() { }

        public void MethodA() { }
    }

    public class WrappableThroughInterfaceWithAttributes : Interface
    {
        [GlobalCountCallHandler(HandlerName = "WrappableThroughInterfaceWithAttributes-Method")]
        public void Method() { }

        [GlobalCountCallHandler(HandlerName = "WrappableThroughInterfaceWithAttributes-Method3")]
        public void Method3() { }
    }

    public interface Interface : InterfaceBase
    {
        void Method();
    }

    public interface InterfaceA
    {
        void MethodA();
    }

    public interface InterfaceBase
    {
        void Method3();
    }

    public class DerivedWrappable : Wrappable
    {
        public void Method4() { }
    }

    public class Wrappable : MarshalByRefObject, Interface, InterfaceA
    {
        public void Method() { }

        public void Method2() { }

        public void Method3() { }

        public void MethodA() { }

        public void MethodRef(ref object foo)
        {
            foo = "foo";
        }

        public void MethodRefValue(ref int foo)
        {
            foo = 42;
        }

        public void MethodOut(out object foo)
        {
            foo = "foo";
        }

        public void MethodOutValue(out int foo)
        {
            foo = 42;
        }
    }

    public class WrappableWithProperty : MarshalByRefObject
    {
        public void Method() { }

        private Wrappable wrappable;

        public Wrappable Wrappable
        {
            get { return wrappable; }
            set { wrappable = value; }
        }

    }
}
