// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.CollectionSupport
{
    [TestClass]
    public class CollectionSupportConfigurationFileFixture : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\CollectionSupportConfigurationFileFixture.config";

        public CollectionSupportConfigurationFileFixture()
            : base(ConfigFileName)
        { }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfiguringPropertyInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = GetContainer("ConfiguringPropertyInjectionYieldsProperlyInjectedObject");

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(3, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfiguringPropertyInjectionForArrayPropertyWithNoElementsYieldsProperlyInjectedObject()
        {
            IUnityContainer container = GetContainer("ConfiguringPropertyInjectionForArrayPropertyWithNoElementsYieldsProperlyInjectedObject");

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(0, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfiguringGenericPropertyInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = GetContainer("ConfiguringGenericPropertyInjectionYieldsProperlyInjectedObject");

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(3, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, @"ConfigFiles")]
        public void ConfiguringCtorInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = GetContainer("ConfiguringCtorInjectionYieldsProperlyInjectedObject");

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(3, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        [DeploymentItem(@"CollectionSupport\CollectionSupportConfigurationFileFixture.config", @"CollectionSupport")]
        public void ConfiguringGenericCtorInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = GetContainer("ConfiguringGenericCtorInjectionYieldsProperlyInjectedObject");

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(3, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, @"ConfigFiles")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConfiguringGenericCtorWithInvalidItemThrows()
        {
            IUnityContainer container = GetContainer("ConfiguringGenericCtorWithInvalidItemThrows");

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();
            Assert.AreEqual(2, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, @"ConfigFiles")]
        public void ConfiguringMethodInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = GetContainer("ConfiguringMethodInjectionYieldsProperlyInjectedObject");

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(2, resolved.ArrayMethod.Length);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, @"ConfigFiles")]
        public void ConfiguringGenericMethodInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = GetContainer("ConfiguringGenericMethodInjectionYieldsProperlyInjectedObject");

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(2, resolved.ArrayMethod.Length);
        }
    }
}
