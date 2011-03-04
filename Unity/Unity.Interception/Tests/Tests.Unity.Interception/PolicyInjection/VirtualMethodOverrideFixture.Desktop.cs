using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.PolicyInjection
{
    public partial class VirtualMethodOverrideFixture
    {
        [TestMethod]
        public void CallHandlerRunsWhenCallingThroughBaseClassReference()
        {
            var container = CreateContainer();
            container.RegisterType<RootClass, DoubleDerivedClass>();
            AddInterceptDoSomethingPolicy(container);
            // TODO: Works with TP, fails with VM
            InterceptWith<DoubleDerivedClass, TransparentProxyInterceptor>(container);

            var log = container.Resolve<Dictionary<string, List<string>>>();

            var target = container.Resolve<RootClass>();
            var baseRef = (RootClass)target;

            baseRef.DoSomething();

            AssertOneHandlerCreated(log);
            var handlerId = GetHandlerId(log);

            Assert.IsTrue(log.ContainsKey(handlerId));
            Assert.AreEqual(1, log[handlerId].Count);

        }

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
