// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for the PolicySet class.
    /// </summary>
    [TestClass]
    public class PolicySetFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void SetUp()
        {
            container = new UnityContainer();
        }

        //[TestMethod]
        //public void ShouldInitializeToEmpty()
        //{
        //    PolicySet policies = new PolicySet();

        //    Assert.IsFalse(policies.AppliesTo(GetType()));
        //    MethodInfo thisMember = GetType().GetMethod("ShouldInitializeToEmpty");
        //    List<ICallHandler> handlers = new List<ICallHandler>(policies.GetHandlersFor(thisMember));
        //    Assert.AreEqual(0, handlers.Count);
        //}

        [TestMethod]
        public void ShouldBeAbleToAddOnePolicy()
        {
            container
                .RegisterInstance<ICallHandler>("Handler1", new Handler1())
                .RegisterInstance<ICallHandler>("Handler2", new Handler2());

            PolicySet policies = new PolicySet();

            RuleDrivenPolicy p
                = new RuleDrivenPolicy(
                    "NameMatching",
                    new IMatchingRule[] { new MemberNameMatchingRule("ShouldBeAbleToAddOnePolicy") },
                    new string[] { "Handler1", "Handler2" });

            policies.Add(p);

            MethodImplementationInfo thisMember = GetMethodImplInfo<PolicySetFixture>("ShouldBeAbleToAddOnePolicy");
            List<ICallHandler> handlers =
                new List<ICallHandler>(policies.GetHandlersFor(thisMember, container));

            Assert.AreEqual(2, handlers.Count);
            Assert.IsTrue(typeof(Handler1) == handlers[0].GetType());
            Assert.IsTrue(typeof(Handler2) == handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldMatchPolicyByTypeName()
        {
            PolicySet policies = GetMultiplePolicySet();

            MethodImplementationInfo nameDoesntMatchMember = GetMethodImplInfo<MatchesByType>("NameDoesntMatch");
            MethodImplementationInfo nameMatchMember = GetMethodImplInfo<MatchesByType>("NameMatch");

            List<ICallHandler> nameDoesntMatchHandlers =
                new List<ICallHandler>(policies.GetHandlersFor(nameDoesntMatchMember, container));
            List<ICallHandler> nameMatchHandlers =
                new List<ICallHandler>(policies.GetHandlersFor(nameMatchMember, container));

            Assert.AreEqual(1, nameDoesntMatchHandlers.Count);
            Assert.IsTrue(typeof(Handler1) == nameDoesntMatchHandlers[0].GetType());

            Assert.AreEqual(2, nameMatchHandlers.Count);
            Assert.IsTrue(typeof(Handler1) == nameMatchHandlers[0].GetType());
            Assert.IsTrue(typeof(Handler2) == nameMatchHandlers[1].GetType());
        }

        [TestMethod]
        public void ShouldMatchPolicyByMethodName()
        {
            PolicySet policies = GetMultiplePolicySet();

            MethodImplementationInfo noMatchMember = GetMethodImplInfo<MatchesByMemberName>("NoMatch");
            MethodImplementationInfo nameMatchMember = GetMethodImplInfo<MatchesByMemberName>("NameMatch");
            List<ICallHandler> noMatchHandlers =
                new List<ICallHandler>(policies.GetHandlersFor(noMatchMember, container));
            List<ICallHandler> nameMatchHandlers =
                new List<ICallHandler>(policies.GetHandlersFor(nameMatchMember, container));

            Assert.AreEqual(0, noMatchHandlers.Count);
            Assert.AreEqual(1, nameMatchHandlers.Count);
            Assert.IsTrue(typeof(Handler2) == nameMatchHandlers[0].GetType());
        }

        [TestMethod]
        public void ShouldNotMatchPolicyWhenNoRulesMatch()
        {
            PolicySet policies = GetMultiplePolicySet();

            MethodImplementationInfo noMatchMember = GetMethodImplInfo<NoMatchAnywhere>("NoMatchHere");
            List<ICallHandler> noMatchHandlers =
                new List<ICallHandler>(policies.GetHandlersFor(noMatchMember, container));
            Assert.AreEqual(0, noMatchHandlers.Count);
        }

        [TestMethod]
        public void ShouldGetCorrectHandlersGivenAttributesOnInterfaceMethodsAfterAddingAttributeDrivenPolicy()
        {
            PolicySet policies = new PolicySet();

            List<ICallHandler> oneHandlers
                = new List<ICallHandler>(policies.GetHandlersFor(GetMethodImplInfo<TwoType>("One"), container));

            Assert.AreEqual(0, oneHandlers.Count);

            policies.Add(new AttributeDrivenPolicy());

            MethodImplementationInfo oneInfo = new MethodImplementationInfo(
                typeof(IOne).GetMethod("One"),
                typeof(TwoType).GetMethod("One"));

            oneHandlers
                = new List<ICallHandler>(policies.GetHandlersFor(oneInfo, container));

            Assert.AreEqual(2, oneHandlers.Count);
            Assert.IsTrue(oneHandlers[0] is MarkerCallHandler);
            Assert.IsTrue(oneHandlers[1] is MarkerCallHandler);

            Assert.AreEqual("IOneOne", ((MarkerCallHandler)oneHandlers[0]).HandlerName);
            Assert.AreEqual("MethodOneOverride", ((MarkerCallHandler)oneHandlers[1]).HandlerName);
        }

        [TestMethod]
        public void ShouldNotDuplicateHandlersWhenCreatingViaInterface()
        {
            container
                .RegisterInstance<ICallHandler>("Handler1", new CallCountHandler())
                .RegisterInstance<ICallHandler>("Handler2", new CallCountHandler());

            RuleDrivenPolicy policy
                = new RuleDrivenPolicy("MatchesInterfacePolicy",
                    new IMatchingRule[] { new TypeMatchingRule("ITwo") },
                    new string[] { "Handler1", "Handler2" });

            PolicySet policies = new PolicySet(policy);
            MethodImplementationInfo twoInfo = new MethodImplementationInfo(
                typeof(ITwo).GetMethod("Two"), typeof(TwoType).GetMethod("Two"));

            List<ICallHandler> handlers
                = new List<ICallHandler>(policies.GetHandlersFor(twoInfo, container));
            Assert.AreEqual(2, handlers.Count);
        }

        [TestMethod]
        public void HandlersOrderedProperly()
        {
            RuleDrivenPolicy policy
                = new RuleDrivenPolicy("MatchesInterfacePolicy",
                    new IMatchingRule[] { new TypeMatchingRule("ITwo") },
                    new string[] { "Handler1", "Handler2", "Handler3", "Handler4" });

            ICallHandler handler1 = new CallCountHandler();
            handler1.Order = 3;

            ICallHandler handler2 = new CallCountHandler();
            handler2.Order = 0;

            ICallHandler handler3 = new CallCountHandler();
            handler3.Order = 2;

            ICallHandler handler4 = new CallCountHandler();
            handler4.Order = 1;

            container
                .RegisterInstance<ICallHandler>("Handler1", handler1)
                .RegisterInstance<ICallHandler>("Handler2", handler2)
                .RegisterInstance<ICallHandler>("Handler3", handler3)
                .RegisterInstance<ICallHandler>("Handler4", handler4);

            PolicySet policies = new PolicySet(policy);

            MethodImplementationInfo twoInfo = new MethodImplementationInfo(
                typeof(ITwo).GetMethod("Two"), typeof(TwoType).GetMethod("Two"));
            List<ICallHandler> handlers
                = new List<ICallHandler>(policies.GetHandlersFor(twoInfo, container));

            Assert.AreSame(handler4, handlers[0]);
            Assert.AreSame(handler3, handlers[1]);
            Assert.AreSame(handler1, handlers[2]);
            Assert.AreSame(handler2, handlers[3]);
        }

        [TestMethod]
        public void HandlersOrderedProperlyUsingRelativeAndAbsoluteOrder()
        {
            RuleDrivenPolicy policy
                = new RuleDrivenPolicy("MatchesInterfacePolicy",
                    new IMatchingRule[] { new TypeMatchingRule("ITwo") },
                    new string[] { "Handler1", "Handler2", "Handler3", "Handler4", "Handler5", "Handler6" });

            ICallHandler handler1 = new CallCountHandler();
            handler1.Order = 0;

            ICallHandler handler2 = new CallCountHandler();
            handler2.Order = 3;

            ICallHandler handler3 = new CallCountHandler();
            handler3.Order = 3;

            ICallHandler handler4 = new CallCountHandler();
            handler4.Order = 2;

            ICallHandler handler5 = new CallCountHandler();
            handler5.Order = 4;

            ICallHandler handler6 = new CallCountHandler();
            handler6.Order = 1;

            container
                .RegisterInstance<ICallHandler>("Handler1", handler1)
                .RegisterInstance<ICallHandler>("Handler2", handler2)
                .RegisterInstance<ICallHandler>("Handler3", handler3)
                .RegisterInstance<ICallHandler>("Handler4", handler4)
                .RegisterInstance<ICallHandler>("Handler5", handler5)
                .RegisterInstance<ICallHandler>("Handler6", handler6);

            PolicySet policies = new PolicySet(policy);

            MethodImplementationInfo twoInfo = new MethodImplementationInfo(
                typeof(ITwo).GetMethod("Two"), typeof(TwoType).GetMethod("Two"));

            List<ICallHandler> handlers
                = new List<ICallHandler>(policies.GetHandlersFor(twoInfo, container));

            Assert.AreEqual(handler6, handlers[0]);
            Assert.AreEqual(handler4, handlers[1]);
            Assert.AreEqual(handler2, handlers[2]);
            Assert.AreEqual(handler3, handlers[3]);
            Assert.AreEqual(handler5, handlers[4]);
            Assert.AreEqual(handler1, handlers[5]);
        }

        private PolicySet GetMultiplePolicySet()
        {
            container
                .RegisterInstance<ICallHandler>("Handler1", new Handler1())
                .RegisterInstance<ICallHandler>("Handler2", new Handler2());

            RuleDrivenPolicy typeMatchPolicy
                = new RuleDrivenPolicy("MatchesType",
                    new IMatchingRule[] { new TypeMatchingRule(typeof(MatchesByType)) },
                    new string[] { "Handler1" });

            RuleDrivenPolicy nameMatchPolicy
                = new RuleDrivenPolicy("MatchesName",
                    new IMatchingRule[] { new MemberNameMatchingRule("NameMatch") },
                    new string[] { "Handler2" });

            return new PolicySet(typeMatchPolicy, nameMatchPolicy);
        }

        private MethodInfo GetNameDoesntMatchMethod()
        {
            return typeof(MatchesByType).GetMethod("NameDoesntMatch");
        }

        private MethodImplementationInfo GetMethodImplInfo<T>(string methodName)
        {
            return new MethodImplementationInfo(null,
                typeof(T).GetMethod(methodName));
        }
    }

    internal class MatchesByType
    {
        // Matches type policy
        public void NameDoesntMatch() { }

        // matches type & name policies
        public void NameMatch() { }
    }

    internal class MatchesByMemberName
    {
        public void NameMatch() { }

        public void NoMatch() { }
    }

    internal class NoMatchAnywhere
    {
        public void NoMatchHere() { }
    }

    public interface IOne
    {
        [MarkerCallHandler("IOneOne")]
        void One();
    }

    public interface ITwo
    {
        void Two();
    }

    public class OneType : IOne
    {
        public void MethodOne() { }

        public virtual void MethodTwo() { }

        public virtual void One() { }
    }

    public class TwoType : OneType, ITwo
    {
        public void BarOne() { }

        public override void MethodTwo() { }

        [MarkerCallHandler("MethodOneOverride")]
        public override void One() { }

        public void Two() { }
    }

    public class MarkerCallHandler : ICallHandler
    {
        private string handlerName;
        private int order = 0;

        public MarkerCallHandler(string handlerName)
        {
            this.handlerName = handlerName;
        }

        public string HandlerName
        {
            get { return handlerName; }
            set { handlerName = value; }
        }

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
            return getNext()(input, getNext);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
    public class MarkerCallHandlerAttribute : HandlerAttribute
    {
        private string handlerName;

        public MarkerCallHandlerAttribute(string handlerName)
        {
            this.handlerName = handlerName;
        }

        public override ICallHandler CreateHandler(IUnityContainer ignored)
        {
            return new MarkerCallHandler(this.handlerName);
        }
    }
}
