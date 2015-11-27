// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    [TestClass]
    public class InterceptingInterfaceMethodsFixture
    {
        [TestMethod]
        public void ImplicitlyImplementedMethodsAreInterceptedIfVirtual()
        {
            CallCountHandler handler = new CallCountHandler();
            Interesting instance = WireupHelper.GetInterceptedInstance<Interesting>("DoSomethingInteresting", handler);

            instance.DoSomethingInteresting();

            Assert.IsTrue(instance.SomethingWasCalled);
            Assert.AreEqual(1, handler.CallCount);
        }
    }

    public interface IOne
    {
        void DoSomethingInteresting();
    }

    public class Interesting : IOne
    {
        public bool SomethingWasCalled;

        public virtual void DoSomethingInteresting()
        {
            SomethingWasCalled = true;
        }
    }
}
