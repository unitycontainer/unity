// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.IO;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.TestSupport.Configuration
{
    public class ConfigFileLoader<TResourceLocator>
    {
        private System.Configuration.Configuration configuration;

        public ConfigFileLoader(string configFileName)
        {
            DumpResourceFileToDisk(configFileName);
            LoadConfigFromFile(configFileName);
        }

        public TSection GetSection<TSection>(string sectionName)
            where TSection : ConfigurationSection
        {
            return (TSection)configuration.GetSection(sectionName);
        }

        private void LoadConfigFromFile(string configFileName)
        {
            if (!configFileName.EndsWith(".config"))
            {
                configFileName += ".config";
            }

            var fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configFileName
                };

            configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }

        private static void DumpResourceFileToDisk(string configFileName)
        {
            using (Stream resourceStream = GetResourceStream(configFileName))
            using (Stream outputStream = GetOutputStream(configFileName))
            {
                CopyStream(resourceStream, outputStream);
            }
        }

        private static Stream GetResourceStream(string configFileName)
        {
            string resourceName = Sequence.Collect(GetResourceNamespace(), configFileName, "config").JoinStrings(".");

            var currentAssembly = typeof(TResourceLocator).Assembly;
            return currentAssembly.GetManifestResourceStream(resourceName);
        }

        private static string GetResourceNamespace()
        {
            return typeof(TResourceLocator).Namespace;
        }

        private static Stream GetOutputStream(string configFileName)
        {
            string configFileDir = AppDomain.CurrentDomain.BaseDirectory;
            string configFilePath = Path.Combine(configFileDir, configFileName + ".config");

            return new FileStream(configFilePath, FileMode.Create, FileAccess.Write);
        }

        private static void CopyStream(Stream inputStream, Stream outputStream)
        {
            var buffer = new byte[4096];
            int numRead = inputStream.Read(buffer, 0, buffer.Length);
            while (numRead > 0)
            {
                outputStream.Write(buffer, 0, numRead);
                numRead = inputStream.Read(buffer, 0, buffer.Length);
            }
        }
    }
}
