using System;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForSpecificConstructors
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForSpecificConstructors : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForSpecificConstructors() : base("VariousConstructors")
        {
        }

        private IUnityContainer container;

        protected override void Arrange()
        {
            base.Arrange();
            container = new UnityContainer();
        }

        [TestMethod]
        public void Then_CanResolveMockDatabaseAndItCallsDefaultConstructor()
        {
            Section.Configure(container, "defaultConstructor");
            var result = container.Resolve<MockDatabase>();
            Assert.IsTrue(result.DefaultConstructorCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Then_ConstructorsThatDoNotMatchThrowAnException()
        {
            Section.Configure(container, "invalidConstructor");
        }
    }
}
