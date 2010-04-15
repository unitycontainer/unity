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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
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
            var proxied = Intercept.ThroughProxy(target,
                new TransparentProxyInterceptor(),
                new[] { new AdditionalInterfaceBehavior()});

            Assert.IsNotNull(proxied);
        }

        [TestMethod]
        public void BehaviorAddsInterface()
        {
            var target = new MockDal();
            var proxied = Intercept.ThroughProxy(target,
                new TransparentProxyInterceptor(),
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
            var proxied = Intercept.ThroughProxyWithAdditionalInterfaces(target,
                new TransparentProxyInterceptor(),
                new[] {new AdditionalInterfaceBehavior(false)},
                new[] {typeof (IAdditionalInterface)});

            Assert.IsNotNull(proxied as IAdditionalInterface);
        }

        [TestMethod]
        public void CanInvokeMethodOnManuallyAddedInterface()
        {
            var target = new MockDal();
            var proxied = Intercept.ThroughProxyWithAdditionalInterfaces(target,
                new TransparentProxyInterceptor(),
                new[] { new AdditionalInterfaceBehavior(false) },
                new[] { typeof(IAdditionalInterface) });

            Assert.AreEqual(10, ((IAdditionalInterface)proxied).DoNothing());
        }
    }
}
