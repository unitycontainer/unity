// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity.InterceptionExtension;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Unity.Tests.TestObjects;

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for InterceptionFixture
    /// </summary>
    [TestClass]
    public class InterceptionFixture
    {
        [TestMethod]
        public void InterfaceInterceptionWithoutAdditionalInterfaces()
        {
            DoNothingInterceptionBehavior.Reset();
            IInterceptionBehavior[] behaviourArray = new IInterceptionBehavior[1];
            behaviourArray[0] = new DoNothingInterceptionBehavior();
            IInterfaceA target = new ImplementsInterface();
            IInterfaceA proxy = Intercept.ThroughProxy(target, new TransparentProxyInterceptor(), behaviourArray);
            proxy.TargetMethod();
            Assert.AreEqual("Called", DoNothingInterceptionBehavior.PreCalled);
            Assert.AreEqual("Called", DoNothingInterceptionBehavior.PostCalled);
        }

        [TestMethod]
        public void MBROInterceptionWithoutAdditionalInterfaces()
        {
            DoNothingInterceptionBehavior.Reset();
            IInterceptionBehavior[] behaviourArray = new IInterceptionBehavior[1];
            behaviourArray[0] = new DoNothingInterceptionBehavior();
            ImplementsMBRO target = new ImplementsMBRO();
            ImplementsMBRO proxy = Intercept.ThroughProxy(target, new TransparentProxyInterceptor(), behaviourArray);
            proxy.TargetMethod();
            Assert.AreEqual("Called", DoNothingInterceptionBehavior.PreCalled);
            Assert.AreEqual("Called", DoNothingInterceptionBehavior.PostCalled);
        }

        [TestMethod]
        public void VirtualInterceptionWithoutAdditionalInterfaces()
        {
            DoNothingInterceptionBehavior.Reset();
            IInterceptionBehavior[] behaviourArray = new IInterceptionBehavior[1];
            behaviourArray[0] = new DoNothingInterceptionBehavior();

            var proxy = Intercept.NewInstance<HasVirtualMethods>(new VirtualMethodInterceptor(), behaviourArray);
            proxy.TargetMethod();
            Assert.AreEqual("Called", DoNothingInterceptionBehavior.PreCalled);
            Assert.AreEqual("Called", DoNothingInterceptionBehavior.PostCalled);
        }

        [TestMethod]
        public void InterfaceInterceptionWithAdditionalInterfaces()
        {
            IInterceptionBehavior[] behaviourArray = new IInterceptionBehavior[1];
            behaviourArray[0] = new BehavourForAdditionalInterface();
            ITestInterface1 target = new TestClass123();
            var proxy =
                (IAdditionalInterface)
                    Intercept.ThroughProxyWithAdditionalInterfaces(target, new TransparentProxyInterceptor(),
                        behaviourArray, new[] { typeof(IAdditionalInterface) });

            int behaviourReturn = proxy.DoNothing();
            Assert.AreEqual(100, behaviourReturn);
        }

        [TestMethod]
        public void MBROInterceptionWithAdditionalInterfaces()
        {
            IInterceptionBehavior[] behaviourArray = new IInterceptionBehavior[1];
            behaviourArray[0] = new BehavourForAdditionalInterface();
            TestClass123 target = new TestClass123();
            var proxy =
                (IAdditionalInterface)
                    Intercept.ThroughProxyWithAdditionalInterfaces(target, new TransparentProxyInterceptor(),
                        behaviourArray, new[] { typeof(IAdditionalInterface) });
            int behaviourReturn = proxy.DoNothing();
            Assert.AreEqual(100, behaviourReturn);
        }

        [TestMethod]
        public void VirtualInterceptionWithAdditionalInterfaces()
        {
            IInterceptionBehavior[] behaviourArray = new IInterceptionBehavior[1];
            behaviourArray[0] = new BehavourForAdditionalInterface();
            TestClass123 target = new TestClass123();
            var proxy =
                (IAdditionalInterface)
                    Intercept.NewInstanceWithAdditionalInterfaces<TestClass123>(new VirtualMethodInterceptor(),
                        behaviourArray, new[] { typeof(IAdditionalInterface) });
            int behaviourReturn = proxy.DoNothing();
            Assert.AreEqual(100, behaviourReturn);
        }

        [TestMethod]
        public void InterafaceInterceptionWithMultipleBehaviourWillExecuteFalse()
        {
            ITarget objectToIntercept = new MBROTarget();

            ITarget proxy = Intercept.ThroughProxy(objectToIntercept, new InterfaceInterceptor(),
                new IInterceptionBehavior[]
                {
                    new TestBehaviour("behaviour1", true), new TestBehaviour("behaviour2", false),
                    new TestBehaviour("behaviour3", false)
                });

            string result = String.Empty;

            proxy.TargetMethod(ref result);

            Assert.AreEqual("behaviour1preinside targetbehaviour1post", result);
        }

        [TestMethod]
        public void VirtualInterceptionWithMultipleBehaviourWillExecuteFalse()
        {
            var proxy = Intercept.NewInstance<MBROTarget>(new VirtualMethodInterceptor(),
                new IInterceptionBehavior[]
                {
                    new TestBehaviour("behaviour1", false), new TestBehaviour("behaviour2", false),
                    new TestBehaviour("behaviour3", true)
                });

            string result = String.Empty;

            proxy.TargetMethod(ref result);

            Assert.AreEqual("behaviour3preinside targetbehaviour3post", result);
        }

        [TestMethod]
        public void TransparentProxyInterceptionWithMultipleBehaviourWillExecuteFalse()
        {
            MBROTarget objectToIntercept = new MBROTarget();

            MBROTarget proxy = Intercept.ThroughProxy(objectToIntercept, new TransparentProxyInterceptor(),
                new IInterceptionBehavior[]
                {
                    new TestBehaviour("behaviour1", false), new TestBehaviour("behaviour2", true),
                    new TestBehaviour("behaviour3", false)
                });

            string result = String.Empty;

            proxy.TargetMethod(ref result);

            Assert.AreEqual("behaviour2preinside targetbehaviour2post", result);
        }

        [TestMethod]
        public void InterafaceInterceptionWithSingleBehaviourWillExecuteFalse()
        {
            ITarget objectToIntercept = new MBROTarget();

            ITarget proxy = Intercept.ThroughProxy(objectToIntercept, new InterfaceInterceptor(),
                new IInterceptionBehavior[] { new TestBehaviour("behaviour1", false) });

            string result = String.Empty;

            proxy.TargetMethod(ref result);

            Assert.AreEqual("inside target", result);
        }

        [TestMethod]
        public void VirtualInterceptionWithSingleBehaviourWillExecuteFalse()
        {
            var proxy = Intercept.NewInstance<MBROTarget>(new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] { new TestBehaviour("behaviour1", false) });

            string result = String.Empty;

            proxy.TargetMethod(ref result);

            Assert.AreEqual("inside target", result);
        }

        [TestMethod]
        public void TransparentProxyInterceptionWithSingleBehaviourWillExecuteFalse()
        {
            MBROTarget objectToIntercept = new MBROTarget();

            MBROTarget proxy = Intercept.ThroughProxy(objectToIntercept, new TransparentProxyInterceptor(),
                new IInterceptionBehavior[] { new TestBehaviour("behaviour1", false) });

            string result = String.Empty;

            proxy.TargetMethod(ref result);

            Assert.AreEqual("inside target", result);
        }

        [TestMethod]
        public void ResolveTwiceWithConstructorDependencyAndVirtualInterception()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IRepository, TestRepository>(new ContainerControlledLifetimeManager());
            container.Configure<Interception>()
                .SetInterceptorFor(typeof(TestService), new VirtualMethodInterceptor());
            var svc1 = container.Resolve<TestService>();
            Assert.IsNotNull(svc1);

            var svc2 = container.Resolve<TestService>();
            Assert.IsNotNull(svc2);
            Assert.AreNotEqual(svc1, svc2);
        }

        [TestMethod]
        public void ResolveTwiceWithConstructorDependencyAndInterfaceInterception()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IRepository, TestRepository>(new ContainerControlledLifetimeManager());
            container.Configure<Interception>()
                .SetInterceptorFor(typeof(TestService), new TransparentProxyInterceptor());
            var svc1 = container.Resolve<TestService>();
            Assert.IsNotNull(svc1);

            var svc2 = container.Resolve<TestService>();
            Assert.IsNotNull(svc2);
            Assert.AreNotEqual(svc1, svc2);
        }

        [TestMethod]
        public void ResolveTwiceWithConsttructorDependencyAndMBROInterception()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IRepository, TestRepository>().RegisterType<IService, TestService>();
            container.Configure<Interception>()
                .SetInterceptorFor(typeof(IService), new TransparentProxyInterceptor());
            var svc1 = container.Resolve<IService>();
            Assert.IsNotNull(svc1);

            var svc2 = container.Resolve<IService>();
            Assert.IsNotNull(svc2);
            Assert.AreNotEqual(svc1, svc2);
        }

        [TestMethod]
        public void ResolveTwiceWithPropertyDependencyAndVirtualInterception()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IRepository1, TestRepository1>(new ContainerControlledLifetimeManager());
            container.Configure<Interception>()
                .SetInterceptorFor(typeof(TestService1), new VirtualMethodInterceptor());
            var svc1 = container.Resolve<TestService1>();
            Assert.IsNotNull(svc1);
            Assert.IsNotNull(svc1.MyProperty);
            var svc2 = container.Resolve<TestService1>();
            Assert.IsNotNull(svc2);
            Assert.AreNotEqual(svc1, svc2);
            Assert.IsNotNull(svc2.MyProperty);
        }

        [TestMethod]
        public void ResolveTwiceWithPropertyDependencyAndMBROInterception()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IRepository1, TestRepository1>(new ContainerControlledLifetimeManager());
            container.Configure<Interception>()
                .SetInterceptorFor(typeof(TestService1), new TransparentProxyInterceptor());
            var svc1 = container.Resolve<TestService1>();
            Assert.IsNotNull(svc1);
            Assert.IsNotNull(svc1.MyProperty);
            var svc2 = container.Resolve<TestService1>();
            Assert.IsNotNull(svc2);
            Assert.AreNotEqual(svc1, svc2);
            Assert.IsNotNull(svc2.MyProperty);
        }

        [TestMethod]
        public void ResolveTwiceWithPropertyDependencyAndInterfaceInterception()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IRepository1, TestRepository1>().RegisterType<IService1, TestService1>();
            container.Configure<Interception>()
                .SetInterceptorFor(typeof(IService1), new TransparentProxyInterceptor());
            var svc1 = container.Resolve<IService1>();
            Assert.IsNotNull(svc1);
            Assert.IsNotNull(svc1);
            var svc2 = container.Resolve<IService1>();
            Assert.IsNotNull(svc2);
            Assert.AreNotEqual(svc1, svc2);
            Assert.IsNotNull(svc2.MyProperty);
        }

        public interface IRepository
        {
        }

        public class TestRepository : IRepository
        {
        }

        public interface IService
        {
        }

        public class TestService : MarshalByRefObject, IService
        {
            public TestService(IRepository repository)
            {
            }
        }

        public interface IRepository1
        {
        }

        public class TestRepository1 : IRepository1
        {
        }

        public interface IService1
        {
            TestRepository1 MyProperty { get; set; }
        }

        public class TestService1 : MarshalByRefObject, IService1
        {
            [Dependency]
            public TestRepository1 MyProperty { get; set; }
        }

        public interface ITarget
        {
            void TargetMethod(ref string result);
        }

        public class MBROTarget : MarshalByRefObject, ITarget
        {
            public virtual void TargetMethod(ref string result)
            {
                result = result + "inside target";
            }
        }

        public class TestBehaviour : IInterceptionBehavior
        {
            #region IInterceptionBehavior Members

            public string BehaviourName = null;

            public bool ToExecute = true;

            public TestBehaviour(string behaviourName, bool willExecute)
            {
                BehaviourName = behaviourName;

                ToExecute = willExecute;
            }

            IMethodReturn IInterceptionBehavior.Invoke(IMethodInvocation input,
                GetNextInterceptionBehaviorDelegate getNext)
            {
                string preParam = (string)input.Arguments[0];

                preParam = preParam + BehaviourName + "pre";

                input.Arguments[0] = preParam;

                IMethodReturn returnValue = getNext()(input, getNext);

                if (returnValue.Exception == null)
                {
                    string postParam = (string)returnValue.Outputs[0];

                    postParam = postParam + BehaviourName + "post";

                    returnValue.Outputs[0] = postParam;
                }

                return returnValue;
            }

            IEnumerable<Type> IInterceptionBehavior.GetRequiredInterfaces()
            {
                return Type.EmptyTypes;
            }

            bool IInterceptionBehavior.WillExecute
            {
                get { return ToExecute; }
            }

            #endregion
        }
    }
}