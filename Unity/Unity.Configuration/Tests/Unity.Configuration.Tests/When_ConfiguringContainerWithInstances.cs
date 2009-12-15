using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithInstances
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithInstances : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithInstances() : base("RegisteringInstances")
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
        public void Then_DefaultStringInstanceIsRegistered()
        {
            Assert.AreEqual("AdventureWorks", container.Resolve<string>());
        }

        [TestMethod]
        public void Then_DefaultIntInstanceIsRegistered()
        {
            Assert.AreEqual(42, container.Resolve<int>());
        }

        [TestMethod]
        public void Then_NamedIntIsRegistered()
        {
            Assert.AreEqual(23, container.Resolve<int>("forward"));
        }

        [TestMethod]
        public void Then_InstanceUsingTypeConverterIsCreatedProperly()
        {
            Assert.AreEqual(-23, container.Resolve<int>("negated"));
        }
    }
}
