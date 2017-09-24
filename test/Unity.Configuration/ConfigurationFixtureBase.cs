// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Collections.Generic;
using Unity.Configuration;

namespace Unity.Tests
{
    public abstract class ConfigurationFixtureBase
    {
        internal const string ConfigFilesFolder = "ConfigFiles";

        protected UnityConfigurationSection ConfigSection { get; private set; }
        protected Dictionary<string, IUnityContainer> containers = new Dictionary<string, IUnityContainer>();
        private string configFileName;

        public ConfigurationFixtureBase(string configFileName) : base()
        {
            this.configFileName = configFileName;
        }

        [TestInitialize]
        public void Initialize()
        {
            DoInitialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            DoCleanup();
        }

        protected virtual void DoCleanup()
        {
        }

        protected virtual void DoInitialize()
        {
            ConfigSection = GetUnitySection(configFileName);
        }
        
        protected IUnityContainer GetContainer()
        {
            return GetContainer(null);
        }

        protected IUnityContainer GetContainer(string containerName)
        {
            string name = containerName ?? String.Empty;

            if (!containers.ContainsKey(name))
            {
                IUnityContainer container = new UnityContainer();

                if (string.IsNullOrEmpty(name))
                {
                    ConfigSection.Configure(container);
                }
                else
                {
                    ConfigSection.Configure(container, name);
                }
                
                containers.Add(name, container);
            }

            return containers[name];
        }

        public static UnityConfigurationSection GetUnitySection(string configFileName)
        {
            if (!configFileName.EndsWith(".config"))
            {
                configFileName += ".config";
            }

            ExeConfigurationFileMap map = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = configFileName
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            return (UnityConfigurationSection)config.GetSection("unity");
        }

        public static IUnityContainer GetContainer(string configFileName, string containerName)
        {
            UnityConfigurationSection config = GetUnitySection(configFileName);
            
            IUnityContainer container = new UnityContainer();
            config.Configure(container, containerName);

            return container;
        }
    }
}
