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

using SysConfiguration = System.Configuration.Configuration;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    [TestClass]
    public class ValidatingInjectionConfigurationFixture : ConfigurationFixtureBase
    {
        private const string configFileName = "ValidatingInjectionConfiguration";

        [TestMethod]
        [DeploymentItem("ValidatingInjectionConfiguration.config")]
        public void GenericParameterArrayWithDependencyThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityDependencyForGenericParameterArray");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ValidatingInjectionConfiguration.config")]
        public void GenericParameterArrayWithValueThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityValueForGenericParameterArray");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ValidatingInjectionConfiguration.config")]
        public void GenericParameterArrayWithPolymorphicValueThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityUnknownValueForGenericParameterArray");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ValidatingInjectionConfiguration.config")]
        public void GenericParameterWithValueThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityValueForGenericParameter");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ValidatingInjectionConfiguration.config")]
        public void GenericParameterWithArrayThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityArrayForGenericParameter");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        [TestMethod]
        [DeploymentItem("ValidatingInjectionConfiguration.config")]
        public void GenericParameterWithPolymorphicValueThrows()
        {
            SysConfiguration configuration = OpenConfigFile(configFileName);

            try
            {
                configuration.GetSection("unityUnknownValueForGenericParameter");
                Assert.Fail("Should have thrown ConfigurationErrorsException");
            }
            catch (ConfigurationErrorsException)
            {
            }
        }

        protected override string ConfigFileName
        {
            get { return configFileName; }
        }
    }
}
