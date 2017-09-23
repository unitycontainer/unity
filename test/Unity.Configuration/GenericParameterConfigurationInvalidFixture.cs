// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Configuration;

namespace Unity.Tests.Generics
{
    [TestClass]
    public class GenericParameterConfigurationInvalidFixture 
    {
        // TODO: Verify
        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid1.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid1()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid1.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid2.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid2()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid2.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid3.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid3()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid3.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid4.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid4()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid4.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid5.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid5()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid5.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid6.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid6()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid6.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid7.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid7()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid7.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid8.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid8()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid8.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid9.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid9()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid9.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid10.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid10()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid10.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid11.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid11()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid11.config");

        //    Assert.IsNotNull(section);
        //}

        ///// <summary>
        ///// Verifies that configuration is invalid
        ///// </summary>
        //[TestMethod]
        //[DeploymentItem(@"ConfigFiles\GenericParameterConfigurationTestsInvalid12.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void ConfigurationIsInvalid12()
        //{
        //    UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(@"ConfigFiles\GenericParameterConfigurationTestsInvalid12.config");

        //    Assert.IsNotNull(section);
        //}
    }
}
