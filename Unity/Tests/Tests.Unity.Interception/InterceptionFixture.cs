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
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
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

            container.Configure<Interception>().SetInjectorFor<Wrappable>(new RemotingPolicyInjector());
        }

        [TestMethod]
        public void CanConfigureRemotingInterceptionOnInterface()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.Configure<Interception>().SetInjectorFor<Interface>(new RemotingPolicyInjector());
        }

        [TestMethod]
        public void ConfiguringRemotingInterceptionOnNonMBROTypeThrows()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            try
            {
                container.Configure<Interception>()
                    .SetInjectorFor<WrappableThroughInterface>(new RemotingPolicyInjector());
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
                .SetDefaultInjectorFor<Wrappable>(new RemotingPolicyInjector());
        }

        [TestMethod]
        public void CanConfigureDefaultRemotingInterceptionOnInterface()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.Configure<Interception>()
                .SetDefaultInjectorFor<Interface>(new RemotingPolicyInjector());
        }

        [TestMethod]
        public void ConfiguringDefaultRemotingInterceptionOnNonMBROTypeThrows()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();

            try
            {
                container.Configure<Interception>()
                    .SetDefaultInjectorFor<WrappableThroughInterface>(new RemotingPolicyInjector());
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
                    .SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

            Wrappable wrappable = container.Resolve<Wrappable>();
            Assert.IsNotNull(wrappable);
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable));
        }

        [TestMethod]
        public void CanCreateWrappedObjectIfDefaultInterceptionPolicy()
        {
            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container.Configure<Interception>().SetDefaultInjectorFor<Wrappable>(new RemotingPolicyInjector());

            Wrappable wrappable = container.Resolve<Wrappable>();
            Assert.IsNotNull(wrappable);
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable));
        }

        [TestMethod]
        public void CanCreateNamedWrappedObjectIfDefaultInterceptionPolicy()
        {
            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container.Configure<Interception>().SetDefaultInjectorFor<Wrappable>(new RemotingPolicyInjector());

            Wrappable wrappable = container.Resolve<Wrappable>("foo");
            Assert.IsNotNull(wrappable);
            Assert.IsTrue(RemotingServices.IsTransparentProxy(wrappable));
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
                    .SetInjectorFor<Interface>(new RemotingPolicyInjector());

            Interface wrappedOverInterface = container.Resolve<Interface>();
            wrappedOverInterface.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["WrappableThroughInterfaceWithAttributes-Method"]);
        }

        [TestMethod]
        public void CanInterceptWrappedObject()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container.Configure<Interception>().SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

            Wrappable wrappable = container.Resolve<Wrappable>();
            wrappable.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanCreateWrappedObject"]);
        }

        [TestMethod]
        public void CanInterceptLifetimeManagedWrappedObject()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObject");
            container
                .RegisterType<Wrappable>(new ContainerControlledLifetimeManager())
                .Configure<Interception>()
                    .SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Wrappable>("wrappable", new RemotingPolicyInjector());

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
                    .SetInjectorFor<DerivedWrappable>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<DerivedWrappable>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

            Interface wrappable = container.Resolve<Interface>();
            wrappable.Method();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverInterface"]);
        }

        [TestMethod]
        public void CanInterceptCallsToLifetimeManagedMappedMBROOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanInterceptCallsToMappedMBROOverInterface");
            container
                .RegisterType<Interface, Wrappable>(new ContainerControlledLifetimeManager())
                .Configure<Interception>()
                    .SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

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
                    .SetDefaultInjectorFor<Wrappable>(new RemotingPolicyInjector());

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
                    .SetDefaultInjectorFor<Wrappable>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Wrappable>(new RemotingPolicyInjector());

            Interface wrappable = container.Resolve<Interface>();
            ((InterfaceA)wrappable).MethodA();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["CanInterceptCallsToMappedMBROOverDifferentInterface"]);
        }

        // TODO add test for explicit interface

        [TestMethod]
        public void CanCreateWrappedObjectOverInterface()
        {
            GlobalCountCallHandler.Calls.Clear();

            IUnityContainer container = CreateContainer("CanCreateWrappedObjectOverInterface");
            container
                .RegisterType<Interface, WrappableThroughInterface>()
                .Configure<Interception>()
                    .SetInjectorFor<Interface>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Interface>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Interface>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Interface>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Interface>(new RemotingPolicyInjector());

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
                    .SetInjectorFor<Interface>(new RemotingPolicyInjector());

            Interface wrapped = container.Resolve<Interface>();
            wrapped.Method3();

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
                .RegisterType<InjectionPolicy, RuleDrivenPolicy>("parentPolicy")
                .RegisterType<InjectionPolicy, RuleDrivenPolicy>("childPolicy")
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<RuleDrivenPolicy>(
                        "parentPolicy",
                        new InjectionConstructor(
                            new ResolvedArrayParameter<IMatchingRule>(
                                new ResolvedParameter<IMatchingRule>("parentRule")),
                            new string[] { "parentCallHandler" }))
                    .ConfigureInjectionFor<RuleDrivenPolicy>(
                        "childPolicy",
                        new InjectionConstructor(
                            new ResolvedArrayParameter<IMatchingRule>(
                                new ResolvedParameter<IMatchingRule>("childRule")),
                            new string[] { "childCallHandler" }))
                    .ConfigureInjectionFor<WrappableWithProperty>(
                        new InjectionProperty("Wrappable"))
                    .Container
                .Configure<Interception>()
                    .SetDefaultInjectorFor<WrappableWithProperty>(new RemotingPolicyInjector())
                    .SetDefaultInjectorFor<Wrappable>(new RemotingPolicyInjector());

            WrappableWithProperty instance = container.Resolve<WrappableWithProperty>();

            instance.Method();
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["parent"]); // method

            instance.Wrappable.Method();
            Assert.AreEqual(2, GlobalCountCallHandler.Calls["parent"]); // method and getter
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["child"]);
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
                .RegisterType<InjectionPolicy, RuleDrivenPolicy>("policy")
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<RuleDrivenPolicy>(
                        "policy",
                        new InjectionConstructor(
                            new ResolvedArrayParameter<IMatchingRule>(
                                new ResolvedParameter<IMatchingRule>("alwaystrue")),
                            new string[] { "globalCountHandler" }));

            return container;
        }

    }

    public class WrappableThroughInterface : Interface
    {
        public void Method() { }

        public void Method3() { }
    }

    public class WrappableThroughInterfaceWithAttributes : Interface
    {
        [GlobalCountCallHandler(HandlerName = "WrappableThroughInterfaceWithAttributes-Method")]
        public void Method() { }

        [GlobalCountCallHandler(HandlerName = "WrappableThroughInterfaceWithAttributes-Method3")]
        public void Method3() { }
    }

    public interface Interface : InterfaceBase
    {
        void Method();
    }

    public interface InterfaceA
    {
        void MethodA();
    }

    public interface InterfaceBase
    {
        void Method3();
    }

    public class DerivedWrappable : Wrappable
    {
        public void Method4() { }
    }

    public class Wrappable : MarshalByRefObject, Interface, InterfaceA
    {
        public void Method() { }

        public void Method2() { }

        public void Method3() { }

        public void MethodA() { }
    }

    public class WrappableWithProperty : MarshalByRefObject
    {
        public void Method() { }

        private Wrappable wrappable;

        public Wrappable Wrappable
        {
            get { return wrappable; }
            set { wrappable = value; }
        }

    }
}
