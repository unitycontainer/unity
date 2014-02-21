// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using MetadatatokenCollisionAssembly;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.InterceptionExtension
{
    [TestClass]
    public abstract class TransparentProxyInterceptorFixture : InterceptorFixtureBase
    {
        public virtual void CanCreateInstance()
        {
            IUnityContainer container = GetContainer();
            IInterceptionTestClass sut = this.GetSUT(container);

            Assert.IsNotNull(sut);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut));
        }

        public virtual void CanCreateTwoInstanceAndIdentitieAreDifferentWhenTransient()
        {
            IUnityContainer container = GetContainer();
            IInterceptionTestClass sut1 = this.GetSUT(container);
            IInterceptionTestClass sut2 = this.GetSUT(container);

            Assert.IsNotNull(sut1);
            Assert.IsNotNull(sut2);
            Assert.AreNotSame(sut1, sut2);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut1));
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut2));
        }

        /// <remarks>
        /// Relies on the fact that a new proxy is ALWAYS generated
        /// </remarks>
        public virtual void CanCreateTwoInstanceAndIdentitieAreDifferentWhenContainerControlled()
        {
            IUnityContainer container = GetContainer();
            container.RegisterType<InterceptionTestClassMarshalByRefObject>(new ContainerControlledLifetimeManager());
            container.RegisterType<InterceptionTestClassInterface>(new ContainerControlledLifetimeManager());

            IInterceptionTestClass sut1 = this.GetSUT(container);
            IInterceptionTestClass sut2 = this.GetSUT(container);

            Assert.IsNotNull(sut1);
            Assert.IsNotNull(sut2);
            Assert.AreNotSame(sut1, sut2);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut1));
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut2));
        }

        public virtual void CanCastToOtherInterface()
        {
            IUnityContainer container = GetContainer();

            IInterceptionTestClass sut = this.GetSUT(container);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut));

            IInterceptionTestClassEmpty otherInterface = sut as IInterceptionTestClassEmpty;
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(otherInterface));

            switch (this.Mode)
            {
                case TestMode.Interface:
                    Assert.IsNotNull(otherInterface);
                    break;
                case TestMode.MarshalByRefObject:
                    Assert.IsNotNull(otherInterface);
                    break;
            }
        }

        public virtual void CanCastToOtherInterfaceAndCallMethodsOnIt()
        {
            IUnityContainer container = GetContainer();

            IInterceptionTestClass sut = this.GetSUT(container);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut));

            IInterceptionTestClassEmpty otherInterface = sut as IInterceptionTestClassEmpty;
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(otherInterface));
            otherInterface.DoSomething();

            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        public virtual void CanCastToImplementationClass()
        {
            IUnityContainer container = GetContainer();

            IInterceptionTestClass sut = this.GetSUT(container);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut));

            switch (this.Mode)
            {
                case TestMode.Interface:
                    {
                        InterceptionTestClassInterface implementationClass = sut as InterceptionTestClassInterface;
                        Assert.IsNull(implementationClass);
                    }
                    break;
                case TestMode.MarshalByRefObject:
                    {
                        InterceptionTestClassMarshalByRefObject implementationClass = sut as InterceptionTestClassMarshalByRefObject;
                        Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(implementationClass));
                        Assert.IsNotNull(implementationClass);
                    }
                    break;
            }
        }

        public virtual void TestMethodInterception()
        {
            IUnityContainer container = GetContainer();

            IInterceptionTestClass sut = this.GetSUT(container);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut));
            int previousValue = sut.SetAndReturnPreviousValue(5);

            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        public virtual void TestPropertyWriteInterception()
        {
            IUnityContainer container = GetContainer();

            IInterceptionTestClass sut = this.GetSUT(container);
            sut.PropertyTest = 5;

            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        public virtual void TestPropertyReadInterception()
        {
            IUnityContainer container = GetContainer();

            IInterceptionTestClass sut = this.GetSUT(container);
            Assert.IsTrue(System.Runtime.Remoting.RemotingServices.IsTransparentProxy(sut));
            int i = sut.PropertyTest;

            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        protected abstract IInterceptionTestClass GetSUT(IUnityContainer container);

        protected TestMode Mode
        {
            get;
            set;
        }

        public enum TestMode
        {
            MarshalByRefObject,
            Interface
        }

        [TestMethod]
        public void WhenUsingPIABWithOverridenMethods()
        {
            int intercepted = 0;
            var container = new UnityContainer();
            container
                .AddNewExtension<Interception>()
                .RegisterType<ActionCallHandler>()
                .RegisterType<BaseClass>(
                    new Interceptor(new TransparentProxyInterceptor()),
                    new InterceptionBehavior<PolicyInjectionBehavior>())
                .RegisterType<DerivedClass>(new Interceptor(new TransparentProxyInterceptor()),
                    new InterceptionBehavior<PolicyInjectionBehavior>())
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddCallHandler(new ActionCallHandler(() => intercepted++))
                        .AddMatchingRule(new MemberNameMatchingRule("DoSomething"));

            var instance = container.Resolve<BaseClass>();
            var derived = container.Resolve<DerivedClass>();

            Assert.AreEqual(0, intercepted);

            string resultBase = instance.DoSomething();

            Assert.AreEqual("from base", resultBase);

            Assert.AreEqual(1, intercepted);

            string resultDerived = derived.DoSomething();

            Assert.AreEqual("from derived", resultDerived);

            Assert.AreEqual(2, intercepted);
        }

        [TestClass]
        public class MetadatokenTestClass
        {
            [TestMethod]
            public void WhenObjectGetTypeMethodIsCalled()
            {
                int intercepted = 0;
                var container = new UnityContainer();
                container
                    .AddNewExtension<Interception>()
                    .RegisterType<ActionCallHandler>()
                    .RegisterType<InterceptedType>(
                        new Interceptor(new TransparentProxyInterceptor()),
                        new InterceptionBehavior<PolicyInjectionBehavior>())
                    .Configure<Interception>()
                        .AddPolicy("policy")
                            .AddCallHandler(new ActionCallHandler(() => intercepted++))
                            .AddMatchingRule(new MemberNameMatchingRule("MethodX"));

                var instance = container.Resolve<InterceptedType>();

                Assert.AreEqual(0, intercepted);

                instance.MethodX();

                Assert.AreEqual(1, intercepted);

                instance.GetType();

                Assert.AreEqual(1, intercepted);
            }
        }

        public class BaseClass : MarshalByRefObject
        {
            public virtual string DoSomething()
            {
                return "from base";
            }
        }

        public class DerivedClass : BaseClass
        {
            public override string DoSomething()
            {
                return "from derived";
            }
        }

        public class ActionCallHandler : ICallHandler
        {
            private Action action = null;

            public ActionCallHandler(Action action)
            {
                this.action = action;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                action();
                return getNext()(input, getNext);
            }

            public int Order { get; set; }
        }

        [TestClass]
        public class MBRO : TransparentProxyInterceptorFixture
        {
            public MBRO()
            {
                this.Mode = TestMode.MarshalByRefObject;
            }

            [TestMethod]
            public override void CanCreateInstance() { base.CanCreateInstance(); }

            [TestMethod]
            public override void CanCreateTwoInstanceAndIdentitieAreDifferentWhenTransient() { base.CanCreateTwoInstanceAndIdentitieAreDifferentWhenTransient(); }

            [TestMethod]
            public override void CanCreateTwoInstanceAndIdentitieAreDifferentWhenContainerControlled() { base.CanCreateTwoInstanceAndIdentitieAreDifferentWhenContainerControlled(); }

            [TestMethod]
            public override void CanCastToImplementationClass() { base.CanCastToImplementationClass(); }

            [TestMethod]
            public override void CanCastToOtherInterface() { base.CanCastToOtherInterface(); }

            [TestMethod]
            public override void TestMethodInterception() { base.TestMethodInterception(); }

            [TestMethod]
            public override void TestPropertyWriteInterception() { base.TestPropertyWriteInterception(); }

            [TestMethod]
            public override void TestPropertyReadInterception() { base.TestPropertyReadInterception(); }

            [TestMethod]
            public override void CanCastToOtherInterfaceAndCallMethodsOnIt() { base.CanCastToOtherInterfaceAndCallMethodsOnIt(); }

            protected override IUnityContainer GetContainer()
            {
                IUnityContainer container = base.GetContainer();

                container.Configure<Interception>()
                    .SetDefaultInterceptorFor<InterceptionTestClassMarshalByRefObject>(new TransparentProxyInterceptor());

                return container;
            }

            protected override IInterceptionTestClass GetSUT(IUnityContainer container)
            {
                return container.Resolve<InterceptionTestClassMarshalByRefObject>();
            }
        }

        [TestClass]
        public class Interface : TransparentProxyInterceptorFixture
        {
            public Interface()
            {
                this.Mode = TestMode.Interface;
            }

            [TestMethod]
            public override void CanCreateInstance() { base.CanCreateInstance(); }

            [TestMethod]
            public override void CanCreateTwoInstanceAndIdentitieAreDifferentWhenTransient() { base.CanCreateTwoInstanceAndIdentitieAreDifferentWhenTransient(); }

            [TestMethod]
            public override void CanCreateTwoInstanceAndIdentitieAreDifferentWhenContainerControlled() { base.CanCreateTwoInstanceAndIdentitieAreDifferentWhenContainerControlled(); }

            [TestMethod]
            public override void CanCastToImplementationClass() { base.CanCastToImplementationClass(); }

            [TestMethod]
            public override void CanCastToOtherInterface() { base.CanCastToOtherInterface(); }

            [TestMethod]
            public override void TestMethodInterception() { base.TestMethodInterception(); }

            [TestMethod]
            public override void TestPropertyWriteInterception() { base.TestPropertyWriteInterception(); }

            [TestMethod]
            public override void TestPropertyReadInterception() { base.TestPropertyReadInterception(); }

            [TestMethod]
            public override void CanCastToOtherInterfaceAndCallMethodsOnIt() { base.CanCastToOtherInterfaceAndCallMethodsOnIt(); }

            #region Overriden Stuff
            protected override IUnityContainer GetContainer()
            {
                IUnityContainer container = base.GetContainer();

                container.RegisterType<IInterceptionTestClass, InterceptionTestClassInterface>();

                container.Configure<Interception>()
                    .SetDefaultInterceptorFor<IInterceptionTestClass>(new TransparentProxyInterceptor());

                return container;
            }

            protected override IInterceptionTestClass GetSUT(IUnityContainer container)
            {
                return container.Resolve<IInterceptionTestClass>();
            }
            #endregion
        }

        #region Test Classes
        public class InterceptionTestClassMarshalByRefObject : MarshalByRefObject, IInterceptionTestClass, IInterceptionTestClassEmpty
        {
            public int PropertyTest
            {
                get;
                set;
            }

            public int MethodTest
            {
                get;
                set;
            }

            public int SetAndReturnPreviousValue(int newValue)
            {
                int tmp = this.MethodTest;

                this.MethodTest = newValue;

                return tmp;
            }

            public void DoSomething()
            {
            }
        }

        public class TestInterceptableClass : MarshalByRefObject
        {
            public void Test() { }
            public void Test2() { }
        }

        public interface IInterceptionTestClass
        {
            int PropertyTest { get; set; }
            int MethodTest { get; set; }
            int SetAndReturnPreviousValue(int newValue);
        }

        public interface IInterceptionTestClassEmpty
        {
            void DoSomething();
        }

        public class InterceptionTestClassInterface : IInterceptionTestClass, IInterceptionTestClassEmpty
        {
            private int propertyTest;
            public int PropertyTest
            {
                get { return propertyTest; }
                set { propertyTest = value; }
            }

            private int methodTest;
            public int MethodTest
            {
                get { return methodTest; }
                set { methodTest = value; }
            }

            public int SetAndReturnPreviousValue(int newValue)
            {
                int tmp = methodTest;

                methodTest = newValue;

                return tmp;
            }

            public void DoSomething()
            {
            }
        }
        #endregion
    }
}
