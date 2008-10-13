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
using System.ComponentModel;
using System.Configuration;
using System.Runtime.Remoting;
using TestSupport.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class ConfiguringInterceptorsFixture : ConfigurationFixtureBase
    {
        private const string configFileName = "Interceptors";

        [TestMethod]
        [DeploymentItem("Interceptors.config")]
        public void CanConfigureDefaultInterceptorForType()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureDefaultInterceptorForType");

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
        [DeploymentItem("Interceptors.config")]
        public void CanConfigureDefaultInterceptorForTypeWithVirtualMethodInterceptor()
        {
            IUnityContainer container =
                GetConfiguredContainer("CanConfigureDefaultInterceptorForTypeWithVirtualMethodInterceptor");

            container
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<CallCountHandler>();

            WrappableWithVirtualMethods anonymous = container.Resolve<WrappableWithVirtualMethods>();
            WrappableWithVirtualMethods named = container.Resolve<WrappableWithVirtualMethods>("name");

            Assert.AreSame(typeof(WrappableWithVirtualMethods), anonymous.GetType().BaseType);
            Assert.AreSame(typeof(WrappableWithVirtualMethods), named.GetType().BaseType);
        }

        [TestMethod]
        [DeploymentItem("Interceptors.config")]
        public void CanConfigureInterceptorForType()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureInterceptorForType");

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
        [DeploymentItem("Interceptors.config")]
        public void CanConfigureInterceptorForTypeAndName()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureInterceptorForTypeAndName");

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
        [DeploymentItem("Interceptors.config")]
        public void CanConfigureSeveralInterceptors()
        {
            IUnityContainer container = GetConfiguredContainer("CanConfigureSeveralInterceptors");

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
        [DeploymentItem("Interceptors.config")]
        public void CanMixDefaultAndNonDefaultInterceptors()
        {
            IUnityContainer container = GetConfiguredContainer("CanMixDefaultAndNonDefaultInterceptors");

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

        [TestMethod]
        [DeploymentItem("Interceptors.config")]
        public void CanMixTransparentProxyAndVirtualMethodInterceptors()
        {
            IUnityContainer container = GetConfiguredContainer("CanMixTransparentProxyAndVirtualMethodInterceptors");

            container
                .Configure<Interception>()
                    .AddPolicy("policy")
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<CallCountHandler>();

            Wrappable anonymousWrappable = container.Resolve<Wrappable>();
            Wrappable namedWrappable = container.Resolve<Wrappable>("name");
            WrappableWithVirtualMethods anonymousWrappableWrappableWithVirtualMethods
                = container.Resolve<WrappableWithVirtualMethods>();
            WrappableWithVirtualMethods namedWrappableWrappableWithVirtualMethods
                = container.Resolve<WrappableWithVirtualMethods>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymousWrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(namedWrappable));
            Assert.AreSame(
                typeof(WrappableWithVirtualMethods),
                anonymousWrappableWrappableWithVirtualMethods.GetType());
            Assert.AreSame(
                typeof(WrappableWithVirtualMethods),
                namedWrappableWrappableWithVirtualMethods.GetType().BaseType);
        }

        [TestMethod]
        [DeploymentItem("Interceptors.config")]
        public void CanSpecifyInterceptorWithTypeConverter()
        {
            IUnityContainer container = GetConfiguredContainer("CanSpecifyInterceptorWithTypeConverter");

            Assert.AreEqual("source value", MyTransparentProxyInterceptorTypeConverter.sourceValue);
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

    public class WrappableWithVirtualMethods
    {
        public virtual void Method() { }

        public virtual void Method3() { }
    }

    public class MyTransparentProxyInterceptorTypeConverter : TypeConverter
    {
        public static string sourceValue;

        public override object ConvertFrom(
            ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value)
        {
            sourceValue = (string)value;
            return new TransparentProxyInterceptor();
        }
    }
}
