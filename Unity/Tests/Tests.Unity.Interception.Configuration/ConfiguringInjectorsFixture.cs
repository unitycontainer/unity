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
using System.Runtime.Remoting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class ConfiguringInjectorsFixture : ConfigurationFixtureBase
    {
        private const string configFileName = "Injectors";

        [TestMethod]
        [DeploymentItem("Injectors.config")]
        public void CanConfigureDefaultInjectorForType()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureDefaultInjectorForType");

            container
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<CallCountHandler>();

            Wrappable anonymous = container.Resolve<Wrappable>();
            Wrappable named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        [DeploymentItem("Injectors.config")]
        public void CanConfigureInjectorForType()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureInjectorForType");

            container
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<CallCountHandler>();

            Wrappable anonymous = container.Resolve<Wrappable>();
            Wrappable named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsFalse(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        [DeploymentItem("Injectors.config")]
        public void CanConfigureInjectorForTypeAndName()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureInjectorForTypeAndName");

            container
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<CallCountHandler>();

            Wrappable anonymous = container.Resolve<Wrappable>();
            Wrappable named = container.Resolve<Wrappable>("name");

            Assert.IsFalse(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        [DeploymentItem("Injectors.config")]
        public void CanConfigureSeveralInjectors()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureSeveralInjectors");

            container
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<CallCountHandler>();

            Wrappable anonymous = container.Resolve<Wrappable>();
            Wrappable named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        [DeploymentItem("Injectors.config")]
        public void CanMixDefaultAndNonDefaultInjectors()
        {
            IUnityContainer container = GetConfiguredContainer("CanMixDefaultAndNonDefaultInjectors");

            container
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<CallCountHandler>();

            Wrappable anonymousWrappable = container.Resolve<Wrappable>();
            Wrappable namedWrappable = container.Resolve<Wrappable>("name");
            WrappableWithProperty anonymousWrappableWithProperty
                = container.Resolve<WrappableWithProperty>();
            WrappableWithProperty namedWrappableWithProperty
                = container.Resolve<WrappableWithProperty>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymousWrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(namedWrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymousWrappableWithProperty));
            Assert.IsFalse(RemotingServices.IsTransparentProxy(namedWrappableWithProperty));
        }

        public void ThisMethodShouldBeIgnored()
        {
            // this reference is necessary so the VSTS runner will copy the assembly.
            InterceptionConfigurationElement element = new InterceptionConfigurationElement();
            element.Configure(null);
        }

        // no injection if no type
        // no lifetime if no type
        // 

        protected override string ConfigFileName
        {
            get { return configFileName; }
        }
    }
}
