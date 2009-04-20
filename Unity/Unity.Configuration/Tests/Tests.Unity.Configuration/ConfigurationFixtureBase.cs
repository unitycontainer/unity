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
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

using SysConfiguration = System.Configuration.Configuration;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    public abstract class ConfigurationFixtureBase
    {
        protected abstract string ConfigFileName { get; }

        protected TObj ResolveConfiguredObject<TObj>(string containerName)
        {
            IUnityContainer container = GetConfiguredContainer(containerName);
            return container.Resolve<TObj>();
        }

        protected TObj ResolveConfiguredObject<TObj>(string containerName, string name)
        {
            IUnityContainer container = GetConfiguredContainer(containerName);
            return container.Resolve<TObj>(name);
        }

        protected IUnityContainer GetConfiguredContainer(string containerName)
        {
            UnityConfigurationSection section = GetUnitySection(ConfigFileName);
            IUnityContainer container = new UnityContainer();
            section.Containers[containerName].Configure(container);
            return container;
        }

        protected UnityConfigurationSection GetUnitySection(string baseName)
        {
            SysConfiguration config = OpenConfigFile(baseName);
            return (UnityConfigurationSection)config.GetSection("unity");
        }

        protected SysConfiguration OpenConfigFile(string baseName)
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = baseName + ".config";
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
    }
}
