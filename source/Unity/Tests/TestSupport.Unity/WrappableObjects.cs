// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.TestSupport
{
    public class WrappableThroughInterface : Interface, InterfaceA
    {
        public void Method() { }

        public void Method3() { }

        public void MethodA() { }
    }
#if !DNXCORE50
    public class WrappableThroughInterfaceWithAttributes : Interface
    {
        [GlobalCountCallHandler(HandlerName = "WrappableThroughInterfaceWithAttributes-Method")]
        public void Method() { }

        [GlobalCountCallHandler(HandlerName = "WrappableThroughInterfaceWithAttributes-Method3")]
        public void Method3() { }
    }
#endif
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

    public partial class Wrappable : Interface, InterfaceA
    {
        public virtual void Method() { }

        public virtual void Method2() { }

        public virtual void Method3() { }

        public virtual void MethodA() { }

        public virtual void MethodRef(ref object parameter)
        {
            parameter = "parameter";
        }

        public virtual void MethodRefValue(ref int parameter)
        {
            parameter = 42;
        }

        public virtual void MethodOut(out object parameter)
        {
            parameter = "parameter";
        }

        public virtual void MethodOutValue(out int parameter)
        {
            parameter = 42;
        }
    }

    public partial class WrappableWithProperty
    {
        public virtual void Method() { }

        private Wrappable wrappable;

        public virtual Wrappable Wrappable
        {
            get { return wrappable; }
            set { wrappable = value; }
        }
    }
}
