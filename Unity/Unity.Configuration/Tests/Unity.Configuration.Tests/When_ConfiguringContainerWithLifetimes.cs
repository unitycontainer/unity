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

using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithLifetimes
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithLifetimes : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithLifetimes() : base("Lifetimes")
        {
        }

        private IUnityContainer container;

        protected override void Arrange()
        {
            base.Arrange();
            container = new UnityContainer();
        }

        protected override void Act()
        {
            base.Act();

            Section.Configure(container);
        }

        [TestMethod]
        public void Then_BaseILoggerHasSingletonLifetime()
        {
            AssertRegistration<ILogger>(null).HasLifetime<ContainerControlledLifetimeManager>();
        }

        [TestMethod]
        public void Then_MockLoggerHasExternalLifetime()
        {
            AssertRegistration<ILogger>("mock").HasLifetime<ExternallyControlledLifetimeManager>();
        }

        [TestMethod]
        public void Then_SessionLoggerHasSessionLifetime()
        {
            AssertRegistration<ILogger>("session").HasLifetime<SessionLifetimeManager>();
        }

        [TestMethod]
        public void Then_ReverseSessionLoggerHasSessionLifetime()
        {
            AssertRegistration<ILogger>("reverseSession").HasLifetime<SessionLifetimeManager>();
        }

        [TestMethod]
        public void Then_ReverseSessionLoggerLifetimeWasInitializedUsingTypeConverter()
        {
            AssertRegistration<ILogger>("reverseSession").LifetimeHasSessionKey("sdrawkcab");
        }

        [TestMethod]
        public void Then_RegistrationWithoutExplicitLifetimeIsTransient()
        {
            AssertRegistration<ILogger>("transient").HasLifetime<TransientLifetimeManager>();
        }

        [TestMethod]
        public void Then_RegistrationWithEmptyLifetimeTypeIsTransient()
        {
            AssertRegistration<ILogger>("explicitTransient").HasLifetime<TransientLifetimeManager>();
        }

        private RegistrationsToAssertOn AssertRegistration<TRegisterType>(string registeredName)
        {
            return new RegistrationsToAssertOn(
                container.Registrations
                    .Where(r => r.RegisteredType == typeof (TRegisterType) && r.Name == registeredName));
        }
    }

    internal  static partial class RegistrationsToAssertOnExtensions
    {
        public static void LifetimeHasSessionKey(this RegistrationsToAssertOn r, string sessionKey)
        {
            Assert.IsTrue(
                r.Registrations.All(reg => ((SessionLifetimeManager) reg.LifetimeManager).SessionKey == sessionKey));

        }
    }
}
