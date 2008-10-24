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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
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
