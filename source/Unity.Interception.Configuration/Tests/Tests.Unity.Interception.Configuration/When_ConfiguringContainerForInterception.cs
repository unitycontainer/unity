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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForInterception
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForInterception : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForInterception() : base("InterceptionInjectionMembers")
        {
        }

        protected override void Arrange()
        {
            GlobalCountInterceptionBehavior.Calls.Clear();
            base.Arrange();
        }

        protected override void Teardown()
        {
            GlobalCountInterceptionBehavior.Calls.Clear();
            base.Teardown();
        }

        private IUnityContainer ConfiguredContainer(string containerName)
        {
            return new UnityContainer().LoadConfiguration(Section, containerName);
        }

        [TestMethod]
        public void Then_CanConfigureInterceptorThroughConfigurationFile()
        {
            var container = ConfiguredContainer("configuringInterceptorThroughConfigurationFile");

            var callCount = new CallCountInterceptionBehavior();
            container.RegisterType<Interceptable>(new InterceptionBehavior(callCount));

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, callCount.CallCount);
        }

        [TestMethod]
        public void Then_CanConfigureAdditionalInterfaceThroughConfigurationFile()
        {
            IUnityContainer container = ConfiguredContainer("configuringAdditionalInterfaceThroughConfigurationFile");

            var callCount = new CallCountInterceptionBehavior();
            container.RegisterType<Interceptable>(
                new InterceptionBehavior(callCount),
                new Interceptor(new VirtualMethodInterceptor()));

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, callCount.CallCount);
            Assert.IsTrue(instance is IServiceProvider);
        }

        [TestMethod]
        public void Then_CanConfigureResolvedInterceptionBehavior()
        {
            IUnityContainer container =
                ConfiguredContainer("configuringInterceptionBehaviorWithTypeThroughConfigurationFile");

            container.RegisterType<Interceptable>(new Interceptor(new VirtualMethodInterceptor()));

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls.Values.First());
        }

        [TestMethod]
        public void Then_CanConfigureNamedResolvedBehavior()
        {
            IUnityContainer container = ConfiguredContainer("canConfigureNamedBehavior")
                .RegisterType<Interceptable>(new Interceptor(new VirtualMethodInterceptor()));

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls["fixed"]);
        }

        [TestMethod]
        public void Then_CanConfigureBehaviorWithNameOnly()
        {
            var callCount = new CallCountInterceptionBehavior();

            IUnityContainer container = ConfiguredContainer("canConfigureBehaviorWithNameOnly")
                .RegisterType<Interceptable>(new Interceptor(new VirtualMethodInterceptor()))
                .RegisterInstance<IInterceptionBehavior>("call count", callCount);

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, callCount.CallCount);
        }

        [TestMethod]
        public void Then_CanConfigureDefaultInterceptor()
        {
            IUnityContainer container = ConfiguredContainer("configuringDefaultInterceptor")
                .Configure<Interception>()
                    .AddPolicy("all")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<GlobalCountCallHandler>()
                .Container;

            var instance1 = container.Resolve<Wrappable>();
            var instance2 = container.Resolve<Wrappable>("two");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(instance1));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(instance2));
        }

        [TestMethod]
        public void Then_CanAddInterfaceThroughConfiguredBehavior()
        {
            IUnityContainer container = ConfiguredContainer("addingInterfacesImplicitlyThroughBehavior");

            var instance = container.Resolve<Interceptable>();
            Assert.IsNotNull(instance as IAdditionalInterface);
        }

        [TestMethod]
        public void Then_CanAddInterfaceThroughExplicitConfiguration()
        {
            IUnityContainer container = ConfiguredContainer("addingInterfacesExplicitlyWithBehavior");

            var instance = container.Resolve<Interceptable>();
            Assert.IsNotNull(instance as IAdditionalInterface);
        }

        [TestMethod]
        public void Then_MultipleBehaviorsCanBeConfigured()
        {
            var container = ConfiguredContainer("multipleBehaviorsOnOneRegistration");
            var instance = container.Resolve<Interceptable>();


            instance.DoSomething();

            var countHandler = (CallCountInterceptionBehavior) container.Resolve<IInterceptionBehavior>("fixed");

            Assert.AreEqual(1, countHandler.CallCount);

            Assert.IsNotNull(instance as IAdditionalInterface);
        }
    }
}
