// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.Override
{
    [TestClass]
    public class PropertyInjectionOverrideConfigurationFixture
    {
        [TestMethod]
        [DeploymentItem(@"ConfigFiles\PropertyOverride.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        public void OverridesDoNotApplyOnDifferentNamedTypeRegistered()
        {
            // Load the container details from the config file.
            IUnityContainer container = ConfigurationFixtureBase.GetContainer(@"ConfigFiles\PropertyOverride.config",
                "PropertyOverrideContainer");

            // Resolve using the type mentioned in the config files.
            var defaultConfigResult = container.Resolve<TestTypeInConfig>("MyTestTypeToOverride");

            // The default values in the config files should be present.
            Assert.AreEqual<int>(101, defaultConfigResult.Value);
            Assert.AreEqual<int>(-111, defaultConfigResult.X);
            Assert.AreEqual<string>("DefaultFromConfigFile", defaultConfigResult.Y);

            // Create the PropertyOverrides object.
            PropertyOverrides overrides = new PropertyOverrides();
            overrides.Add("X", 9999);
            overrides.Add("Y", "Overridden");

            // Resolve using the wrong type(only construction injection info available, no property injection info) mentioned in the config files.
            // Override is ignored.
            var overriddenResult = container.Resolve<TestTypeInConfig>("DefaultConstructor", overrides);

            // The default values in the config files should be retained.
            Assert.AreEqual<int>(1001, overriddenResult.Value);
            Assert.AreEqual<int>(0, overriddenResult.X);
            Assert.AreEqual<string>(null, overriddenResult.Y);
        }

        [TestMethod]
        [DeploymentItem(@"ConfigFiles\PropertyOverride.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        public void PropertyOverrideWithDefaultInConfig()
        {
            TestTypeInConfig overrideObject = new TestTypeInConfig(666);

            IUnityContainer container = ConfigurationFixtureBase.GetContainer(@"ConfigFiles\PropertyOverride.config",
                "PropertyOverrideContainer");

            var defaultResult = container.Resolve<TestTypeInConfig>("MyTestTypeToOverride");
            PropertyOverrides overrides = new PropertyOverrides();
            overrides.Add("X", 9999);
            overrides.Add("Y", "Overridden");
            var overriddenResult = container.Resolve<TestTypeInConfig>("MyTestTypeToOverride", overrides);

            Assert.AreEqual<int>(101, defaultResult.Value);
            Assert.AreEqual<int>(-111, defaultResult.X);
            Assert.AreEqual<string>("DefaultFromConfigFile", defaultResult.Y);
            Assert.AreEqual<int>(101, overriddenResult.Value);
            Assert.AreEqual<int>(9999, overriddenResult.X);
            Assert.AreEqual<string>("Overridden", overriddenResult.Y);
        }
    }
}
