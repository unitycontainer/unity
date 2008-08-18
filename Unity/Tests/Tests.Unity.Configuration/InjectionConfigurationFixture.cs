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

using System.Configuration;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysConfiguration = System.Configuration.Configuration;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    [TestClass]
    public class InjectionConfigurationFixture
    {
        private const string configFileName = "ConfiguringInjectionConstructor";

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void MockDatabaseIsConfiguredToCallDefaultConstructor()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("defaultConstructor");

            Assert.IsTrue(string.IsNullOrEmpty(db.ConnectionString));
            Assert.IsTrue(db.DefaultConstructorCalled);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void MockDatabaseIsInitializedWithConnectionString()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("oneParameterConstructor");

            Assert.IsFalse(db.DefaultConstructorCalled);
            Assert.AreEqual("Northwind", db.ConnectionString);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigureDependencyParametersForConstructor()
        {
            ObjectWithOneConstructorDependency obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency>(
                    "oneDependencyParameterConstructor");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigureMultipleConstructorParameters()
        {
            ObjectWithTwoConstructorParameters obj =
                ResolveConfiguredObject<ObjectWithTwoConstructorParameters>(
                    "twoConstructorParameters");

            Assert.IsNotNull(obj.ConnectionString);
            Assert.IsNotNull(obj.Logger);

            Assert.AreEqual("AdventureWorks", obj.ConnectionString);
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));
            
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigurePropertyInjection()
        {
            ObjectUsingLogger obj = ResolveConfiguredObject<ObjectUsingLogger>("injectionProperty");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanInjectMultipleProperties()
        {
            ObjectWithTwoProperties obj =
                ResolveConfiguredObject<ObjectWithTwoProperties>("multipleProperties");

            Assert.IsInstanceOfType(obj.Obj1, typeof(SpecialLogger));
            Assert.AreEqual("Hello", obj.Obj2);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void InjectionMethodsAreCalled()
        {
            ObjectWithInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithInjectionMethod>("method");

            Assert.AreEqual(obj.ConnectionString, "contoso");
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigureInjectionForNamedInstances()
        {
            MockDatabase db =
                ResolveConfiguredObject<MockDatabase>("injectNamed", "Northwind");

            Assert.AreEqual("Northwind", db.ConnectionString);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void ConfiguringNamedInjectionDoesntAffectDefault()
        {
            IUnityContainer container = GetConfiguredContainer("injectNamed");
            MockDatabase defaultDb = container.Resolve<MockDatabase>();
            MockDatabase nwDb = container.Resolve<MockDatabase>("Northwind");

            Assert.AreEqual("contoso", defaultDb.ConnectionString);
            Assert.AreEqual("Northwind", nwDb.ConnectionString);
        }


        private TObj ResolveConfiguredObject<TObj>(string containerName)
        {
            IUnityContainer container = GetConfiguredContainer(containerName);
            return container.Resolve<TObj>();
        }

        private TObj ResolveConfiguredObject<TObj>(string containerName, string name)
        {
            IUnityContainer container = GetConfiguredContainer(containerName);
            return container.Resolve<TObj>(name);
        }

        private IUnityContainer GetConfiguredContainer(string containerName)
        {
            UnityConfigurationSection section = GetUnitySection(configFileName);
            IUnityContainer container = new UnityContainer();
            section.Containers[containerName].Configure(container);
            return container;
        }

        private UnityConfigurationSection GetUnitySection(string baseName)
        {
            SysConfiguration config = OpenConfigFile(baseName);
            return (UnityConfigurationSection)config.GetSection("unity");
        }

        private SysConfiguration OpenConfigFile(string baseName)
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = baseName + ".config";
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
    }
}
