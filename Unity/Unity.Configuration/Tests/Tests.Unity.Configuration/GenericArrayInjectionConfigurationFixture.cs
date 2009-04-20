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
    public class GenericArrayInjectionConfigurationFixture : ConfigurationFixtureBase
    {
        private const string configFileName = "ConfiguringGenericArrayInjection";

        [TestMethod]
        [DeploymentItem("ConfiguringGenericArrayInjection.config")]
        public void InjectsEmptyArrayWhenNoRegisteredInstances()
        {
            GenericObjectArrayConstructorDependency<ILogger> instance
                = ResolveConfiguredObject<GenericObjectArrayConstructorDependency<ILogger>>("emptyArray");

            Assert.AreEqual(0, instance.values.Length);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringGenericArrayInjection.config")]
        public void InjectsPopulatedArrayWhenInstancesAreRegistered()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArray");

            GenericObjectArrayConstructorDependency<ILogger> instance
                = container.Resolve<GenericObjectArrayConstructorDependency<ILogger>>();

            Assert.AreEqual(2, instance.values.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.values[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.values[1]);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringGenericArrayInjection.config")]
        public void DefaultRegisteredInstancesAreIgnoredByArrayParameter()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArrayWithDefaultInstance");

            GenericObjectArrayConstructorDependency<ILogger> instance
                = container.Resolve<GenericObjectArrayConstructorDependency<ILogger>>();

            Assert.AreEqual(2, instance.values.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.values[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.values[1]);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringGenericArrayInjection.config")]
        public void InjectsPopulatedArrayWhenSpecifyingValues()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArrayWithValues");

            GenericObjectArrayConstructorDependency<ILogger> instance
                = container.Resolve<GenericObjectArrayConstructorDependency<ILogger>>();

            Assert.AreEqual(2, instance.values.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.values[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.values[1]);
            Assert.IsNotNull(container.Resolve<ILogger>("logger3"));    // registered, but not in the array
        }

        [TestMethod]
        [DeploymentItem("ConfiguringGenericArrayInjection.config")]
        public void InjectsEmptyArrayToPropertyWhenNoRegisteredInstances()
        {
            GenericObjectArrayPropertyDependency<ILogger> instance
                = ResolveConfiguredObject<GenericObjectArrayPropertyDependency<ILogger>>("emptyArrayForProperty");

            Assert.AreEqual(0, instance.Values.Length);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringGenericArrayInjection.config")]
        public void InjectsPopulatedArrayToPropertyWhenInstancesAreRegistered()
        {
            IUnityContainer container = GetConfiguredContainer("populatedArrayForProperty");

            GenericObjectArrayPropertyDependency<ILogger> instance
                = container.Resolve<GenericObjectArrayPropertyDependency<ILogger>>();

            Assert.AreEqual(2, instance.Values.Length);
            Assert.AreSame(container.Resolve<ILogger>("logger1"), instance.Values[0]);
            Assert.AreSame(container.Resolve<ILogger>("logger2"), instance.Values[1]);
        }

        protected override string ConfigFileName
        {
            get { return configFileName; }
        }
    }
}
