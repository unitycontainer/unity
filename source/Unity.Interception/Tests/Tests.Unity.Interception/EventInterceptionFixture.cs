// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Test fixture to verify fix of codeplex issue
    /// http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=4608 :
    /// Intercepting event add/remove methods does not work with VirtualMethodInterceptor
    /// or InterfaceInterceptor 
    /// </summary>
    [TestClass]
    public class EventInterceptionFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Setup()
        {
            container = new UnityContainer();
            container.RegisterType<ICallHandler, MyCallHandler>("MyHandler", new ContainerControlledLifetimeManager());
            container.AddNewExtension<Interception>();

            container.Configure<Interception>()
                .AddPolicy("My Policy")
                    .AddMatchingRule(new AlwaysMatchingRule())
                    .AddCallHandler("MyHandler");
        }

        [TestMethod]
        public void CanInterceptEventWithVirtualMethodInterceptor()
        {
            container.RegisterType<MyClass>(
                new Interceptor<VirtualMethodInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            var o = container.Resolve<MyClass>();

            o.E1 += (sender, args) => { };

            var handler = (MyCallHandler)container.Resolve<ICallHandler>("MyHandler");
            Assert.IsTrue(handler.WasCalled);
        }

        [TestMethod]
        public void CanInterceptEventWithInterfaceInterceptor()
        {
            container.RegisterType<IMyInterface, MyClass>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            var o = container.Resolve<IMyInterface>();

            o.E1 += (sender, args) => { };

            var handler = (MyCallHandler)container.Resolve<ICallHandler>("MyHandler");
            Assert.IsTrue(handler.WasCalled);
        }

        public class MyCallHandler : ICallHandler
        {
            public bool WasCalled = false;

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                WasCalled = true;
                return getNext()(input, getNext);
            }

            public int Order { get; set; }
        }

        public interface IMyInterface
        {
            event EventHandler<EventArgs> E1;
        }

        public class MyClass : IMyInterface
        {
            // Disable "event not used" warning - it's a test
#pragma warning disable 67
            public virtual event EventHandler<EventArgs> E1;
#pragma warning restore 67

        }
    }
}
