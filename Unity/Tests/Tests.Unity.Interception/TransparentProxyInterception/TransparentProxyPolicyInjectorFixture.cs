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
using System.Runtime.Remoting;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Summary description for FactoryFixture
    /// </summary>
    [TestClass]
    public class TransparentProxyPolicyInjectorFixture
    {
        CallCountHandler countHandler;
        IUnityContainer container;

        [TestInitialize]
        public void SetUp()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void ShouldCreateRawObjectWhenNoPolicyPresent()
        {
            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();
            PolicySet policySet = new PolicySet();

            MockDal dal = factory.Wrap<MockDal>(new MockDal(), policySet, container);

            Assert.IsNotNull(dal);
            Assert.IsTrue(dal is MockDal);
            Assert.IsFalse(RemotingServices.IsTransparentProxy(dal));
        }

        [TestMethod]
        public void ShouldRequireInteceptionForTypeMatchingPolicy()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            bool shouldInterceptMockDal = factory.TypeRequiresInterception(typeof(MockDal), policySet);
            bool shouldInterceptString = factory.TypeRequiresInterception(typeof(string), policySet);

            Assert.IsTrue(shouldInterceptMockDal);
            Assert.IsFalse(shouldInterceptString);
        }

        [TestMethod]
        public void ShouldAddInterceptionAccordingToPolicy()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            MockDal dal = factory.Wrap<MockDal>(new MockDal(), policySet, container);

            Assert.IsTrue(dal is MockDal);
            Assert.IsTrue(RemotingServices.IsTransparentProxy(dal));
            object realProxy = RemotingServices.GetRealProxy(dal);
            Assert.IsNotNull(realProxy);
            Assert.IsTrue(realProxy is InterceptingRealProxy);
        }

        [TestMethod]
        public void ShouldCallHandlersWhenCallingMethods()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            MockDal dal = factory.Wrap<MockDal>(new MockDal(), policySet, container);

            Assert.AreEqual(0, countHandler.CallCount);
            dal.DoSomething("43");
            dal.DoSomething("63");
            dal.DoSomething("Hike!");
            Assert.AreEqual(3, countHandler.CallCount);
        }

        [TestMethod]
        public void HandlerCanShortcutMethodExecution()
        {
            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();
            PolicySet policySet = new PolicySet(GetShortcutPolicy());

            MockDal dal = factory.Wrap<MockDal>(new MockDal(), policySet, container);

            Assert.AreEqual(42, dal.DoSomething("should return 42"));
            Assert.AreEqual(-1, dal.DoSomething("shortcut"));
        }

        [TestMethod]
        public void ShouldCallProperHandlersInThePresenceOfOverloadedMethods()
        {
            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();
            PolicySet policySet = new PolicySet(GetOverloadsPolicy());

            MockDalWithOverloads dal
                = factory.Wrap<MockDalWithOverloads>(new MockDalWithOverloads(), policySet, container);

            Assert.AreEqual(42, dal.DoSomething("not intercepted"));
            Assert.IsNull(dal.DoSomething(42));
        }

        [TestMethod]
        public void ShouldCallHandlersOnPropertyGet()
        {
            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();
            PolicySet policySet = new PolicySet(GetPropertiesPolicy());

            MockDal dal = factory.Wrap<MockDal>(new MockDal(), policySet, container);
            double startBalance = dal.Balance;
            dal.Balance = 162.3;
            double endBalance = dal.Balance;

            Assert.AreEqual(2, countHandler.CallCount);
        }

        [TestMethod]
        public void ShouldNotInterceptMethodsThatHaveNoPolicyAttribute()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            MockDal dal = factory.Wrap<MockDal>(new MockDal(), policySet, container);

            Assert.IsTrue(RemotingServices.IsTransparentProxy(dal));
            Assert.AreEqual(0, countHandler.CallCount);
            dal.SomethingCritical();
            Assert.AreEqual(0, countHandler.CallCount);
        }

        [TestMethod]
        public void ShouldNotInterceptAnyMethodsOnClassThatHasNoPolicyAttribute()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            CriticalFakeDal dal = factory.Wrap<CriticalFakeDal>(new CriticalFakeDal(), policySet, container);

            Assert.IsFalse(RemotingServices.IsTransparentProxy(dal));
        }

        [TestMethod]
        public void ShouldInterceptInterfaceImplementationMethods()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            IDal dal = factory.Wrap<IDal>(new MockDal(), policySet, container);
            Assert.AreEqual(0, countHandler.CallCount);
            dal.Deposit(200);
            Assert.AreEqual(1, countHandler.CallCount);
        }

        [TestMethod]
        public void ShouldNotBeAbleToCastToUnimplementedInterfaces()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            IDal dal = factory.Wrap<IDal>(new MockDal(), policySet, container);

            ICallHandler ch = dal as ICallHandler;
            Assert.IsNull(ch);
        }

        [TestMethod]
        public void CanCastToImplementedInterfaces()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            MockDal dal = factory.Wrap<MockDal>(new MockDal(), policySet, container);

            IDal iDal = dal as IDal;
            Assert.IsNotNull(iDal);
        }

        [TestMethod]
        public void CanCastAcrossInterfaces()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            IDal dal = factory.Wrap<IDal>(new MockDal(), policySet, container);

            IMonitor monitor = dal as IMonitor;
            Assert.IsNotNull(monitor);
        }

        [TestMethod]
        public void CannotCastToNonMBROBaseClass()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            IDal dal = factory.Wrap<IDal>(new InterfacesOnlyDal(), policySet, container);

            Assert.IsNotNull(dal as IMonitor);
            Assert.IsNull(dal as InterfacesOnlyDal);
        }

        [TestMethod]
        public void CanCastToMBROBaseClass()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            IDal dal = factory.Wrap<IDal>(new MockDal(), policySet, container);

            Assert.IsNotNull(dal as IMonitor);
            Assert.IsNotNull(dal as MockDal);
        }

        [TestMethod]
        public void ShouldCreateAllPipelinesForTargetWhenCreatingViaInterface()
        {
            RuleDrivenPolicy policy
                = new RuleDrivenPolicy(
                    "MockDal Policy",
                    new IMatchingRule[] { new TypeMatchingRule(typeof(MockDal)) },
                    new string[] { "CallCountHandler" });
            container.RegisterInstance<ICallHandler>("CallCountHandler", countHandler = new CallCountHandler());

            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();
            IDal dal = factory.Wrap<IDal>(new MockDal(), new PolicySet(policy), container);

            IMonitor monitor = (IMonitor)dal;

            monitor.Log("one");
            monitor.Log("two");
            monitor.Log("tree");

            MockDal target = (MockDal)dal;
            target.DoSomething("something");

            Assert.AreEqual(4, countHandler.CallCount);
        }

        [TestMethod]
        public void CanRewrapAnInterceptedObject()
        {
            TransparentProxyPolicyInjector factory = GetFactoryWithPolicies();
            PolicySet policySet = new PolicySet(GetCallCountingPolicy());

            IDal dal = factory.Wrap<IDal>(new MockDal(), policySet, container);

            object dalTarget = ((InterceptingRealProxy)RemotingServices.GetRealProxy(dal)).Target;

            IMonitor monitor = factory.Wrap<IMonitor>(dalTarget, policySet, container);

            object monitorTarget = ((InterceptingRealProxy)RemotingServices.GetRealProxy(monitor)).Target;
            Assert.AreSame(dalTarget, monitorTarget);
        }

        [TestMethod]
        public void ShouldInterceptMethodsIfTypeImplementsMatchingInterface()
        {
            PolicySet policies = new PolicySet(GetPolicyThatMatchesInterface());
            Assert.IsTrue(policies.AppliesTo(typeof(MockDal)));

            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();

            MockDal mockDal = factory.Wrap<MockDal>(new MockDal(), policies, container);
            IDal dal = mockDal;

            dal.Deposit(123.45);
            dal.Withdraw(54.32);
            mockDal.DoSomething("foo");

            Assert.AreEqual(2, countHandler.CallCount);
        }

        [TestMethod]
        public void ShouldCallHandlerOnImplementationMethodWhenCalledViaInterface()
        {
            // Set up policy to match underlying type and method name
            RuleDrivenPolicy policy
                = new RuleDrivenPolicy(
                    "MockDal.Deposit policy",
                    new IMatchingRule[] { 
                        new TypeMatchingRule(typeof(MockDal)),
                        new MemberNameMatchingRule("Deposit")
                    },
                    new string[] { "CallCountHandler" });
            container.RegisterInstance<ICallHandler>("CallCountHandler", countHandler = new CallCountHandler());

            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();

            IDal dal = factory.Wrap<IDal>(new MockDal(), new PolicySet(policy), container);
            dal.Deposit(10);
            dal.Deposit(54);
            dal.Deposit(72.5);

            Assert.AreEqual(3, countHandler.CallCount);
        }

        #region Helper Factory methods

        RuleDrivenPolicy GetCallCountingPolicy()
        {
            RuleDrivenPolicy typeMatchPolicy
                = new RuleDrivenPolicy(
                    "DALPolicy",
                    new IMatchingRule[] 
                    {
                        new NamespaceMatchingRule("Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest")
                    },
                    new string[] { "CallCountHandler" });
            container.RegisterInstance<ICallHandler>("CallCountHandler", countHandler = new CallCountHandler());

            return typeMatchPolicy;
        }

        RuleDrivenPolicy GetShortcutPolicy()
        {
            RuleDrivenPolicy typeMatchPolicy
                = new RuleDrivenPolicy(
                    "ShortcutPolicy",
                    new IMatchingRule[]
                    {
                        new TypeMatchingAssignmentRule(typeof(MockDal))
                    },
                    new string[] { "shortcut" });
            container.RegisterInstance<ICallHandler>("shortcut", new ShortcuttingHandler("shortcut"));

            return typeMatchPolicy;
        }

        RuleDrivenPolicy GetOverloadsPolicy()
        {
            RuleDrivenPolicy policy
                = new RuleDrivenPolicy(
                    "NullStringPolicy",
                    new IMatchingRule[]
                    {
                        new TagAttributeMatchingRule("NullString")
                    },
                    new string[] { "MakeReturnNullHandler" });
            container.RegisterInstance<ICallHandler>("MakeReturnNullHandler", new MakeReturnNullHandler());

            return policy;
        }

        RuleDrivenPolicy GetPropertiesPolicy()
        {
            RuleDrivenPolicy policy
                = new RuleDrivenPolicy(
                    "Intercept balance policy",
                    new IMatchingRule[]
                    {
                        new MemberNameMatchingRule("get_Balance")
                    },
                    new string[] { "CallCountHandler" });
            container.RegisterInstance<ICallHandler>("CallCountHandler", countHandler = new CallCountHandler());

            return policy;
        }

        RuleDrivenPolicy GetPolicyThatMatchesInterface()
        {
            RuleDrivenPolicy policy
                = new RuleDrivenPolicy(
                    "Matches IDal",
                    new IMatchingRule[]
                    {
                        new TypeMatchingAssignmentRule(typeof(IDal))
                    },
                    new string[] { "CallCountHandler" });
            container.RegisterInstance<ICallHandler>("CallCountHandler", countHandler = new CallCountHandler());

            return policy;
        }

        TransparentProxyPolicyInjector GetFactoryWithPolicies()
        {
            TransparentProxyPolicyInjector factory = new TransparentProxyPolicyInjector();
            return factory;
        }

        #endregion
    }
}