// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Runtime.Remoting;
using Unity.Configuration;
using Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Unity.InterceptionExtension.Configuration.Tests.TestObjects;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringInterceptorsThroughContainerElementExtension
    /// </summary>
    [TestClass]
    public class When_ConfiguringInterceptorsThroughContainerElementExtension : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringInterceptorsThroughContainerElementExtension()
            : base("InterceptorsThroughContainerElementExtension")
        {
        }

        private IUnityContainer GetContainer(string containerName)
        {
            return new UnityContainer()
                .LoadConfiguration(section, containerName)
                .Configure<Interception>()
                .AddPolicy("policy")
                .AddMatchingRule<AlwaysMatchingRule>()
                .AddCallHandler<CallCountHandler>()
                .Container;
        }

        [TestMethod]
        public void Then_CanConfigureDefaultInterceptorForType()
        {
            IUnityContainer container = this.GetContainer("configuringDefaultInterceptorForType");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        public void Then_CanConfigureVirtualMethodInterceptor()
        {
            IUnityContainer container = this.GetContainer("configuringDefaultInterceptorForTypeWithVirtualMethodInterceptor");

            var anonymous = container.Resolve<WrappableWithVirtualMethods>();
            var named = container.Resolve<WrappableWithVirtualMethods>("name");

            Assert.AreSame(typeof(WrappableWithVirtualMethods), anonymous.GetType().BaseType);
            Assert.AreSame(typeof(WrappableWithVirtualMethods), named.GetType().BaseType);
        }

        [TestMethod]
        public void Then_CanConfigureInterceptorForType()
        {
            IUnityContainer container = this.GetContainer("configuringInterceptorForType");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsFalse(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        public void Then_CanConfigureInterceptorForTypeAndName()
        {
            IUnityContainer container = this.GetContainer("configuringInterceptorForTypeAndName");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsFalse(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        public void Then_CanConfigureSeveralInterceptors()
        {
            IUnityContainer container = this.GetContainer("configuringSeveralInterceptors");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        public void Then_CanMixDefaultAndNonDefaultInterceptors()
        {
            IUnityContainer container = this.GetContainer("mixingDefaultAndNonDefaultInterceptors");

            var anonymousWrappable = container.Resolve<Wrappable>();
            var namedWrappable = container.Resolve<Wrappable>("name");
            var anonymousWrappableWithProperty
                = container.Resolve<WrappableWithProperty>();
            var namedWrappableWithProperty
                = container.Resolve<WrappableWithProperty>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymousWrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(namedWrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymousWrappableWithProperty));
            Assert.IsFalse(RemotingServices.IsTransparentProxy(namedWrappableWithProperty));
        }

        [TestMethod]
        public void Then_CanMixTransparentProxyAndVirtualMethodInterceptors()
        {
            IUnityContainer container = this.GetContainer("mixingTransparentProxyAndVirtualMethodInterceptors");

            var anonymousWrappable = container.Resolve<Wrappable>();
            var namedWrappable = container.Resolve<Wrappable>("name");
            var anonymousWrappableWrappableWithVirtualMethods
                = container.Resolve<WrappableWithVirtualMethods>();
            var namedWrappableWrappableWithVirtualMethods
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
        public void Then_CanSpecifyInterceptorUsingTypeConverter()
        {
            this.GetContainer("specifyingInterceptorWithTypeConverter");

            Assert.AreEqual("source value", MyTransparentProxyInterceptorTypeConverter.SourceValue);
        }
    }
}
