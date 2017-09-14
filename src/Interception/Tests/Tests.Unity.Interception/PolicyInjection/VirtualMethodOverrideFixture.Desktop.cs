// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.PolicyInjection
{
    public partial class VirtualMethodOverrideFixture
    {
        [TestMethod]
        public void CanInterceptDoSomethingMethodWithTransparentProxy()
        {
            var container = CreateContainer();
            AddInterceptDoSomethingPolicy(container);
            InterceptWith<DoubleDerivedClass, TransparentProxyInterceptor>(container);

            var log = container.Resolve<Dictionary<string, List<string>>>();

            var target = container.Resolve<DoubleDerivedClass>();
            target.DoSomething();

            AssertOneHandlerCreated(log);
            var handlerId = GetHandlerId(log);

            Assert.IsTrue(log.ContainsKey(handlerId));
            Assert.AreEqual(1, log[handlerId].Count);
        }

        public partial class RootClass : MarshalByRefObject
        {
        }
    }
}
