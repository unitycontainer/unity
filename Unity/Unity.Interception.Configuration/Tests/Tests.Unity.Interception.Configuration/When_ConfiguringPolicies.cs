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

using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringPolicies
    /// </summary>
    [TestClass]
    public class When_ConfiguringPolicies : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringPolicies() : base("Policies")
        {
        }

        private IUnityContainer GetConfiguredContainer(string containerName)
        {
            IUnityContainer container = new UnityContainer();
            Section.Configure(container, containerName);
            return container;
        }

        [TestMethod]
        public void Then_CanConfigureAnEmptyPolicy()
        {
            IUnityContainer container = GetConfiguredContainer("oneEmptyPolicy");

            var policies = new List<InjectionPolicy>(container.ResolveAll<InjectionPolicy>());

            Assert.AreEqual(2, policies.Count);
            Assert.IsInstanceOfType(policies[0], typeof (AttributeDrivenPolicy));
            Assert.IsInstanceOfType(policies[1], typeof (RuleDrivenPolicy));
            Assert.AreEqual("policyOne", policies[1].Name);
        }

        [TestMethod]
        public void Then_MatchingRuleInPolicyIsConfigured()
        {
            IUnityContainer container = GetConfiguredContainer("policyWithGivenRulesAndHandlersTypes");

            GlobalCountCallHandler.Calls.Clear();

            container.RegisterType<Wrappable>("wrappable",
                new Interceptor<TransparentProxyInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            var wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["default"]);
        }

        [TestMethod]
        public void Then_RulesAndHandlersCanBeConfiguredExternalToPolicy()
        {
            IUnityContainer container
                = GetConfiguredContainer("policyWithExternallyConfiguredRulesAndHandlers");

            GlobalCountCallHandler.Calls.Clear();

            container.RegisterType<Wrappable>("wrappable",
                new Interceptor<TransparentProxyInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            var wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
        }

        [TestMethod]
        public void Then_RulesAndHandlersCanHaveInjectionConfiguredInPolicyElement()
        {
            IUnityContainer container
                = GetConfiguredContainer("policyWithInjectedRulesAndHandlers");

            GlobalCountCallHandler.Calls.Clear();

            container
                .RegisterType<Wrappable>("wrappable",
                    new Interceptor<TransparentProxyInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>());

            var wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithLifetimeManagedInjectedRulesAndHandlers()
        {
            IUnityContainer container
                = GetConfiguredContainer("policyWithLifetimeManagedInjectedRulesAndHandlers");

            GlobalCountCallHandler.Calls.Clear();

            container
                .RegisterType<Wrappable>("wrappable",
                    new Interceptor<TransparentProxyInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>());

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
    }
}
