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

using System.Runtime.Remoting;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
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
                .LoadConfiguration(Section, containerName)
                .Configure<Interception>()
                .AddPolicy("policy")
                .AddMatchingRule<AlwaysMatchingRule>()
                .AddCallHandler<CallCountHandler>()
                .Container;
        }

        [TestMethod]
        public void Then_CanConfigureDefaultInterceptorForType()
        {
            IUnityContainer container = GetContainer("configuringDefaultInterceptorForType");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }


        [TestMethod]
        public void Then_CanConfigureVirtualMethodInterceptor()
        {
            IUnityContainer container = GetContainer("configuringDefaultInterceptorForTypeWithVirtualMethodInterceptor");

            var anonymous = container.Resolve<WrappableWithVirtualMethods>();
            var named = container.Resolve<WrappableWithVirtualMethods>("name");

            Assert.AreSame(typeof (WrappableWithVirtualMethods), anonymous.GetType().BaseType);
            Assert.AreSame(typeof (WrappableWithVirtualMethods), named.GetType().BaseType);
        }

        [TestMethod]
        public void Then_CanConfigureInterceptorForType()
        {
            IUnityContainer container = GetContainer("configuringInterceptorForType");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsFalse(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        public void Then_CanConfigureInterceptorForTypeAndName()
        {
            IUnityContainer container = GetContainer("configuringInterceptorForTypeAndName");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsFalse(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        public void Then_CanConfigureSeveralInterceptors()
        {
            IUnityContainer container = GetContainer("configuringSeveralInterceptors");

            var anonymous = container.Resolve<Wrappable>();
            var named = container.Resolve<Wrappable>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymous));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(named));
        }

        [TestMethod]
        public void Then_CanMixDefaultAndNonDefaultInterceptors()
        {
            IUnityContainer container = GetContainer("mixingDefaultAndNonDefaultInterceptors");

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
            IUnityContainer container = GetContainer("mixingTransparentProxyAndVirtualMethodInterceptors");

            var anonymousWrappable = container.Resolve<Wrappable>();
            var namedWrappable = container.Resolve<Wrappable>("name");
            var anonymousWrappableWrappableWithVirtualMethods
                = container.Resolve<WrappableWithVirtualMethods>();
            var namedWrappableWrappableWithVirtualMethods
                = container.Resolve<WrappableWithVirtualMethods>("name");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(anonymousWrappable));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(namedWrappable));
            Assert.AreSame(
                typeof (WrappableWithVirtualMethods),
                anonymousWrappableWrappableWithVirtualMethods.GetType());
            Assert.AreSame(
                typeof (WrappableWithVirtualMethods),
                namedWrappableWrappableWithVirtualMethods.GetType().BaseType);
        }

        [TestMethod]
        public void Then_CanSpecifyInterceptorUsingTypeConverter()
        {
            GetContainer("specifyingInterceptorWithTypeConverter");

            Assert.AreEqual("source value", MyTransparentProxyInterceptorTypeConverter.sourceValue);
        }
    }
}
