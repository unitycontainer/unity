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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for the Policy class
    /// </summary>
    [TestClass]
    public class PolicyFixture
    {
        //[TestMethod]
        //public void ShouldInitializeToEmpty()
        //{
        //    RuleDrivenPolicy p = new RuleDrivenPolicy("Empty");
        //    Assert.AreEqual("Empty", p.Name);
        //    Assert.AreEqual(0, p.RuleSet.Count);
        //    Assert.AreEqual(0, p.Handlers.Count);
        //}

        //[TestMethod]
        //public void ShouldPreserveHandlerOrder()
        //{
        //    RuleDrivenPolicy p = new RuleDrivenPolicy("OrderedHandlers");

        //    ICallHandler h1 = new Handler1();
        //    ICallHandler h2 = new Handler2();
        //    ICallHandler h3 = new Handler3();

        //    p.Handlers.Add(h2);
        //    p.Handlers.Add(h1);
        //    p.Handlers.Add(h3);

        //    Assert.AreEqual(3, p.Handlers.Count);
        //    Assert.AreSame(h2, p.Handlers[0]);
        //    Assert.AreSame(h1, p.Handlers[1]);
        //    Assert.AreSame(h3, p.Handlers[2]);
        //}

        [TestMethod]
        public void ShouldHaveNoHandlersWhenPolicyDoesntMatch()
        {
            IMatchingRule[] rules = { };
            IUnityContainer container = CreateConfiguredContainer();

            InjectionPolicy p = CreatePolicy(container, rules);

            MethodBase thisMember = GetType().GetMethod("ShouldHaveNoHandlersWhenPolicyDoesntMatch");
            List<ICallHandler> memberHandlers
                = new List<ICallHandler>(p.GetHandlersFor(thisMember, container));
            Assert.AreEqual(0, memberHandlers.Count);
        }

        [TestMethod]
        public void ShouldGetHandlersInOrderWithGetHandlersFor()
        {
            IMatchingRule[] rules = { new MemberNameMatchingRule("ShouldGetHandlersInOrderWithGetHandlersFor") };
            IUnityContainer container = CreateConfiguredContainer();

            InjectionPolicy p = CreatePolicy(container, rules);

            MethodBase member = GetType().GetMethod("ShouldGetHandlersInOrderWithGetHandlersFor");

            List<ICallHandler> expectedHandlers = new List<ICallHandler>(container.ResolveAll<ICallHandler>());
            List<ICallHandler> actualHandlers = new List<ICallHandler>(p.GetHandlersFor(member, container));

            CollectionAssert.AreEqual(
                expectedHandlers,
                actualHandlers,
                new TypeComparer());
        }

        [TestMethod]
        public void ShouldBeAbleToMatchPropertyGet()
        {
            IMatchingRule[] rules = { new MemberNameMatchingRule("get_Balance") };
            IUnityContainer container = CreateConfiguredContainer();

            InjectionPolicy p = CreatePolicy(container, rules);

            PropertyInfo balanceProperty = typeof(MockDal).GetProperty("Balance");
            MethodBase getMethod = balanceProperty.GetGetMethod();

            List<ICallHandler> expectedHandlers = new List<ICallHandler>(container.ResolveAll<ICallHandler>());
            List<ICallHandler> actualHandlers = new List<ICallHandler>(p.GetHandlersFor(getMethod, container));

            CollectionAssert.AreEqual(
                expectedHandlers,
                actualHandlers,
                new TypeComparer());
        }

        private static InjectionPolicy CreatePolicy(IUnityContainer container, IMatchingRule[] rules)
        {
            InjectionPolicy p
                = new RuleDrivenPolicy(rules, new string[] { "handler1", "handler2", "handler3" });
            return p;
        }

        private static IUnityContainer CreateConfiguredContainer()
        {
            IUnityContainer container =
                new UnityContainer()
                .RegisterType<ICallHandler, Handler1>("handler1")
                .RegisterType<ICallHandler, Handler2>("handler2")
                .RegisterType<ICallHandler, Handler3>("handler3");
            return container;
        }

        //[TestMethod]
        //public void ShouldOnlyGetHandlersOnceIfPolicyMatchesBothClassAndInterface()
        //{
        //    IMatchingRule[] rules = { new MemberNameMatchingRule("MyMethod") };
        //    ICallHandler[] handlers = { new CallCountHandler() };

        //    RuleDrivenPolicy p = new RuleDrivenPolicy(rules, handlers);

        //    MethodInfo myMethod = typeof(MyFooClass).GetMethod("MyMethod");
        //    List<ICallHandler> retrievedHandlers = new List<ICallHandler>(p.GetHandlersFor(myMethod));

        //    Assert.AreEqual(1, retrievedHandlers.Count);
        //    Assert.AreSame(handlers[0], retrievedHandlers[0]);
        //}
    }

    class Handler1 : ICallHandler
    {
        int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    class Handler2 : ICallHandler
    {
        int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    class Handler3 : ICallHandler
    {
        int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    public interface IFoo
    {
        void MyMethod();
    }

    public class MyFooClass : IFoo
    {
        public void MyMethod() { }
    }

    public class TypeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x.GetType() == y.GetType())
                return 0;

            return -1;
        }
    }
}