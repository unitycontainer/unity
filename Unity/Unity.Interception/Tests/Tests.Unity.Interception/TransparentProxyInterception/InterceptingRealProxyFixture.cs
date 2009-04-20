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
using System.Reflection;
using System.Runtime.Remoting;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.TransparaentProxyInterception.Tests
{
    [TestClass]
    public class InterceptingRealProxyFixture
    {
        [TestMethod]
        public void CanProxyMBROMethods()
        {
            MBROWithOneMethod original = new MBROWithOneMethod();
            MBROWithOneMethod proxy = new InterceptingRealProxy(original, typeof (MBROWithOneMethod))
                .GetTransparentProxy() as MBROWithOneMethod;

            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        public void ProxyImplementsIInterceptingProxy()
        {
            MBROWithOneMethod original = new MBROWithOneMethod();
            MBROWithOneMethod proxy = new InterceptingRealProxy(original, typeof (MBROWithOneMethod))
                .GetTransparentProxy() as MBROWithOneMethod;

            Assert.IsNotNull(proxy as IInterceptingProxy);
        }

        [TestMethod]
        public void CanInterceptMethodsThroughProxy()
        {
            MethodInfo doSomething = typeof (MBROWithOneMethod).GetMethod("DoSomething");
            CallCountHandler handler = new CallCountHandler();

            MBROWithOneMethod original = new MBROWithOneMethod();
            MBROWithOneMethod intercepted = new InterceptingRealProxy(original, typeof (MBROWithOneMethod))
                .GetTransparentProxy() as MBROWithOneMethod;

            IInterceptingProxy proxy = (IInterceptingProxy) intercepted;
            proxy.SetPipeline(doSomething, new HandlerPipeline(Seq.Collect<ICallHandler>(handler)));

            int result = intercepted.DoSomething(5);

            Assert.AreEqual(5*3, result);
            Assert.AreEqual(1, handler.CallCount);
        }

        [TestMethod]
        public void ProxyMapsInterfaceMethodsToTheirImplementations()
        {
            MethodInfo something = typeof(InterfaceOne).GetMethod("Something");
            MethodInfo somethingImpl = typeof (MBROWithInterface).GetMethod("Something");

            CallCountHandler handler = new CallCountHandler();

            MBROWithInterface original = new MBROWithInterface();
            MBROWithInterface intercepted = new InterceptingRealProxy(original, typeof(MBROWithOneMethod))
                .GetTransparentProxy() as MBROWithInterface;

            HandlerPipeline pipeline = new HandlerPipeline(Seq.Collect<ICallHandler>(handler));
            IInterceptingProxy proxy = (IInterceptingProxy)intercepted;

            proxy.SetPipeline(something, pipeline);
            HandlerPipeline implPipeline = proxy.GetPipeline(somethingImpl);

            Assert.AreSame(pipeline, implPipeline);
        }

        
        internal class MBROWithOneMethod : MarshalByRefObject
        {
            public int DoSomething(int i)
            {
                return i*3;
            }
        }

        internal interface InterfaceOne
        {
            void Something();
        }

        internal class MBROWithInterface : MarshalByRefObject, InterfaceOne
        {
            public void Something()
            {
                
            }
        }
    }
}
