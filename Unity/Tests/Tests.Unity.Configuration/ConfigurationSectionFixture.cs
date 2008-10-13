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
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysConfiguration = System.Configuration.Configuration;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Tests for our Unity configuration section
    /// </summary>
    [TestClass]
    public class ConfigurationSectionFixture
    {
        [TestMethod]
        [DeploymentItem("ContainersWithTypesAlias.config")]
        public void CanResolveTypeAliases()
        {
            using (TypeCreator helper = new TypeCreator("ContainersWithTypesAlias.config"))
            {
                ConfigurationSectionFixtureProxy proxy = helper.CreateInstance<ConfigurationSectionFixtureProxy>();
                proxy.CanResolveTypeAliases();
            }
        }

        [TestMethod]
        [DeploymentItem("Basic.config")]
        public void CanReadSectionFromConfig()
        {
            SysConfiguration config = OpenConfigFile("Basic");

            // Let's make sure it's reading the config file at all
            Assert.AreEqual("Its a value", config.AppSettings.Settings["test1"].Value);

            UnityConfigurationSection unitySection =
                (UnityConfigurationSection)config.GetSection("unity");
            Assert.IsNotNull(unitySection);
        }

        [TestMethod]
        [DeploymentItem("Basic.config")]
        public void SectionIncludesContainersElement()
        {
            UnityConfigurationSection section = GetUnitySection("Basic");
            Assert.IsNotNull(section.Containers);
        }

        [TestMethod]
        [DeploymentItem("Basic.config")]
        public void SectionContainsContainersElements()
        {
            UnityConfigurationSection section = GetUnitySection("Basic");

            Assert.AreEqual(3, section.Containers.Count);
            string[] expectedNames = { "one", "two", "three" };
            foreach (string name in expectedNames)
            {
                Assert.IsNotNull(section.Containers[name]);
            }
        }

        [TestMethod]
        [DeploymentItem("ContainersWithTypeConfig.config")]
        public void CanAccessTypeConfig()
        {
            using (TypeCreator helper = new TypeCreator("ContainersWithTypeConfig.config"))
            {
                ConfigurationSectionFixtureProxy proxy = helper.CreateInstance<ConfigurationSectionFixtureProxy>();
                proxy.CanAccessTypeConfig();
            }
        }

        [TestMethod]
        [DeploymentItem("ContainersWithTypesAlias.config")]
        public void ContainerCanSpecifyInformationAboutTypes()
        {
            using (TypeCreator helper = new TypeCreator("ContainersWithTypesAlias.config"))
            {
                ConfigurationSectionFixtureProxy proxy = helper.CreateInstance<ConfigurationSectionFixtureProxy>();
                proxy.ContainerCanSpecifyInformationAboutTypes();
            }
        }

        [TestMethod]
        [DeploymentItem("ContainersWithTypesAlias.config")]
        public void UnityConfigurationCanSpecifyInformationAboutTypesAlias()
        {
            UnityConfigurationSection section = GetUnitySection("ContainersWithTypesAlias");

            Assert.AreEqual(2, section.TypeAliases.Count);

            UnityTypeAlias typeAlias = section.TypeAliases[0];

            Assert.AreEqual("ILogger", typeAlias.Alias);
            Assert.AreSame(typeof(ILogger), typeAlias.Type);
        }

        [TestMethod]
        [DeploymentItem("ContainersWithTypesAlias.config")]
        public void TypeElementUsesAlias()
        {
            using (TypeCreator helper = new TypeCreator("ContainersWithTypesAlias.config"))
            {
                ConfigurationSectionFixtureProxy proxy = helper.CreateInstance<ConfigurationSectionFixtureProxy>();
                proxy.TypeElementUsesAlias();
            }
        }

        [TestMethod]
        [DeploymentItem("UnnamedContainers.config")]
        public void ConfigFileCanSpecifyContainerWithoutAName()
        {
            UnityConfigurationSection section = GetUnitySection("UnnamedContainers");
            UnityContainerElement ce = section.Containers.Default;
            UnityContainerElement ce2 = section.Containers["one"];
            Assert.IsNotNull(ce);
            Assert.IsNotNull(ce2);

            Assert.AreEqual(1, ce.Types.Count);
        }

        [TestMethod]
        [DeploymentItem("UnnamedContainers.config")]
        public void CanSetLifetimeToSingleton()
        {
            UnityConfigurationSection section = GetUnitySection("UnnamedContainers");
            UnityContainerElement ce = section.Containers.Default;
            UnityTypeElement te = ce.Types[0];

            Assert.AreEqual(typeof(ContainerControlledLifetimeManager), te.Lifetime.Type);
        }

        [TestMethod]
        [DeploymentItem("UnnamedContainers.config")]
        public void ContainerGeneratesExpectedConfiguration()
        {
            TestForExpectedActions("UnnamedContainers", Sequence.Collect(
                                                            ConfigurationActionRecord.RegisterAction(typeof(ILogger), typeof(SpecialLogger), null, new ContainerControlledLifetimeManager())
                                                            ));
        }

        [TestMethod]
        [DeploymentItem("ContainerExtensions.config")]
        public void CanAddContainerExtensionViaConfig()
        {
            TestForExpectedActions("ContainerExtensions", Sequence.Collect(
                                                              ConfigurationActionRecord.AddExtensionAction(typeof(MockContainerExtension)),
                                                              ConfigurationActionRecord.RegisterAction(typeof(ILogger), typeof(SpecialLogger), null, new ContainerControlledLifetimeManager())
                                                              ));
        }

        [TestMethod]
        [DeploymentItem("RegisteringOneInstance.config")]
        public void CanAddDefaultStringInstanceViaConfiguration()
        {
            TestForExpectedActions("RegisteringOneInstance",
                                   Sequence.Collect(
                                       ConfigurationActionRecord.RegisterInstanceAction(typeof(string), "database", "Northwind", new ContainerControlledLifetimeManager())
                                       ));
        }

        [TestMethod]
        [DeploymentItem("RegisteringInstances.config")]
        public void CanRegisterInstancesWithTypeConverters()
        {
            TestForExpectedActions("RegisteringInstances",
                                   Sequence.Collect(
                                       ConfigurationActionRecord.RegisterInstanceAction(typeof(string),
                                                                                        null,
                                                                                        "AdventureWorks",
                                                                                        new ContainerControlledLifetimeManager()),
                                       ConfigurationActionRecord.RegisterInstanceAction(typeof(int),
                                                                                        null,
                                                                                        42,
                                                                                        new ContainerControlledLifetimeManager()),
                                       ConfigurationActionRecord.RegisterInstanceAction(typeof(int),
                                                                                        "backwards",
                                                                                        -23,
                                                                                        new ContainerControlledLifetimeManager()),
                                       ConfigurationActionRecord.RegisterInstanceAction(typeof(int),
                                                                                        "forward",
                                                                                        23,
                                                                                        new ContainerControlledLifetimeManager())));
        }

        [TestMethod]
        [DeploymentItem("ExtensionConfig.config")]
        public void CanIncludeExtensionConfigInContainerConfig()
        {
            TestForExpectedActions("ExtensionConfig",
                                   Sequence.Collect(
                                       ConfigurationActionRecord.ConfigureExtensionOneAction("Extension One"),
                                       ConfigurationActionRecord.ConfigureExtensionTwoAction("Extension Two")));
        }

        [TestMethod]
        [DeploymentItem("Lifetimes.config")]
        public void CanSpecifyLifetimeForType()
        {
            TestForExpectedActions("Lifetimes",
                                   Sequence.Collect(
                                       ConfigurationActionRecord.RegisterAction(
                                           typeof(ILogger), typeof(SpecialLogger), null,
                                           new ContainerControlledLifetimeManager()),
                                       ConfigurationActionRecord.RegisterAction(
                                           typeof(ILogger), typeof(MockLogger), "mock",
                                           new ExternallyControlledLifetimeManager()),
                                       ConfigurationActionRecord.RegisterAction(
                                           typeof(ILogger), typeof(MockLogger), "session",
                                           new SessionLifetimeManager("sessionKey")),
                                       ConfigurationActionRecord.RegisterAction(
                                           typeof(ILogger), typeof(MockLogger), "reverseSession",
                                           new SessionLifetimeManager("sdrawkcab")),
                                       ConfigurationActionRecord.RegisterAction(
                                           typeof(ILogger), typeof(MockLogger), "transient", null),
                                       ConfigurationActionRecord.RegisterAction(
                                           typeof(ILogger), typeof(MockLogger), "explicitTransient", null)
                                       )
                );
        }

        [TestMethod]
        [DeploymentItem("Lifetimes.config")]
        public void CanUseTypeConverterToSpecifyLifetime()
        {
            UnityConfigurationSection section = GetUnitySection("Lifetimes");
            UnityTypeElement typeElement = section.Containers.Default.Types[2];

            SessionLifetimeManager lifetime =
                (SessionLifetimeManager)typeElement.Lifetime.CreateLifetimeManager();
            Assert.AreEqual("sessionKey", lifetime.SessionKey);
        }

        [TestMethod]
        [DeploymentItem("Lifetimes.config")]
        public void CanUseCustomTypeConverterToSpecifyLifetime()
        {
            UnityConfigurationSection section = GetUnitySection("Lifetimes");
            UnityTypeElement typeElement = section.Containers.Default.Types[3];

            SessionLifetimeManager lifetime =
                (SessionLifetimeManager)typeElement.Lifetime.CreateLifetimeManager();
            Assert.AreEqual("sdrawkcab", lifetime.SessionKey);
        }

        [TestMethod]
        [DeploymentItem("Lifetimes.config")]
        public void EmptyStringForLifetimeTypeGivesNullLifetimeManager()
        {
            UnityConfigurationSection section = GetUnitySection("Lifetimes");
            UnityTypeElement typeElement = section.Containers.Default.Types[4];

            LifetimeManager lifetime = typeElement.Lifetime.CreateLifetimeManager();
            Assert.IsNull(lifetime);
        }

        private void TestForExpectedActions(string configName, ConfigurationActionRecord[] expectedActions)
        {
            UnityContainerElement element = GetUnitySection(configName).Containers.Default;
            MockUnityContainer container = new MockUnityContainer();
            element.Configure(container);

            CollectionAssert.AreEqual(expectedActions, container.ConfigActions);
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

    internal class ConfigurationSectionFixtureProxy : MarshalByRefObject
    {
        internal void TypeElementUsesAlias()
        {
            UnityConfigurationSection section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;

            Assert.AreEqual(2, section.TypeAliases.Count);

            UnityTypeAlias typeAlias = section.TypeAliases[0];
            UnityTypeAlias mapToTypeAlias = section.TypeAliases[1];

            Assert.AreEqual("ILogger", typeAlias.Alias);
            Assert.AreSame(typeof(ILogger), typeAlias.Type);

            UnityContainerElement ce = section.Containers[0];

            Assert.AreEqual(3, ce.Types.Count);

            // Check Type
            UnityTypeElement te = section.Containers[0].Types[1];
            Assert.AreSame(typeAlias.Type, te.Type);

            // Check MapTo
            UnityTypeElement te2 = section.Containers[0].Types[2];
            Assert.AreSame(mapToTypeAlias.Type, te2.MapTo);
        }

        internal void ContainerCanSpecifyInformationAboutTypes()
        {
            UnityConfigurationSection section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
            UnityContainerElement ce = section.Containers[0];

            UnityTypeElement te = section.Containers[0].Types[2];
            Assert.AreSame(typeof(ILogger), te.Type);
            Assert.AreEqual("mapToAlias", te.Name);
            Assert.AreSame(typeof(MockLogger), te.MapTo);
        }

        internal void CanResolveTypeAliases()
        {
            UnityConfigurationSection section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;

            Assert.AreEqual(1, section.Containers.Count);

            UnityTypeResolver typeResolver = section.Containers[0].TypeResolver;

            Assert.IsNotNull(typeResolver);

            Type alias = typeResolver.ResolveType("ILogger");
            Assert.AreSame(typeof(ILogger), alias);

            Type fullName = typeResolver.ResolveType("Microsoft.Practices.Unity.TestSupport.MockLogger, TestSupport.Unity");
            Assert.AreSame(typeof(MockLogger), fullName);
        }

        internal void CanAccessTypeConfig()
        {
            UnityConfigurationSection section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
            UnityContainerElement ce = section.Containers[0];

            UnityContainerTypeConfigurationElement te = section.Containers[0].Types[0].TypeConfig["Microsoft.Practices.Unity.TestSupport.TypeConfigMock, Tests.Unity.Configuration"];

            Type extensionType = Type.GetType(te.ExtensionType, true);

            Assert.AreSame(typeof(TypeConfigMock), extensionType);
        }
    }

    internal class TypeCreator : IDisposable
    {
        private const string defaultAppDomainName = "Test Domain";
        private AppDomain testDomain;
        private string configurationFile;
        private string appDomainName;

        public string ConfigurationFile
        {
            get { return this.configurationFile; }
        }

        public string AppDomainName
        {
            get { return appDomainName; }
        }

        public TypeCreator()
            : this(defaultAppDomainName, string.Empty)
        { }

        public TypeCreator(string configurationFile)
            : this(defaultAppDomainName, configurationFile)
        {
        }

        public TypeCreator(string appDomainName, string configurationFile)
        {
            this.configurationFile = configurationFile;
            this.appDomainName = appDomainName;

            Initialize();
        }

        public T CreateInstance<T>()
        {
            try
            {
                string typeFullName = typeof(T).FullName;
                T proxy = (T)testDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeFullName);
                return proxy;
            }
            catch
            {
                return default(T);
            }
        }

        private void Initialize()
        {
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = Environment.CurrentDirectory;
            if (string.IsNullOrEmpty(configurationFile))
            {
                ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            }
            else
            {
                ads.ConfigurationFile = ads.ApplicationBase + "\\" + configurationFile;
            }

            testDomain = AppDomain.CreateDomain(this.appDomainName, AppDomain.CurrentDomain.Evidence, ads);
        }
        #region IDisposable Members

        public void Dispose()
        {
            if (testDomain != null)
            {
                AppDomain.Unload(testDomain);
            }
        }

        #endregion
    }
}
