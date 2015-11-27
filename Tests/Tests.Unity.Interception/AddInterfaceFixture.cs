// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Summary description for AddInterfaceFixture
    /// </summary>
    [TestClass]
    public class AddInterfaceFixture
    {
        [TestMethod]
        public void CanProxyWithBehaviorThatAddsInterface()
        {
            var target = new MockDal();
            var proxied = Intercept.ThroughProxy<IDal>(target,
                new InterfaceInterceptor(),
                new[] { new AdditionalInterfaceBehavior() });

            Assert.IsNotNull(proxied);
        }

        [TestMethod]
        public void BehaviorAddsInterface()
        {
            var target = new MockDal();
            var proxied = Intercept.ThroughProxy<IDal>(target,
                new InterfaceInterceptor(),
                new[] { new AdditionalInterfaceBehavior() });

            Assert.IsNotNull(proxied as IAdditionalInterface);
        }

        [TestMethod]
        public void CanInvokeMethodAddedByBehavior()
        {
            var proxied = Intercept.NewInstance<MockDal>(
                new VirtualMethodInterceptor(),
                new[] { new AdditionalInterfaceBehavior() });

            Assert.AreEqual(10, ((IAdditionalInterface)proxied).DoNothing());
        }

        [TestMethod]
        public void CanManuallyAddAdditionalInterface()
        {
            var target = new MockDal();
            var proxied = Intercept.ThroughProxyWithAdditionalInterfaces<IDal>(target,
                new InterfaceInterceptor(),
                new[] { new AdditionalInterfaceBehavior(false) },
                new[] { typeof(IAdditionalInterface) });

            Assert.IsNotNull(proxied as IAdditionalInterface);
        }

        [TestMethod]
        public void CanInvokeMethodOnManuallyAddedInterface()
        {
            var target = new MockDal();
            var proxied = Intercept.ThroughProxyWithAdditionalInterfaces<IDal>(target,
                new InterfaceInterceptor(),
                new[] { new AdditionalInterfaceBehavior(false) },
                new[] { typeof(IAdditionalInterface) });

            Assert.AreEqual(10, ((IAdditionalInterface)proxied).DoNothing());
        }
    }
}
