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
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    [TestClass]
    public class InterceptionConfigurationFixture
    {
        [TestMethod]
        public void CanSetUpInterceptorThroughInjectionMember()
        {
            CallCountHandler handler = new CallCountHandler();

            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.Configure<Interception>()
                .AddPolicy("policy")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler(handler);

            container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor(new InterfaceInterceptor()),
                new InterceptionBehavior(new PolicyInjectionBehaviorDescriptor()),
                new InterceptionBehavior(new SomeOtherInterceptorDescriptor()));

            IInterface instance = container.Resolve<IInterface>("test");

            instance.DoSomething("1");

            Assert.AreEqual(1, handler.CallCount);
        }

        [TestMethod]
        public void CanSetUpInterceptorThroughInjectionMemberForExistingInterceptor()
        {
            CallCountInterceptionBehavior interceptionBehavior = new CallCountInterceptionBehavior();

            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor(new InterfaceInterceptor()),
                new InterceptionBehavior(interceptionBehavior));

            IInterface instance = container.Resolve<IInterface>("test");

            instance.DoSomething("1");

            Assert.AreEqual(1, interceptionBehavior.CallCount);
        }

        [TestMethod]
        public void CanSetUpAdditionalInterfaceThroughInjectionMemberForInstanceInterception()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            int invokeCount = 0;

            container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor(new InterfaceInterceptor()),
                new AdditionalInterface(typeof(IOtherInterface)),
                new InterceptionBehavior(
                    new DelegateInterceptionBehavior(
                        (mi, gn) => { invokeCount++; return mi.CreateMethodReturn(0); })));

            IInterface instance = container.Resolve<IInterface>("test");

            ((IOtherInterface)instance).DoSomethingElse("1");

            Assert.AreEqual(1, invokeCount);
        }

        [TestMethod]
        public void CanSetUpAdditionalInterfaceThroughInjectionMemberForTypeInterception()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            int invokeCount = 0;

            container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor(new VirtualMethodInterceptor()),
                new AdditionalInterface(typeof(IOtherInterface)),
                new InterceptionBehavior(
                    new DelegateInterceptionBehavior(
                        (mi, gn) => { invokeCount++; return mi.CreateMethodReturn(0); })));

            IInterface instance = container.Resolve<IInterface>("test");

            ((IOtherInterface)instance).DoSomethingElse("1");

            Assert.AreEqual(1, invokeCount);
        }

        [TestMethod]
        public void CanSetUpAdditionalInterfaceThroughGenericInjectionMemberForTypeInterception()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            int invokeCount = 0;

            container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor(new VirtualMethodInterceptor()),
                new AdditionalInterface<IOtherInterface>(),
                new InterceptionBehavior(
                    new DelegateInterceptionBehavior(
                        (mi, gn) => { invokeCount++; return mi.CreateMethodReturn(0); })));

            IInterface instance = container.Resolve<IInterface>("test");

            ((IOtherInterface)instance).DoSomethingElse("1");

            Assert.AreEqual(1, invokeCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfiguringAnAdditionalInterfaceWithANonInterfaceTypeThrows()
        {
            new AdditionalInterface(typeof(int));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfiguringAnAdditionalInterfaceWithANullTypeThrows()
        {
            new AdditionalInterface(null);
        }

        public class BaseClass : IInterface
        {
            public int DoSomething(string param)
            {
                return int.Parse(param);
            }

            public int Property
            {
                get;
                set;
            }
        }

        public interface IInterface
        {
            int DoSomething(string param);
            int Property { get; set; }
        }

        public interface IOtherInterface
        {
            int DoSomethingElse(string param);
        }

        public class SomeOtherInterceptorDescriptor : IInterceptionBehaviorDescriptor
        {
            public IInterceptionBehavior GetInterceptionBehavior(IInterceptor interceptor, Type interceptedType, Type implementationType, IUnityContainer container)
            {
                return null;
            }
        }
    }
}
