using System.Configuration;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
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
