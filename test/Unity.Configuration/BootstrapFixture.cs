// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Basic bootstrapping to confirm that our "load from resources" config
    /// file helper works.
    /// </summary>
    [TestClass]
    public class BootstrapFixture
    {
        [TestMethod]
        public void CanReadBootstrapConfig()
        {
            var loader = new ConfigFileLoader<ConfigFileLocator>("Bootstrap");
            var section = loader.GetSection<AppSettingsSection>("appSettings");
            Assert.IsNotNull(section);
            Assert.AreEqual("value", section.Settings["Test"].Value);
        }
    }
}
