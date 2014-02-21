// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Tests.TestDoubles;

namespace Unity.Tests.ContainerRegistration
{
    [TestClass]
    public class GivenContainerRegistrationFixtureValidConfiguration : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\ContainerRegistration.config";

        public GivenContainerRegistrationFixtureValidConfiguration()
            : base(ConfigFileName)
        {
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void WhenRegistrationsAreRetrievedFromAContainerConfiguredUsingConfigFile()
        {
            var registrations = GetContainer().Registrations;

            var count = registrations.Count();

            Assert.AreEqual(5, count);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void WhenRegistrationsAreRetrievedByRegisteredTypeFromAContainerConfiguredUsingConfigFile()
        {
            var registrations = GetContainer().Registrations.Where(c => c.RegisteredType == typeof(ITypeAnotherInterface));

            var count = registrations.Count();

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void WhenRegistrationsAreRetrievedByMappedTypeFromAContainerConfiguredUsingConfigFile()
        {
            var registrations = GetContainer().Registrations.Where(c => c.MappedToType == typeof(TypeImplementation));

            var count = registrations.Count();

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void WhenRegistrationsAreRetrievedByLifeTimeManagerFromAContainerConfiguredUsingConfigFile()
        {
            var registrations = GetContainer().Registrations;

            var singleton = registrations.Where(c => c.LifetimeManagerType == typeof(ContainerControlledLifetimeManager)).Count();
            var external = registrations.Where(c => c.LifetimeManagerType == typeof(ExternallyControlledLifetimeManager)).Count();
            var transient = registrations.Where(c => c.LifetimeManagerType == typeof(TransientLifetimeManager)).Count();
            var session = registrations.Where(c => c.LifetimeManagerType == typeof(SessionLifetimeManager)).Count();

            Assert.AreEqual(1, singleton);
            Assert.AreEqual(1, external);
            Assert.AreEqual(1, transient);
            Assert.AreEqual(1, session);
        }
    }
}
