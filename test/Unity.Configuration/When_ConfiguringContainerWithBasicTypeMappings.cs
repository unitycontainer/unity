// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithBasicTypeMappings
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithBasicTypeMappings : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithBasicTypeMappings()
            : base("BasicTypeMapping")
        {
        }

        private IUnityContainer container;

        protected override void Arrange()
        {
            base.Arrange();
            this.container = new UnityContainer();
        }

        protected override void Act()
        {
            this.section.Configure(this.container);
        }

        [TestMethod]
        public void Then_ContainerHasTwoMappingsForILogger()
        {
            Assert.AreEqual(2,
               this.container.Registrations.Where(r => r.RegisteredType == typeof(ILogger)).Count());
        }

        [TestMethod]
        public void Then_DefaultILoggerIsMappedToMockLogger()
        {
            Assert.AreEqual(typeof(MockLogger),
               this.container.Registrations
                    .Where(r => r.RegisteredType == typeof(ILogger) && r.Name == null)
                    .Select(r => r.MappedToType)
                    .First());
        }

        [TestMethod]
        public void Then_SpecialILoggerIsMappedToSpecialLogger()
        {
            Assert.AreEqual(typeof(SpecialLogger),
               this.container.Registrations
                    .Where(r => r.RegisteredType == typeof(ILogger) && r.Name == "special")
                    .Select(r => r.MappedToType)
                    .First());
        }

        [TestMethod]
        public void Then_AllRegistrationsHaveTransientLifetime()
        {
            Assert.IsTrue(this.container.Registrations
                .Where(r => r.RegisteredType == typeof(ILogger))
                .All(r => r.LifetimeManagerType == typeof(TransientLifetimeManager)));
        }
    }
}
