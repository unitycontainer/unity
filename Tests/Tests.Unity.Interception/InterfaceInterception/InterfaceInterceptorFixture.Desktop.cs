// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.InterfaceInterception
{
    public partial class InterfaceInterceptorFixture
    {
        [TestMethod]
        public void InterceptorCanInterceptProxyInstances()
        {
            CallCountInterceptionBehavior callCounter = new CallCountInterceptionBehavior();

            ProxiedInterfaceImpl impl = new ProxiedInterfaceImpl();
            IProxiedInterface instance = (IProxiedInterface)new MyProxy(typeof(IProxiedInterface), impl).GetTransparentProxy();

            IInstanceInterceptor interceptor = new InterfaceInterceptor();

            IInterceptingProxy proxy = (IInterceptingProxy)interceptor.CreateProxy(typeof(IProxiedInterface), (IProxiedInterface)instance);
            proxy.AddInterceptionBehavior(callCounter);

            IProxiedInterface inter = (IProxiedInterface)proxy;

            Assert.AreEqual("hello world", inter.DoSomething());
            Assert.AreEqual(1, callCounter.CallCount);
        }

        [TestMethod]
        public void CanGetInterceptableMethodsForProxy()
        {
            ProxiedInterfaceImpl impl = new ProxiedInterfaceImpl();
            IProxiedInterface instance = (IProxiedInterface)new MyProxy(typeof(IProxiedInterface), impl).GetTransparentProxy();

            IInstanceInterceptor interceptor = new InterfaceInterceptor();

            var interceptableMethods = interceptor.GetInterceptableMethods(typeof(IProxiedInterface), instance.GetType()).ToArray();

            interceptableMethods.Where(x => x.InterfaceMethodInfo.Name == "DoSomething").AssertHasItems();
        }

        private class MyProxy : RealProxy
        {
            private Type t;
            private object impl;

            public MyProxy(Type t, object impl)
                : base(t)
            {
                this.t = t;
                this.impl = impl;
            }

            public override IMessage Invoke(IMessage msg)
            {
                IMethodCallMessage method = msg as IMethodCallMessage;
                if (method != null)
                {
                    if (method.MethodName == "GetType")
                    {
                        return new ReturnMessage(this.t, new object[0], 0, method.LogicalCallContext, method);
                    }
                    object result = method.MethodBase.Invoke(this.impl, method.InArgs);
                    return new ReturnMessage(result, new object[0], 0, method.LogicalCallContext, method);
                }

                return null;
            }
        }
    }
}
