// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
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
        public When_ConfiguringPolicies()
            : base("Policies")
        {
        }

        private IUnityContainer GetConfiguredContainer(string containerName)
        {
            IUnityContainer container = new UnityContainer();
            section.Configure(container, containerName);
            return container;
        }

        [TestMethod]
        public void Then_CanConfigureAnEmptyPolicy()
        {
            IUnityContainer container = this.GetConfiguredContainer("oneEmptyPolicy");

            var policies = new List<InjectionPolicy>(container.ResolveAll<InjectionPolicy>());

            Assert.AreEqual(2, policies.Count);
            Assert.IsInstanceOfType(policies[0], typeof(AttributeDrivenPolicy));
            Assert.IsInstanceOfType(policies[1], typeof(RuleDrivenPolicy));
            Assert.AreEqual("policyOne", policies[1].Name);
        }

        [TestMethod]
        public void Then_MatchingRuleInPolicyIsConfigured()
        {
            IUnityContainer container = this.GetConfiguredContainer("policyWithGivenRulesAndHandlersTypes");

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
                = this.GetConfiguredContainer("policyWithExternallyConfiguredRulesAndHandlers");

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
                = this.GetConfiguredContainer("policyWithInjectedRulesAndHandlers");

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
                = this.GetConfiguredContainer("policyWithLifetimeManagedInjectedRulesAndHandlers");

            GlobalCountCallHandler.Calls.Clear();

            container
                .RegisterType<Wrappable>("wrappable",
                    new Interceptor<TransparentProxyInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>());

            Wrappable wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);

            var matchingRuleRegistrations = container.Registrations.Where(r => r.RegisteredType == typeof(IMatchingRule));
            var callHandlerRegistrations = container.Registrations.Where(r => r.RegisteredType == typeof(ICallHandler));

            Assert.AreEqual(2, matchingRuleRegistrations.Count());
            Assert.AreEqual(
                1,
                matchingRuleRegistrations.Where(r => r.LifetimeManagerType == typeof(ContainerControlledLifetimeManager)).Count());
            Assert.AreEqual(
                1,
                matchingRuleRegistrations.Where(r => r.LifetimeManagerType == typeof(TransientLifetimeManager)).Count());

            Assert.AreEqual(2, callHandlerRegistrations.Count());
            Assert.AreEqual(
                1,
                callHandlerRegistrations.Where(r => r.LifetimeManagerType == typeof(ContainerControlledLifetimeManager)).Count());
            Assert.AreEqual(
                1,
                callHandlerRegistrations.Where(r => r.LifetimeManagerType == typeof(TransientLifetimeManager)).Count());
        }

        [TestMethod]
        public void Then_RulesAndHandlersInDifferentPoliciesCanHaveTheSameName()
        {
            IUnityContainer container
                = this.GetConfiguredContainer("policyWithDuplicateRuleAndHandlerNames");

            GlobalCountCallHandler.Calls.Clear();

            container
                .RegisterType<Wrappable>("wrappable",
                    new Interceptor<TransparentProxyInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>());

            var wrappable1 = container.Resolve<Wrappable>("wrappable");
            wrappable1.Method2();
            wrappable1.Method3();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["Method3Handler"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["Method2Handler"]);
        }
    }
}
