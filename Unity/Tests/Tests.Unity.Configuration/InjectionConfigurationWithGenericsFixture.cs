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
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.TestSupport;

using SysConfiguration = System.Configuration.Configuration;

namespace Tests.Unity.Configuration
{
    [TestClass]
    public class InjectionConfigurationWithGenericsFixture
    {
        private const string configFileName = "ConfiguringInjectionConstructorWithGenerics";

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void CanConfigureDependencyParametersForConstructor()
        {
            ObjectWithOneConstructorDependency<ILogger> obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency<ILogger>>(
                    "oneDependencyParameterConstructor");

            Assert.IsNotNull(obj.Value);
            Assert.IsInstanceOfType(obj.Value, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void CanConfigureDependencyParametersForConstructorWithEmptyDependencyElement()
        {
            ObjectWithOneConstructorDependency<ILogger> obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency<ILogger>>(
                    "oneDependencyParameterConstructorWithEmptyDependencyValue");

            Assert.IsNotNull(obj.Value);
            Assert.IsInstanceOfType(obj.Value, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void CanConfigureDependencyParametersWithInstanceNameForConstructor()
        {
            ObjectWithOneConstructorDependency<ILogger> obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency<ILogger>>(
                    "oneDependencyParameterWithInstanceNameConstructor");

            Assert.IsNotNull(obj.Value);
            Assert.IsInstanceOfType(obj.Value, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void ConfiguringDependencyParameterWithBothTypeNameAndGenericParameterNameThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityWithGenericWithTypeAndGenericParameterName");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void ConfiguringAGenericMethodParameterWithANonDependencyValueThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityWithGenericWithNonDependencyValue");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void ConfiguringParameterWithNoParameterTypeNorGenericParameterNameThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityWithWithNeitherTypeNorGenericParameter");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void ConfiguringParameterDependencyValueWithTypeThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityWithDependencyValueWithTypeForGenericMethodParameter");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void CanConfigureGenericParameterForProperty()
        {
            ObjectWithOneProperty<ILogger> obj =
                ResolveConfiguredObject<ObjectWithOneProperty<ILogger>>(
                    "injectionProperty");

            Assert.IsNotNull(obj.Value);
            Assert.IsInstanceOfType(obj.Value, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void CanConfigureGenericParameterForPropertyWithInstanceName()
        {
            ObjectWithOneProperty<ILogger> obj =
                ResolveConfiguredObject<ObjectWithOneProperty<ILogger>>(
                    "injectionPropertyWithNamedInstance");

            Assert.IsNotNull(obj.Value);
            Assert.IsInstanceOfType(obj.Value, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void ConfiguringDependencyPropertyWithBothTypeNameAndGenericParameterNameThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityWithGenericWithTypeAndGenericParameterNameForProperty");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void ConfiguringAGenericPropertyWithANonDependencyValueThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityWithGenericWithNonDependencyValueForProperty");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructorWithGenerics.config")]
        public void ConfiguringPropertyWithNoParameterTypeNorGenericParameterNameThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityWithWithNeitherTypeNorGenericParameterForProperty");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
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

    public class ObjectWithOneConstructorDependency<T>
    {
        private T value;

        public ObjectWithOneConstructorDependency(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get { return value; }
        }
    }

    public class ObjectWithOneProperty<T>
    {
        private T value;

        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
