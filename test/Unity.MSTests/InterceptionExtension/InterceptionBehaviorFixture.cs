// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.UI.HtmlControls;

using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.InterceptionExtension
{
    [TestClass]
    public class GivenInterceptionBehaviorFixtureValidConfiguration : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\InterceptionBehavior.config";

        private static readonly Type DummyType =
            typeof(Microsoft.Practices.Unity.InterceptionExtension.Configuration.InterceptionConfigurationExtension);

        public GivenInterceptionBehaviorFixtureValidConfiguration()
            : base(ConfigFileName)
        { }

        [TestMethod]
        public void WhenClassWithMissingInterfaceImplmementationIsIntercepted()
        {
            bool intercepted = false;

            var baseClass = Intercept.NewInstance<MyBaseClass>(
                new VirtualMethodInterceptor(),
                new[] { new MyInterceptionBehavior(() => intercepted = true) },
                Type.EmptyTypes);

            var result = baseClass.DoSomething();

            Assert.IsTrue(intercepted);
        }

        [TestMethod]
        public void WhenAbstractClassIsIntercepted()
        {
            bool intercepted = false;
            var myBaseClass = Intercept.NewInstance<MyBaseClass>(new VirtualMethodInterceptor(),
                new[] { new ActionInterceptionBehavior(() => intercepted = true) });

            myBaseClass.DoSomething();

            Assert.IsTrue(intercepted);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInterceptationBehaviorUsesIncorrectlyTheRequiredInterfaces()
        {
            Intercept.NewInstance<MyBaseClass>(
                new VirtualMethodInterceptor(),
                new[] { new BadInterceptionBehavior() },
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInterceptationBehaviorUsesGenericDefinitionAsRequiredInterfaces()
        {
            Intercept.NewInstance<MyBaseClass>(
                new VirtualMethodInterceptor(),
                new[] { new InterceptionBehaviorWithGenericClass<BadInterceptionBehavior>() },
                Type.EmptyTypes);
        }

        [TestMethod]
        public void CanInterceptClassWithReservedTypeAttributes()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(HtmlInputText));

            Type generatedType = generator.GenerateType();

            HtmlInputText instance = (HtmlInputText)Activator.CreateInstance(generatedType);

            bool intercepted = false;

            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new ActionInterceptionBehavior(() => intercepted = true));

            var result = instance.HasControls();

            Assert.IsTrue(intercepted);
        }

        [TestMethod]
        public void CanInterceptNonPublicPropertiesUsingVirtualInterceptor()
        {
            VirtualMethodInterceptor interceptor =
                new VirtualMethodInterceptor();

            Type generatedType = interceptor.CreateProxyType(typeof(ClassWithNonPublicVirtualProperties));

            ClassWithNonPublicVirtualProperties instance = (ClassWithNonPublicVirtualProperties)Activator.CreateInstance(generatedType);

            bool intercepted = false;

            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                new ActionInterceptionBehavior(() => intercepted = true));

            var result = instance.GetSomePropertyValue();

            Assert.IsTrue(intercepted);
        }

        

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void WhenTypeWithInjectedPropertiesIsIntercepted()
        {
            IUnityContainer container = GetContainer(); 

            var mock = container.Resolve<ClassWithInjectedStuff>();

            Assert.IsTrue(mock.ConstructorInvoked);
            Assert.AreEqual("property data from config", mock.Data);
        }
    }
}