using System;
using System.Runtime.Remoting;

using Microsoft.Practices.Unity.InterceptionExtension.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Tests.TestObjects;

namespace Unity.Tests.InterceptionExtension
{
    [TestClass]
    public class InterceptionConfigurationV5SchemaFixture : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\InterceptionConfigurationV5SchemaFixture.config";
        
        // dummy type to ensure assembly is copied
        // http://connect.microsoft.com/VisualStudio/feedback/details/533935/referenced-assemblies-in-unit-test-are-not-copied-in-testresults-out
        private Type dummy = typeof(InterceptionConfigurationExtension);

        public InterceptionConfigurationV5SchemaFixture()
            : base(ConfigFileName)
        {
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanUseInterfaceInterceptorThroughConfiguration()
        {
            IUnityContainer container = GetContainer("CanUseInterfaceInterceptorThroughConfiguration");

            DoNothingInterceptionBehavior.Reset();
            IInterfaceA abc = container.Resolve<IInterfaceA>();
            abc.TargetMethod();

            Assert.AreEqual<string>("Called", DoNothingInterceptionBehavior.PreCalled);
            Assert.AreEqual<string>("Called", DoNothingInterceptionBehavior.PostCalled);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanUseTransparentProxyInterceptionThroughConfiguration()
        {
            IUnityContainer container = GetContainer("CanUseTransparentProxyInterceptionThroughConfiguration");
            DoNothingInterceptionBehavior.Reset();
            ImplementsMBRO abc = container.Resolve<ImplementsMBRO>();
            abc.TargetMethod();

            Assert.AreEqual<string>("Called", DoNothingInterceptionBehavior.PreCalled);
            Assert.AreEqual<string>("Called", DoNothingInterceptionBehavior.PostCalled);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanUseVirtualMethodInterceptionThroughConfiguration()
        {
            IUnityContainer container = GetContainer("CanUseVirtualMethodInterceptionThroughConfiguration");

            DoNothingInterceptionBehavior.Reset();
            HasVirtualMethods abc = container.Resolve<HasVirtualMethods>();
            abc.TargetMethod();

            Assert.AreEqual<string>("Called", DoNothingInterceptionBehavior.PreCalled);
            Assert.AreEqual<string>("Called", DoNothingInterceptionBehavior.PostCalled);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanUseInterfaceInterceptionAdditionalInterfacesThroughConfiguration()
        {
            IUnityContainer container = GetContainer("CanUseInterfaceInterceptionAdditionalInterfacesThroughConfiguration");

            IInterfaceA proxy = container.Resolve<IInterfaceA>();
            IAdditionalInterface casted = (IAdditionalInterface)proxy;
            int value = casted.DoNothing();

            Assert.AreEqual<int>(100, value);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanConfigureMultipleAdditionalInterfacesThroughConfiguration()
        {
            IUnityContainer container = GetContainer("CanConfigureMultipleAdditionalInterfacesThroughConfiguration");
            IInterfaceA proxy = container.Resolve<IInterfaceA>();
            IAdditionalInterface casted = (IAdditionalInterface)proxy;
            int value = casted.DoNothing();

            Assert.AreEqual<int>(100, value);

            IAdditionalInterface1 casted1 = (IAdditionalInterface1)proxy;
            int value1 = casted1.DoNothing1();

            Assert.AreEqual<int>(200, value1);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanUseTransparentProxyInterceptionWithAdditionalInterfacesThroughConfiguration()
        {
            IUnityContainer container = GetContainer("CanUseTransparentProxyInterceptionWithAdditionalInterfacesThroughConfiguration");

            ImplementsMBRO proxy = container.Resolve<ImplementsMBRO>();
            IAdditionalInterface casted = (IAdditionalInterface)proxy;
            int value = casted.DoNothing();

            Assert.AreEqual<int>(100, value);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanUseVirtualMethodInterceptionWithAdditionalInterfacesThroughConfiguration()
        {
            IUnityContainer container = GetContainer("CanUseVirtualMethodInterceptionWithAdditionalInterfacesThroughConfiguration");

            HasVirtualMethods proxy = container.Resolve<HasVirtualMethods>();
            IAdditionalInterface casted = (IAdditionalInterface)proxy;
            int value = casted.DoNothing();

            Assert.AreEqual<int>(100, value);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanMixDifferentInterceptionMechanismsForSameContainerConfiguration()
        {
            IUnityContainer container = GetContainer("CanMixDifferentInterceptionMechanismsForSameContainerConfiguration");

            IInterfaceA abc = container.Resolve<IInterfaceA>();
            abc.TargetMethod();

            Assert.AreEqual<string>("behaviour2", DoNothingInterceptionBehavior.BehaviourName);

            HasVirtualMethods abc1 = container.Resolve<HasVirtualMethods>();
            abc1.TargetMethod();

            Assert.AreEqual<string>("behaviour1", DoNothingInterceptionBehavior.BehaviourName);

            ImplementsMBRO abc2 = container.Resolve<ImplementsMBRO>();
            abc2.TargetMethod();

            Assert.AreEqual<string>("behaviour3", DoNothingInterceptionBehavior.BehaviourName);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanInterceptSameTypeWithDifferentNamesWithDifferentBehavioursForSameContainerConfiguration()
        {
            IUnityContainer container = GetContainer("CanInterceptSameTypeWithDifferentNamesWithDifferentBehavioursForSameContainerConfiguration");

            IInterfaceB proxy1 = container.Resolve<IInterfaceB>("UsingBehaviourA");
            string input = String.Empty;
            string returnValue = proxy1.TargetMethod(ref input);

            Assert.AreEqual<string>("PreAtargetPostA", input);

            IInterfaceB proxy2 = container.Resolve<IInterfaceB>("UsingBehaviourB");
            string input2 = String.Empty;
            string returnValue2 = proxy2.TargetMethod(ref input2);

            Assert.AreEqual<string>("PreBtargetPostB", input2);

            IInterfaceB proxy3 = container.Resolve<IInterfaceB>("UsingBehaviourC");
            string input3 = String.Empty;
            string returnValue3 = proxy3.TargetMethod(ref input3);

            Assert.AreEqual<string>("PreCtargetPostC", input3);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanInterceptWithBehaviourChainInConfiguration()
        {
            IUnityContainer container = GetContainer("CanInterceptWithBehaviourChainInConfiguration");

            IInterfaceB proxy = container.Resolve<IInterfaceB>();
            string input = String.Empty;
            string returnValue = proxy.TargetMethod(ref input);

            Assert.AreEqual<string>("PreCPreAPreBtargetPostBPostAPostC", input);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NonExistentInterceptionMechanismSpecifiedThrowsValidException()
        {
            IUnityContainer container = GetContainer("NonExistentInterceptionMechanismSpecifiedThrowsValidException");

            IInterfaceA proxy = container.Resolve<IInterfaceA>();
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void NonExistentBehaviourSpecifiedThrowsValidException()
        {
            IUnityContainer container = GetContainer("NonExistentBehaviourSpecifiedThrowsValidException");

            IInterfaceA proxy = container.Resolve<IInterfaceA>();
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void InterceptorsTestWithDefaults()
        {
            IUnityContainer container = GetContainer("TestDefaults");

            IInterfaceTest proxy = container.Resolve<IInterfaceTest>();
            string input = String.Empty;
            string returnValue = proxy.TargetMethod(ref input);

            Assert.IsTrue(RemotingServices.IsTransparentProxy(proxy));
            Assert.AreEqual<string>("PreAtargetPostA", input);

            IInterfaceTest proxy1 = container.Resolve<IInterfaceTest>("WithNameA");
            string input1 = String.Empty;
            string returnValue1 = proxy1.TargetMethod(ref input1);

            Assert.IsFalse(RemotingServices.IsTransparentProxy(proxy1));
            Assert.AreEqual<string>("target", input1);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void TestDefaultNotAppliedForDifferentInterfaceImplementations()
        {
            IUnityContainer container = GetContainer("TestDefaultNotAppliedForDifferentInterfaceImplementations");

            IInterfaceB proxy = container.Resolve<IInterfaceB>();
            string input = String.Empty;
            string returnValue = proxy.TargetMethod(ref input);

            Assert.IsTrue(RemotingServices.IsTransparentProxy(proxy));
            Assert.AreEqual<string>("PreAtargetPostA", input);

            IInterfaceB proxy1 = container.Resolve<IInterfaceB>("WithNameA");
            string input1 = String.Empty;
            string returnValue1 = proxy1.TargetMethod(ref input1);

            Assert.IsFalse(RemotingServices.IsTransparentProxy(proxy1));
            Assert.AreEqual<string>("target", input1);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void OverrideDefault()
        {
            IUnityContainer container = GetContainer("OverrideDefault");

            TestClassForMultipleInterception proxy = container.Resolve<TestClassForMultipleInterception>();
            string input = String.Empty;
            string returnValue = proxy.TargetMethod(ref input);

            Assert.IsTrue(RemotingServices.IsTransparentProxy(proxy));
            Assert.AreEqual<string>("PreAtargetPostA", input);

            TestClassForMultipleInterception proxy1 = container.Resolve<TestClassForMultipleInterception>("virtual");
            string input1 = String.Empty;
            string returnValue1 = proxy1.ThirdTargetMethod(ref input1);

            Assert.IsFalse(RemotingServices.IsTransparentProxy(proxy1));
            Assert.IsTrue(input1.Contains("PreBtarget"));
            Assert.IsTrue(returnValue1.Contains("PreBtarget"));

            TestClassForMultipleInterception proxy3 = container.Resolve<TestClassForMultipleInterception>("changedbehaviourfromdefault");
            string input3 = String.Empty;
            string returnValue3 = proxy3.TargetMethod(ref input3);

            Assert.IsTrue(RemotingServices.IsTransparentProxy(proxy3));
            Assert.AreEqual<string>("PreCtargetPostC", input3);

            TestClassForMultipleInterception proxy2 = container.Resolve<TestClassForMultipleInterception>("shouldbetransparentproxy");
            string input2 = String.Empty;
            string returnValue2 = proxy2.TargetMethod(ref input2);

            Assert.IsTrue(RemotingServices.IsTransparentProxy(proxy2));
            Assert.AreEqual<string>("PreBtargetPostB", input2);
        }

        //pagtfs2008 bug# 3971 reported in http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=3448
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VirtualMethodInterceptionBugWithInputAndReturnValues()
        {
            IUnityContainer container = GetContainer("VirtualInterceptorCallHandlerUsingInputOrReturnValueCollection");

            HasVirtualMethodsTest proxy = container.Resolve<HasVirtualMethodsTest>();
            string inputParam1 = "inputParam1";
            string inputParam2 = "inputParam2";
            string outputParam1 = String.Empty;
            string outputParam2 = String.Empty;
            string refParam1 = "refParam1";
            string refParam2 = "refParam2";

            proxy.TargetMethod(inputParam1, inputParam2, out outputParam1, out outputParam2, ref refParam1, ref refParam2);

            Assert.AreEqual<string>("inside target method outparam1", outputParam1);
            Assert.AreEqual<string>("inside target method outparam2", outputParam2);

            Assert.AreEqual<string>("refParam1inside target method refParam1", refParam1);
            Assert.AreEqual<string>("refParam2inside target method refParam2", refParam2);
        }
    }
}