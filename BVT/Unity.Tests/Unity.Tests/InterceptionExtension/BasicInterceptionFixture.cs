// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.InterceptionExtension
{
    [TestClass]
    public class BasicInterceptionFixture
    {
        [TestMethod]
        public void AddingTheInterceptionExtensionIsRequiredToExposeInterceptionFunctionality()
        {
            IUnityContainer uc = new UnityContainer();

            Assert.IsNull(uc.Configure<Interception>());

            uc.AddNewExtension<Interception>();

            Assert.IsNotNull(uc.Configure<Interception>());
        }

        [TestMethod]
        public void TheAttributeDrivenPolicyIsPresentByDefault()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            List<InjectionPolicy> list = new List<InjectionPolicy>(uc.ResolveAll<InjectionPolicy>());

            Assert.AreEqual(1, list.Count);
            Assert.AreSame(typeof(AttributeDrivenPolicy), list[0].GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemotingPolicyInjectorRequiresMarshalByRefOrInterfaceToSucceed_FailsIfNotMBROrInterface()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<InterceptionTestClass_NotInterceptable>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        public void TransparentProxyInterceptorRequiresMarshalByRefOrInterfaceToSucceed_SucceedsIfMBR()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<InterceptionTestClass_MBR>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        public void TransparentProxyInterceptorRequiresMarshalByRefOrInterfaceToSucceed_SucceedsIfInterface()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<IInterceptionTestClass_Interface>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TransparentProxyInterceptorRequiresMarshalByRefOrInterfaceToSucceed_FailsIfConcreteClassImplementingAnInterface()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<InterceptionTestClass_Interface>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        public void NoTransparentProxyInterceptionOccursWhenNoPoliciesAreDefined_MBR()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.RegisterType<InterceptionTestClass_MBR>(new ContainerControlledLifetimeManager());

            InterceptionTestClass_MBR beforeInterception = uc.Resolve<InterceptionTestClass_MBR>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<InterceptionTestClass_MBR>(new TransparentProxyInterceptor());

            InterceptionTestClass_MBR afterInterception = uc.Resolve<InterceptionTestClass_MBR>();

            Assert.AreSame(beforeInterception, afterInterception);
            Assert.IsFalse(RemotingServices.IsTransparentProxy(beforeInterception));
            Assert.IsFalse(RemotingServices.IsTransparentProxy(afterInterception));
        }

        [TestMethod]
        public void NoRemotingInterceptionOccursWhenNoPoliciesAreDefined_Interface()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.RegisterType<IInterceptionTestClass_Interface, InterceptionTestClass_Interface>(new ContainerControlledLifetimeManager());

            IInterceptionTestClass_Interface beforeInterception = uc.Resolve<IInterceptionTestClass_Interface>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<IInterceptionTestClass_Interface>(new TransparentProxyInterceptor());

            IInterceptionTestClass_Interface afterInterception = uc.Resolve<IInterceptionTestClass_Interface>();

            Assert.AreSame(beforeInterception, afterInterception);
            Assert.IsFalse(RemotingServices.IsTransparentProxy(beforeInterception));
            Assert.IsFalse(RemotingServices.IsTransparentProxy(afterInterception));
        }

        [TestMethod]
        public void TransparentProxyInterceptionOccursWhenPoliciesAreDefined_MBR()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.RegisterType<InterceptionTestClass_MBR>(new ContainerControlledLifetimeManager());

            InterceptionTestClass_MBR beforeInterception = uc.Resolve<InterceptionTestClass_MBR>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<InterceptionTestClass_MBR>(new TransparentProxyInterceptor())
                .AddPolicy("Always Matches")
                .AddMatchingRule<AlwaysMatchingRule>()
                .AddCallHandler<ConvenienceInterceptionAPI.CallCountHandler>();

            InterceptionTestClass_MBR afterInterception = uc.Resolve<InterceptionTestClass_MBR>();

            Assert.AreNotSame(beforeInterception, afterInterception);
            Assert.IsFalse(RemotingServices.IsTransparentProxy(beforeInterception));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(afterInterception));
        }

        [TestMethod]
        public void TransparentProxyInterceptionOccursWhenPoliciesAreDefined_Interface()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Interception>();

            uc.RegisterType<IInterceptionTestClass_Interface, InterceptionTestClass_Interface>(new ContainerControlledLifetimeManager());

            IInterceptionTestClass_Interface beforeInterception = uc.Resolve<IInterceptionTestClass_Interface>();

            uc.Configure<Interception>()
                .SetDefaultInterceptorFor<IInterceptionTestClass_Interface>(new TransparentProxyInterceptor())
                .AddPolicy("Always Matches")
                .AddMatchingRule<AlwaysMatchingRule>()
                .AddCallHandler<ConvenienceInterceptionAPI.CallCountHandler>();

            IInterceptionTestClass_Interface afterInterception = uc.Resolve<IInterceptionTestClass_Interface>();

            Assert.AreNotSame(beforeInterception, afterInterception);
            Assert.IsFalse(RemotingServices.IsTransparentProxy(beforeInterception));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(afterInterception));
        }

        internal class AlwaysMatchingRule : IMatchingRule
        {
            public bool Matches(MethodBase member)
            {
                return true;
            }
        }

        internal interface IInterceptionTestClass_Interface
        {
            void AnInterfaceMethod();
        }

        internal class InterceptionTestClass_Interface : IInterceptionTestClass_Interface
        {
            void IInterceptionTestClass_Interface.AnInterfaceMethod()
            {
            }
        }

        internal class InterceptionTestClass_MBR : MarshalByRefObject
        {
            public void AMethod()
            {
            }
        }

        internal class InterceptionTestClass_NotInterceptable
        {
        }

        internal class TestCallHandler : ICallHandler
        {
            private int interceptionCount;

            public int InterceptionCount
            {
                get { return interceptionCount; }
                set { interceptionCount = value; }
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                interceptionCount++;
                return getNext.Invoke().Invoke(input, getNext);
            }

            public int Order
            {
                get { return 0; }
                set { }
            }
        }
    }
}