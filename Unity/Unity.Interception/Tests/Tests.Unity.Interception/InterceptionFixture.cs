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
using System.Runtime.Remoting;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    [TestClass]
    public class InterceptionFixture
    {
        [TestMethod]
        public void CanConfigureRemotingInterceptionOnMBRO()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.Configure<Interception>().SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        public void CanConfigureRemotingInterceptionOnInterface()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.Configure<Interception>().SetInterceptorFor<Interface>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        public void ConfiguringRemotingInterceptionOnNonMBROTypeThrows()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            try
            {
                container.Configure<Interception>()
                    .SetInterceptorFor<WrappableThroughInterface>(new TransparentProxyInterceptor());
                Assert.Fail("Call to SetInjectorFor<T>() should have thrown");
            }
            catch (ArgumentException)
            {
                // expected exception
            }
        }

        [TestMethod]
        public void CanConfigureDefaultRemotingInterceptionOnMBRO()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.Configure<Interception>()
                .SetDefaultInterceptorFor<Wrappable>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        public void CanConfigureDefaultRemotingInterceptionOnInterface()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.Configure<Interception>()
                .SetDefaultInterceptorFor<Interface>(new TransparentProxyInterceptor());
        }

        [TestMethod]
        public void ConfiguringDefaultRemotingInterceptionOnNonMBROTypeThrows()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            try
            {
                container.Configure<Interception>()
                    .SetDefaultInterceptorFor<WrappableThroughInterface>(new TransparentProxyInterceptor());
                Assert.Fail("Call to SetInjectorFor<T>() should have thrown");
            }
            catch (ArgumentException)
            {
                // expected exception
            }
        }

        [TestMethod]
        public void CanCreateWrappedObjectIfInterceptionPolicyIsSet()
        {
            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>();
            Assert.IsNotNull(wrappable);
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable));
        }

        [TestMethod]
        public void CanCreateWrappedObjectIfDefaultInterceptionPolicy()
        {
            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container.Configure<Interception>().SetDefaultInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>();
            Assert.IsNotNull(wrappable);
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable));
        }

        [TestMethod]
        public void CanCreateNamedWrappedObjectIfDefaultInterceptionPolicy()
        {
            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container.Configure<Interception>().SetDefaultInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>("someName");
            var wrappable2 = container.Resolve<Wrappable>("another");
            Assert.IsNotNull(wrappable);
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable2));
        }

        [TestMethod]
        public void CanSetDefaultInterceptionPolicyThroughRegisterType()
        {
            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container.RegisterType<Wrappable>(
                new DefaultInterceptor(new TransparentProxyInterceptor()),
                new DefaultInterceptionBehavior<PolicyInjectionBehavior>());

            var wrappable = container.Resolve<Wrappable>("someName");
            var wrappable2 = container.Resolve<Wrappable>("another");
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable2));
        }

        [TestMethod]
        public void WillNotCreateWrappedObjectIfNoInterceptionPolicyIsSpecified()
        {
            IUnityContainer container = CreateContainer("CanCreateWrappedObject");

            Wrappable wrappable = container.Resolve<Wrappable>();
            Assert.IsNotNull(wrappable);
            Assert.IsFalse(RemotingServices.IsTransparentProxy(wrappable));
        }

        [TestMethod]
        public void AttributeDrivenPolicyIsAddedByDefault()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<Interface, WrappableThroughInterfaceWithAttributes>();
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrappedOverInterface = container.Resolve<Interface>();
            wrappedOverInterface.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["WrappableThroughInterfaceWithAttributes-Method"]);
        }

        [TestMethod]
        public void CanInterceptWrappedObject()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container.Configure<Interception>().SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>();
            wrappable.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObject"]);
        }

        [TestMethod]
        public void CanInterceptWrappedObjectWithRef()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptWrappedObjectWithRef");
            container.Configure<Interception>().SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>();
            object someObj = null;
            wrappable.MethodRef(ref someObj);

            Assert.AreEqual("parameter", someObj);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptWrappedObjectWithRef"]);
        }

        [TestMethod]
        public void CanInterceptWrappedObjectWithValueTypeRef()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptWrappedObjectWithValueTypeRef");
            container.Configure<Interception>().SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>();
            int someObj = 0;
            wrappable.MethodRefValue(ref someObj);

            Assert.AreEqual(42, someObj);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptWrappedObjectWithValueTypeRef"]);
        }

        [TestMethod]
        public void CanInterceptLifetimeManagedWrappedObject()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container
                .RegisterType<Wrappable>(new ContainerControlledLifetimeManager())
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>();
            wrappable.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObject"]);
        }

        [TestMethod]
        public void CanInterceptExistingWrappedObject()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.BuildUp<Wrappable>(new Wrappable());
            wrappable.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObject"]);
        }

        [TestMethod]
        public void CanInterceptNamedWrappedObject()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>("wrappable", new TransparentProxyInterceptor());

            Wrappable wrappable1 = container.Resolve<Wrappable>("wrappable");
            Wrappable wrappable2 = container.Resolve<Wrappable>();
            wrappable1.Method2();
            wrappable2.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObject"]);
        }

        [TestMethod]
        public void CanInterceptCallsToDerivedOfMBRO()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallsToDerivedOfMBRO");
            container
                .Configure<Interception>()
                    .SetInterceptorFor<DerivedWrappable>(new TransparentProxyInterceptor());

            DerivedWrappable wrappable = container.Resolve<DerivedWrappable>();
            wrappable.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallsToDerivedOfMBRO"]);
        }

        [TestMethod]
        public void InterfaceImplementationsOnDerivedClassesAreWrappedMultipleTimes()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("InterfaceImplementationsOnDerivedClassesAreWrappedMultipleTimes");
            container
                .Configure<Interception>()
                    .SetInterceptorFor<DerivedWrappable>(new TransparentProxyInterceptor());

            DerivedWrappable wrappable = container.Resolve<DerivedWrappable>();
            wrappable.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["InterfaceImplementationsOnDerivedClassesAreWrappedMultipleTimes"]);
        }

        [TestMethod]
        public void CanInterceptCallsToMBROOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallsToMBROOverInterface");
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Wrappable wrappable = container.Resolve<Wrappable>();
            ((Interface)wrappable).Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallsToMBROOverInterface"]);
        }

        [TestMethod]
        public void CanInterceptCallsToMappedMBROOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallsToMappedMBROOverInterface");
            container
                .RegisterType<Interface, Wrappable>()
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Interface wrappable = container.Resolve<Interface>();
            wrappable.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverInterface"]);
        }

        [TestMethod]
        public void CanInterceptCallsToMappedMBROOverInterfaceCastedToType()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallsToMappedMBROOverInterfaceCastedToType");
            container
                .RegisterType<Interface, Wrappable>()
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Interface wrappable = container.Resolve<Interface>();
            ((Wrappable)wrappable).Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverInterfaceCastedToType"]);
        }

        [TestMethod]
        public void CanInterceptCallsToLifetimeManagedMappedMBROOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallsToMappedMBROOverInterface");
            container
                .RegisterType<Interface, Wrappable>(new ContainerControlledLifetimeManager())
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Interface wrappable = container.Resolve<Interface>();
            wrappable.Method();
            Wrappable wrappable2 = container.Resolve<Wrappable>();
            wrappable2.Method();

            Assert.AreEqual(2, GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverInterface"]);
        }

        [TestMethod]
        public void CanInterceptCallsToMappedMBROOverInterfaceWithDefaultConfiguredForType()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container
                = CreateContainer("CanInterceptCallsToMappedMBROOverInterfaceWithDefaultConfiguredForType");
            container
                .RegisterType<Interface, Wrappable>()
                .Configure<Interception>()
                    .SetDefaultInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Interface wrappable = container.Resolve<Interface>();
            wrappable.Method();

            Assert.AreEqual(
                1,
                GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverInterfaceWithDefaultConfiguredForType"]);
        }

        [TestMethod]
        public void CanInterceptCallsToMappedMBROOverInterfaceWithDefaultConfiguredForInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container
                = CreateContainer("CanInterceptCallsToMappedMBROOverInterfaceWithDefaultConfiguredForInterface");
            container
                .RegisterType<Interface, Wrappable>()
                .Configure<Interception>()
                    .SetDefaultInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Interface wrappable = container.Resolve<Interface>();
            wrappable.Method();

            Assert.AreEqual(
                1,
                GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverInterfaceWithDefaultConfiguredForInterface"]);
        }

        [TestMethod]
        public void CanInterceptCallsToMappedMBROOverDifferentInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallsToMappedMBROOverDifferentInterface");
            container
                .RegisterType<Interface, Wrappable>()
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            Interface wrappable = container.Resolve<Interface>();
            ((InterfaceA)wrappable).MethodA();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverDifferentInterface"]);
        }

        [TestMethod]
        public void CanCreateWrappedObjectOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObjectOverInterface");
            container
                .RegisterType<Interface, WrappableThroughInterface>()
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrappedOverInterface = container.Resolve<Interface>();
            wrappedOverInterface.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObjectOverInterface"]);
        }

        [TestMethod]
        public void CanCreatLifetimeManagedeWrappedObjectOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObjectOverInterface");
            container
                .RegisterType<Interface, WrappableThroughInterface>(new ContainerControlledLifetimeManager())
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrappedOverInterface = container.Resolve<Interface>();
            wrappedOverInterface.Method();
            WrappableThroughInterface wrapped = container.Resolve<WrappableThroughInterface>();
            wrapped.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObjectOverInterface"]);
        }

        [TestMethod]
        public void CanInterceptExistingWrappedObjectOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObjectOverInterface");
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrappedOverInterface = container.BuildUp<Interface>(new WrappableThroughInterface());
            wrappedOverInterface.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObjectOverInterface"]);
        }

        [TestMethod]
        public void CanInterceptCallFromBaseOfWrappedInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallFromBaseOfWrappedInterface");
            container.RegisterType<Interface, WrappableThroughInterface>();
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrappedOverInterface = container.Resolve<Interface>();
            wrappedOverInterface.Method3();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallFromBaseOfWrappedInterface"]);
        }

        [TestMethod]
        public void CanConfigureInterceptionOnInterfaceToWrapMBRO()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanConfigureInterceptionOnInterfaceToWrapMBRO");
            container.RegisterType<Interface, Wrappable>();
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrapped = container.Resolve<Interface>();
            wrapped.Method3();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanConfigureInterceptionOnInterfaceToWrapMBRO"]);
        }

        [TestMethod]
        public void CanConfigureInterceptionOnInterfaceToWrapNonMBRO()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanConfigureInterceptionOnInterfaceToWrapNonMBRO");
            container.RegisterType<Interface, WrappableThroughInterface>();
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrapped = container.Resolve<Interface>();
            wrapped.Method3();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanConfigureInterceptionOnInterfaceToWrapNonMBRO"]);
        }

        [TestMethod]
        public void CanInterceptCallToMapppedNonMBROThroughDifferentInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanConfigureInterceptionOnInterfaceToWrapNonMBRO");
            container.RegisterType<Interface, WrappableThroughInterface>();
            container
                .Configure<Interception>()
                    .SetInterceptorFor<Interface>(new TransparentProxyInterceptor());

            Interface wrapped = container.Resolve<Interface>();
            ((InterfaceA)wrapped).MethodA();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanConfigureInterceptionOnInterfaceToWrapNonMBRO"]);
        }

        [TestMethod]
        public void CanInterceptMBROWithDependencyOnOtherMBRO()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = new UnityContainer();

            container.AddNewExtension<Interception>();

            container
                .RegisterInstance<IMatchingRule>(
                    "parentRule",
                    new TypeMatchingRule(typeof(WrappableWithProperty)))
                .RegisterInstance<IMatchingRule>(
                    "childRule",
                    new TypeMatchingRule(typeof(Wrappable)))
                .RegisterInstance<ICallHandler>(
                    "parentCallHandler",
                    new GlobalCountCallHandler("parent"))
                .RegisterInstance<ICallHandler>(
                    "childCallHandler",
                    new GlobalCountCallHandler("child"))
                .RegisterType<InjectionPolicy, RuleDrivenPolicy>("parentPolicy",
                    new InjectionConstructor(
                        new ResolvedArrayParameter<IMatchingRule>(
                            new ResolvedParameter<IMatchingRule>("parentRule")),
                        new string[] { "parentCallHandler" }))
                .RegisterType<InjectionPolicy, RuleDrivenPolicy>("childPolicy",
                    new InjectionConstructor(
                        new ResolvedArrayParameter<IMatchingRule>(
                            new ResolvedParameter<IMatchingRule>("childRule")),
                        new string[] { "childCallHandler" }))
                .RegisterType<WrappableWithProperty>(new InjectionProperty("Wrappable"))
                .Configure<Interception>()
                    .SetDefaultInterceptorFor<WrappableWithProperty>(new TransparentProxyInterceptor())
                    .SetDefaultInterceptorFor<Wrappable>(new TransparentProxyInterceptor());

            WrappableWithProperty instance = container.Resolve<WrappableWithProperty>();

            instance.Method();
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["parent"]); // method

            instance.Wrappable.Method();
            Assert.AreEqual(2, GlobalCountCallHandler.Calls["parent"]); // method and getter
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["child"]);
        }

        [TestMethod]
        public void InstanceInterceptionDoesNotReturnProxyWhenNoHandlerAreConfigured()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<IDal, MockDal>()
                .AddNewExtension<Interception>()
                .Configure<Interception>()
                .SetDefaultInterceptorFor<MockDal>(new TransparentProxyInterceptor())
                .Container;

            IDal dal = container.Resolve<IDal>();

            Assert.IsFalse(dal is IInterceptingProxy);
        }

        private IUnityContainer CreateContainer(string globalCallHandlerName)
        {
            IUnityContainer container = new UnityContainer();

            container.AddNewExtension<Interception>();

            container.RegisterInstance<IMatchingRule>(
                "alwaystrue",
                new AlwaysMatchingRule());
            container.RegisterInstance<ICallHandler>(
                "globalCountHandler",
                new GlobalCountCallHandler(globalCallHandlerName));

            container
                .RegisterType<InjectionPolicy, RuleDrivenPolicy>("policy",
                    new InjectionConstructor(
                        new ResolvedArrayParameter<IMatchingRule>(
                            new ResolvedParameter<IMatchingRule>("alwaystrue")),
                        new string[] { "globalCountHandler" }));

            return container;
        }

        [TestMethod]
        public void CanInterceptMethodOnDerivedType()
        {
            GlobalCountCallHandler.Calls.Clear();

            var container = CreateContainer("CanInterceptMethodOnDerivedType");
            container.RegisterType<BaseInterceptable, DerivedInterceptable>();
            container.Configure<Interception>()
                .SetInterceptorFor<DerivedInterceptable>(new TransparentProxyInterceptor());

            var instance = container.Resolve<BaseInterceptable>();

            instance.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptMethodOnDerivedType"]);
        }

        public class BaseInterceptable : MarshalByRefObject
        {
            public virtual void Method()
            {
            }
        }

        public class DerivedInterceptable : BaseInterceptable
        {
            //public override void Method()
            //{
            //}
        }

    }
}
