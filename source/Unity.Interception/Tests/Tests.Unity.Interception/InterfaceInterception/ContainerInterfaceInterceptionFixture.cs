// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
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

        [TestMethod]
        public void CanInterceptGenericInterfaceWithConstraints()
        {
            var container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType(typeof (IGenericInterfaceWithConstraints<>), typeof (ImplementsGenericInterface<>),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior(new NoopBehavior()));

            var result = container.Resolve<IGenericInterfaceWithConstraints<MockDal>>();

            Assert.IsNotNull(result as IInterceptingProxy);
        }

        public interface IGenericInterfaceWithConstraints<T>
            where T : class
        {
            void TestMethod1<T1>();

            void TestMethod2<T2>()
                where T2 : struct;

            void TestMethod3<T3>()
                where T3 : class;

            void TestMethod4<T4>()
                where T4 : class, new();

            void TestMethod5<T5>()
                where T5 : InjectionPolicy;

            void TestMethod6<T6>()
                where T6 : IMatchingRule;
        }

        public class ImplementsGenericInterface<T> : IGenericInterfaceWithConstraints<T>
            where T : class
        {
            public void TestMethod1<T1>()
            {
                throw new NotImplementedException();
            }

            public void TestMethod2<T2>() where T2 : struct
            {
                throw new NotImplementedException();
            }

            public void TestMethod3<T3>() where T3 : class
            {
                throw new NotImplementedException();
            }

            public void TestMethod4<T4>() where T4 : class, new()
            {
                throw new NotImplementedException();
            }

            public void TestMethod5<T5>() where T5 : InjectionPolicy
            {
                throw new NotImplementedException();
            }

            public void TestMethod6<T6>() where T6 : IMatchingRule
            {
                throw new NotImplementedException();
            }
        }

        private class NoopBehavior : IInterceptionBehavior
        {
            public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
            {
                return getNext()(input, getNext);
            }

            public IEnumerable<Type> GetRequiredInterfaces()
            {
                return Enumerable.Empty<Type>();
            }

            public bool WillExecute
            {
                get { return true; }
            }
        }
    }
}
