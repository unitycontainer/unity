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

using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.InterfaceInterception
{
    [TestClass]
    public class ContainerInterfaceInterceptionFixture
    {
        [TestMethod]
        public void CanInterceptInstancesViaTheContainer()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<IDal, MockDal>()
                .Configure<Interception>()
                    .SetInterceptorFor<IDal>(new InterfaceInterceptor())
                    .AddPolicy("AlwaysMatches")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler<CallCountHandler>("callCount", new ContainerControlledLifetimeManager())
                    .Interception
                .Container;

            IDal dal = container.Resolve<IDal>();

            Assert.IsTrue(dal is IInterceptingProxy);

            dal.Deposit(50.0);
            dal.Deposit(65.0);
            dal.Withdraw(15.0);

            CallCountHandler handler = (CallCountHandler)(container.Resolve<ICallHandler>("callCount"));
            Assert.AreEqual(3, handler.CallCount);
        }

        [TestMethod]
        public void CanInterceptOpenGenericInterfaces()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType(typeof(InterfaceInterceptorFixture.IGenericOne<>), typeof(InterfaceInterceptorFixture.GenericImplementationOne<>))
                .Configure<Interception>()
                    .SetInterceptorFor(typeof(InterfaceInterceptorFixture.IGenericOne<>), new InterfaceInterceptor())
                    .AddPolicy("AlwaysMatches")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler<CallCountHandler>("callCount", new ContainerControlledLifetimeManager())
                    .Interception
                .Container;

            InterfaceInterceptorFixture.IGenericOne<decimal> target = container.Resolve<InterfaceInterceptorFixture.IGenericOne<decimal>>();

            decimal result = target.DoSomething(52m);
            Assert.AreEqual(52m, result);
            target.DoSomething(17m);
            target.DoSomething(84.2m);

            CallCountHandler handler = (CallCountHandler)(container.Resolve<ICallHandler>("callCount"));
            Assert.AreEqual(3, handler.CallCount);
        }

    }
}
