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
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    [TestClass]
    public class ArrayInjectionConfigurationFixture : ConfigurationFixtureBase
    {
        private const string configFileName = "ConfiguringArrayInjection";

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void InjectsEmptyArrayWhenNoRegisteredInstances()
        {
            ObjectArrayConstructorDependency instance
                = ResolveConfiguredObject<ObjectArrayConstructorDependency>("emptyArray");

            Assert.AreEqual(0, instance.loggers.Length);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void InjectsPopulatedArrayWhenInstancesAreRegistered()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArray");

            ObjectArrayConstructorDependency instance = container.Resolve<ObjectArrayConstructorDependency>();

            Assert.AreEqual(2, instance.loggers.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.loggers[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.loggers[1]);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void DefaultRegisteredInstancesAreIgnoredByArrayParameter()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArrayWithDefaultInstance");

            ObjectArrayConstructorDependency instance = container.Resolve<ObjectArrayConstructorDependency>();

            Assert.AreEqual(2, instance.loggers.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.loggers[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.loggers[1]);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void InjectsPopulatedArrayWhenSpecifyingValues()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArrayWithValues");

            ObjectArrayConstructorDependency instance = container.Resolve<ObjectArrayConstructorDependency>();

            Assert.AreEqual(2, instance.loggers.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.loggers[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.loggers[1]);
            Assert.IsNotNull(container.Resolve<ILogger>("logger3"));    // registered, but not in the array
        }

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void AttemptToUseArrayValueElementForNonArrayParameterThrows()
        {
            UnityConfigurationSection section = GetUnitySection("ConfiguringArrayInjection");

            IUnityContainer container = new UnityContainer();

            try
            {
                section.Containers["constructorParameterNotArray"].Configure(container);
                Assert.Fail("Call to Configure() should have failed");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void InjectsEmptyArrayToPropertyWhenNoRegisteredInstances()
        {
            ObjectArrayPropertyDependency instance
                = ResolveConfiguredObject<ObjectArrayPropertyDependency>("emptyArrayForProperty");

            Assert.AreEqual(0, instance.Loggers.Length);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void InjectsPopulatedArrayToPropertyWhenInstancesAreRegistered()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArrayForProperty");

            ObjectArrayPropertyDependency instance = container.Resolve<ObjectArrayPropertyDependency>();

            Assert.AreEqual(2, instance.Loggers.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.Loggers[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.Loggers[1]);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringArrayInjection.config")]
        public void InjectsEmptyArrayWhenArrayElementIsEmpty()
        {
            IUnityContainer container = GetConfiguredContainer("explicitlyInjectingEmptyArray");

            ObjectArrayPropertyDependency instance = container.Resolve<ObjectArrayPropertyDependency>();

            Assert.AreEqual(0, instance.Loggers.Length);
        }


        protected override string ConfigFileName
        {
            get { return configFileName; }
        }
    }
}