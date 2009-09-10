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
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class ConfiguringGeneralizedInterceptionFixture : ConfigurationFixtureBase
    {
        [TestInitialize]
        public void SetUp()
        {
            GlobalCountInterceptionBehavior.Calls.Clear();
        }

        [TestCleanup]
        public void TearDown()
        {
            GlobalCountInterceptionBehavior.Calls.Clear();
        }

        protected override string ConfigFileName
        {
            get { return "InterceptionThroughInjectionMembers"; }
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        public void CanConfigureInterceptorThroughConfigurationFile()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureInterceptorThroughConfigurationFile");

            CallCountInterceptionBehavior callCount = new CallCountInterceptionBehavior();
            container.RegisterType<Interceptable>(new InterceptionBehavior(callCount));

            Interceptable instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, callCount.CallCount);
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        public void CanConfigureAdditionalInterfaceThroughConfigurationFile()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureAdditionalInterfaceThroughConfigurationFile");

            CallCountInterceptionBehavior callCount = new CallCountInterceptionBehavior();
            container.RegisterType<Interceptable>(
                new InterceptionBehavior(callCount),
                new Interceptor(new VirtualMethodInterceptor()));

            Interceptable instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, callCount.CallCount);
            Assert.IsTrue(instance is IServiceProvider);
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        public void CanConfigureInterceptionBehaviorWithTypeThroughConfigurationFile()
        {
            IUnityContainer container =
                GetConfiguredContainer("CanConfigureInterceptionBehaviorWithTypeThroughConfigurationFile");

            container.RegisterType<Interceptable>(new Interceptor(new VirtualMethodInterceptor()));

            Interceptable instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls.Values.First());
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        public void CanConfigureInterceptionBehaviorWithDescriptorTypeThroughConfigurationFile()
        {
            IUnityContainer container =
                GetConfiguredContainer("CanConfigureInterceptionBehaviorWithDescriptorTypeThroughConfigurationFile");

            container.RegisterType<Interceptable>(new Interceptor(new VirtualMethodInterceptor()));

            Interceptable instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls["fixed"]);
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        public void CanConfigureInterceptionBehaviorWithResolvedInstanceThroughConfigurationFile()
        {
            CallCountInterceptionBehavior callCount = new CallCountInterceptionBehavior();

            IUnityContainer container =
                GetConfiguredContainer("CanConfigureInterceptionBehaviorWithResolvedInstanceThroughConfigurationFile");

            container.RegisterType<Interceptable>(new Interceptor(new VirtualMethodInterceptor()));
            container.RegisterInstance<IInterceptionBehavior>("call count", callCount);

            Interceptable instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, callCount.CallCount);
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        public void DeserializingInterceptionBehaviorElementWithNoDataThrows()
        {
            var config = this.OpenConfigFile(this.ConfigFileName);

            try
            {
                Assert.IsNotNull(config.Sections["DeserializingInterceptionBehaviorElementWithNoDataThrows"]);
                Assert.Fail("should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        public void DeserializingInterceptionBehaviorElementWithMultipleDataThrows()
        {
            var config = this.OpenConfigFile(this.ConfigFileName);

            try
            {
                Assert.IsNotNull(config.Sections["DeserializingInterceptionBehaviorElementWithMultipleDataThrows"]);
                Assert.Fail("should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        [ExpectedException(typeof(TypeLoadException))]
        public void InterceptorConfiguredWithNonResolvableTypeThrows()
        {
            IUnityContainer container =
                GetConfiguredContainer("InterceptorConfiguredWithNonResolvableTypeThrows");
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InterceptorConfiguredWithNonCompatibleTypeThrows()
        {
            IUnityContainer container =
                GetConfiguredContainer("InterceptorConfiguredWithNonCompatibleTypeThrows");
        }

        [TestMethod]
        [DeploymentItem("InterceptionThroughInjectionMembers.config")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InterceptorConfiguredWithNonInstantiableTypeThrows()
        {
            IUnityContainer container =
                GetConfiguredContainer("InterceptorConfiguredWithNonInstantiableTypeThrows");
        }
    }

    public class Interceptable
    {
        public virtual int DoSomething()
        {
            return 10;
        }
    }

    public class FixedGlobalCountInterceptionBehaviorDescriptor : IInterceptionBehaviorDescriptor
    {
        public IInterceptionBehavior GetInterceptionBehavior(
            IInterceptor interceptor,
            Type interceptedType,
            Type implementationType,
            IUnityContainer container)
        {
            return new GlobalCountInterceptionBehavior("fixed");
        }
    }

    public class InterceptorWithNoDefaultConstructor : IInstanceInterceptor
    {
        public InterceptorWithNoDefaultConstructor(int ignored)
        {
        }

        public IInterceptingProxy CreateProxy(Type t, object target, params Type[] additionalInterfaces)
        {
            throw new NotImplementedException();
        }

        public bool CanIntercept(Type t)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
        {
            throw new NotImplementedException();
        }
    }

}
