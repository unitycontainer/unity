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
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    [TestClass]
    public class AttributeDrivenPolicyFixture
    {
        MethodImplementationInfo nothingSpecialMethod;
        MethodImplementationInfo doSomethingMethod;
        MethodImplementationInfo getCriticalInfoMethod;
        MethodImplementationInfo mustBeFastMethod;
        MethodImplementationInfo getNameMethod;
        MethodImplementationInfo hasAttributeMethod;
        MethodImplementationInfo doesntHaveAttributeMethod;
        MethodImplementationInfo aNewMethod;
        MethodImplementationInfo getNewNameMethod;

        MethodImplementationInfo getItemMethod;
        MethodImplementationInfo setItemMethod;
        MethodImplementationInfo getItemIntMethod;
        MethodImplementationInfo setItemIntMethod;
        MethodImplementationInfo getItemStringMethod;
        MethodImplementationInfo setItemStringMethod;

        [TestInitialize]
        public void Setup()
        {
            nothingSpecialMethod = MakeMethodImpl<AttributeTestTarget>("NothingSpecial");
            doSomethingMethod = MakeMethodImpl<AttributeTestTarget>("DoSomething");
            getCriticalInfoMethod = MakeMethodImpl<AttributeTestTarget>("GetCriticalInformation");
            mustBeFastMethod = MakeMethodImpl<AttributeTestTarget>("MustBeFast");
            getNameMethod = new MethodImplementationInfo(null, typeof(AttributeTestTarget).GetProperty("Name").GetGetMethod());
            hasAttributeMethod = MakeMethodImpl<SecondAttributeTestTarget>("HasAttribute");
            doesntHaveAttributeMethod = MakeMethodImpl<SecondAttributeTestTarget>("DoesntHaveAttribute");
            aNewMethod = MakeMethodImpl<DerivedAttributeTestTarget>("ANewMethod");
            getNewNameMethod = new MethodImplementationInfo(null, StaticReflection.GetPropertyGetMethodInfo((DerivedAttributeTestTarget t) => t.Name));

            getItemMethod = new MethodImplementationInfo(null, StaticReflection.GetPropertyGetMethodInfo((DerivedAttributeTestTarget t) => t.Item));
            setItemMethod = new MethodImplementationInfo(null, StaticReflection.GetPropertySetMethodInfo((DerivedAttributeTestTarget t) => t.Item));

            getItemIntMethod = new MethodImplementationInfo(null, typeof(DerivedAttributeTestTarget).GetMethod("get_Item", new[] { typeof(int) }));
            setItemIntMethod = new MethodImplementationInfo(null, typeof(DerivedAttributeTestTarget).GetMethod("set_Item", new[] { typeof(int), typeof(object) }));

            getItemStringMethod = new MethodImplementationInfo(null, typeof(DerivedAttributeTestTarget).GetMethod("get_Item", new[] { typeof(string), typeof(double) }));
            setItemStringMethod = new MethodImplementationInfo(null, typeof(DerivedAttributeTestTarget).GetMethod("set_Item", new[] { typeof(string), typeof(double), typeof(object) }));
        }

        private static MethodImplementationInfo MakeMethodImpl<T>(string name)
        {
            return new MethodImplementationInfo(null, typeof(T).GetMethod(name));
        }

        [TestMethod]
        public void MatchingRuleMatchesForAllMethodsInAttributeTestTarget()
        {
            IMatchingRule rule = new AttributeDrivenPolicyMatchingRule();
            Assert.IsTrue(rule.Matches(nothingSpecialMethod.ImplementationMethodInfo));
            Assert.IsTrue(rule.Matches(doSomethingMethod.ImplementationMethodInfo));
            Assert.IsTrue(rule.Matches(getCriticalInfoMethod.ImplementationMethodInfo));
            Assert.IsTrue(rule.Matches(mustBeFastMethod.ImplementationMethodInfo));
        }

        [TestMethod]
        public void MatchingRuleOnlyMatchesOnMethodsWithAttributes()
        {
            IMatchingRule rule = new AttributeDrivenPolicyMatchingRule();

            Assert.IsTrue(rule.Matches(hasAttributeMethod.ImplementationMethodInfo));
            Assert.IsFalse(rule.Matches(doesntHaveAttributeMethod.ImplementationMethodInfo));
        }

        [TestMethod]
        public void ShouldMatchInheritedHandlerAttributes()
        {
            IMatchingRule rule = new AttributeDrivenPolicyMatchingRule();
            Assert.IsTrue(rule.Matches(aNewMethod.ImplementationMethodInfo));
        }

        [TestMethod]
        public void ShouldHaveAttributesCauseMatchesOnMethods()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();

            Assert.IsTrue(policy.Matches(nothingSpecialMethod));
            Assert.IsFalse(policy.Matches(mustBeFastMethod));
        }

        [TestMethod]
        public void ShouldGetCorrectHandlersForMethods()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(nothingSpecialMethod, new UnityContainer()));

            Assert.AreEqual(1, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
        }

        [TestMethod]
        public void ShouldGetHandlersFromClassAndMethodAttributes()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(doSomethingMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler3), handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldGetNoHandlersIfApplyNoPoliciesIsPresent()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(mustBeFastMethod, new UnityContainer()));
            Assert.AreEqual(0, handlers.Count);
        }

        [TestMethod]
        public void ShouldHaveLoggingHandlerForNothingSpecial()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers
                = new List<ICallHandler>(policy.GetHandlersFor(nothingSpecialMethod, new UnityContainer()));
            Assert.AreEqual(1, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
        }

        [TestMethod]
        public void ShouldHaveLoggingAndValidationForDoSomething()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers
                = new List<ICallHandler>(policy.GetHandlersFor(doSomethingMethod, new UnityContainer()));

            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler3), handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldApplyHandlersIfAttributesAreOnProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(getNameMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler3), handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldApplyHandlersIfAttributesAreOnNewProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(getNewNameMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler1), handlers[1].GetType());
        }


        [TestMethod]
        public void ShouldApplyHandlersToGetterIfAttributesAreOnItemProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(getItemMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler3), handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldApplyHandlersToSetterIfAttributesAreOnItemProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(setItemMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler3), handlers[1].GetType());
        }


        [TestMethod]
        public void ShouldApplyHandlersToGetterIfAttributesAreOnIndexedItemProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(getItemIntMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler1), handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldApplyHandlersToSetterIfAttributesAreOnIndexedItemProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(setItemIntMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler1), handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldApplyHandlersToGetterIfAttributesAreOnSecondIndexedItemProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(getItemStringMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler3), handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldApplyHandlersToSetterIfAttributesAreOnSecondIndexedItemProperty()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers =
                new List<ICallHandler>(policy.GetHandlersFor(setItemStringMethod, new UnityContainer()));
            Assert.AreEqual(2, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
            Assert.AreSame(typeof(CallHandler3), handlers[1].GetType());
        }


        [TestMethod]
        public void ShouldInheritHandlersFromBaseClass()
        {
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers
                = new List<ICallHandler>(policy.GetHandlersFor(aNewMethod, new UnityContainer()));
            Assert.AreEqual(1, handlers.Count);
            Assert.AreSame(typeof(CallHandler2), handlers[0].GetType());
        }

        [TestMethod]
        public void ShouldInheritHandlersFromInterface()
        {
            MethodImplementationInfo getNewsMethod = new MethodImplementationInfo(
                typeof(INewsService).GetMethod("GetNews"),
                typeof(NewsService).GetMethod("GetNews"));
            AttributeDrivenPolicy policy = new AttributeDrivenPolicy();
            List<ICallHandler> handlers
                = new List<ICallHandler>(policy.GetHandlersFor(getNewsMethod, new UnityContainer()));
            Assert.AreEqual(1, handlers.Count);
            Assert.AreSame(typeof(CallHandler1), handlers[0].GetType());
        }
    }

    [CallHandler2]//(Categories = new string[] { "one", "two" }, Priority = 34)]
    class AttributeTestTarget
    {
        [CallHandler3]
        public string Name
        {
            get { return "someName"; }
            set { }
        }

        [CallHandler3]
        public string DoSomething(string key,
                                  int value)
        {
            return "I did something";
        }

        public int GetCriticalInformation(string key)
        {
            return 42;
        }

        [ApplyNoPolicies]
        public void MustBeFast() { }

        public int NothingSpecial()
        {
            return 43;
        }

        [CallHandler1]
        public object this[int ignored] { get { return null; } set { ;} }

        [CallHandler3]
        public object this[string ignored, double ignored2] { get { return null; } set { ;} }
    }

    class SecondAttributeTestTarget
    {
        public void DoesntHaveAttribute() { }

        [CallHandler2]
        public void HasAttribute() { }
    }

    class DerivedAttributeTestTarget : AttributeTestTarget
    {
        public void ANewMethod() { }

        [CallHandler1]
        public new string Name
        {
            get { return "someOtherName"; }
            set { }
        }

        [CallHandler3]
        public object Item { get; set; }
    }

    public interface INewsService
    {
        [CallHandler1]//(0, 0, 30)]
        IList GetNews();
    }

    public class NewsService : INewsService
    {
        public IList GetNews()
        {
            return new List<string> { "News1", "News2", "News3" };
        }
    }

    public class CallHandler1Attribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer ignored)
        {
            return new CallHandler1();
        }
    }

    public class CallHandler1 : ICallHandler
    {
        #region ICallHandler Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Order
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }

    public class CallHandler2Attribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer ignored)
        {
            return new CallHandler2();
        }
    }

    public class CallHandler2 : ICallHandler
    {
        #region ICallHandler Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Order
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }

    public class CallHandler3Attribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer ignored)
        {
            return new CallHandler3();
        }
    }

    public class CallHandler3 : ICallHandler
    {
        #region ICallHandler Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Order
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
