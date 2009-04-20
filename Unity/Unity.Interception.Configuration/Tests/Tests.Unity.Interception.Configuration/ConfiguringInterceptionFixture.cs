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
using System.Configuration;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class ConfiguringInterceptionFixture : ConfigurationFixtureBase
    {
        private const string configFileName = "Interception";

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanAddTheInterceptionExtensionThroughConfiguration()
        {
            IUnityContainer container = GetConfiguredContainer("extensionOnly");

            Assert.IsNotNull(container.Configure<Interception>());
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanAddTheInterceptionExtensionAndExtensionConfigThroughConfiguration()
        {
            IUnityContainer container = GetConfiguredContainer("extensionAndExtensionConfig");

            Assert.IsNotNull(container.Configure<Interception>());
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanConfigureAnEmptyPolicy()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureAnEmptyPolicy");

            List<InjectionPolicy> policies
                = new List<InjectionPolicy>(container.ResolveAll<InjectionPolicy>());

            Assert.AreEqual(2, policies.Count);
            Assert.IsInstanceOfType(policies[0], typeof(AttributeDrivenPolicy));
            Assert.IsInstanceOfType(policies[1], typeof(RuleDrivenPolicy));
            Assert.AreEqual("foo", policies[1].Name);
        }


        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanSetUpSeveralEmptyRules()
        {
            IUnityContainer container = GetConfiguredContainer("CanSetUpSeveralEmptyRules");

            List<InjectionPolicy> policies
                = new List<InjectionPolicy>(container.ResolveAll<InjectionPolicy>());

            Assert.AreEqual(3, policies.Count);
            Assert.IsInstanceOfType(policies[0], typeof(AttributeDrivenPolicy));
            Assert.IsInstanceOfType(policies[1], typeof(RuleDrivenPolicy));
            Assert.AreEqual("foo", policies[1].Name);
            Assert.IsInstanceOfType(policies[2], typeof(RuleDrivenPolicy));
            Assert.AreEqual("bar", policies[2].Name);
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanSetUpAPolicyWithGivenRulesAndHandlersTypes()
        {
            IUnityContainer container = GetConfiguredContainer("CanSetUpAPolicyWithGivenRulesAndHandlersTypes");

            GlobalCountCallHandler.Calls.Clear();

            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>("wrappable", new TransparentProxyInterceptor());

            Wrappable wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["default"]);
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanSetUpAPolicyWithExternallyConfiguredRulesAndHandlers()
        {
            IUnityContainer container
                = GetConfiguredContainer("CanSetUpAPolicyWithExternallyConfiguredRulesAndHandlers");

            GlobalCountCallHandler.Calls.Clear();

            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>("wrappable", new TransparentProxyInterceptor());

            Wrappable wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanSetUpAPolicyWithInjectedRulesAndHandlers()
        {
            IUnityContainer container
                = GetConfiguredContainer("CanSetUpAPolicyWithInjectedRulesAndHandlers");

            GlobalCountCallHandler.Calls.Clear();

            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>("wrappable", new TransparentProxyInterceptor());

            Wrappable wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void CanSetUpAPolicyWithLifetimeManagedInjectedRulesAndHandlers()
        {
            IUnityContainer container
                = GetConfiguredContainer("CanSetUpAPolicyWithLifetimeManagedInjectedRulesAndHandlers");

            GlobalCountCallHandler.Calls.Clear();

            container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>("wrappable", new TransparentProxyInterceptor());

            Wrappable wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);

            Assert.AreSame(
                container.Resolve<IMatchingRule>("rule1"),
                container.Resolve<IMatchingRule>("rule1"));
            Assert.AreNotSame(
                container.Resolve<IMatchingRule>("rule2"),
                container.Resolve<IMatchingRule>("rule2"));
            Assert.AreSame(
                container.Resolve<ICallHandler>("handler1"),
                container.Resolve<ICallHandler>("handler1"));
            Assert.AreNotSame(
                container.Resolve<ICallHandler>("handler2"),
                container.Resolve<ICallHandler>("handler2"));
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void ConfiguringAMatchingRuleWithNoTypeButWithLifetimeManagerThrows()
        {
            System.Configuration.Configuration configuration = this.OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unity-matchingRule-lifetime_but_no_type");

                Assert.Fail("should have thrown while deserializing");
            }
            catch (ConfigurationErrorsException)
            { }
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void ConfiguringAMatchingRuleWithNoTypeButWithInjectionThrows()
        {
            System.Configuration.Configuration configuration = this.OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unity-matchingRule-injection_but_no_type");

                Assert.Fail("should have thrown while deserializing");
            }
            catch (ConfigurationErrorsException)
            { }
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void ConfiguringACallHandlerWithNoTypeButWithLifetimeManagerThrows()
        {
            System.Configuration.Configuration configuration = this.OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unity-callHandler-lifetime_but_no_type");

                Assert.Fail("should have thrown while deserializing");
            }
            catch (ConfigurationErrorsException)
            { }
        }

        [TestMethod]
        [DeploymentItem("Interception.config")]
        public void ConfiguringACallHandlerWithNoTypeButWithInjectionThrows()
        {
            System.Configuration.Configuration configuration = this.OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unity-callHandler-injection_but_no_type");

                Assert.Fail("should have thrown while deserializing");
            }
            catch (ConfigurationErrorsException)
            { }
        }

        public void ThisMethodShouldBeIgnored()
        {
            // this reference is necessary so the VSTS runner will copy the assembly.
            InterceptionConfigurationElement element = new InterceptionConfigurationElement();
            element.Configure(null);
        }

        protected override string ConfigFileName
        {
            get { return configFileName; }
        }
    }
}
