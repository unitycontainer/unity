// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForSpecificConstructors
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForSpecificConstructors : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForSpecificConstructors()
            : base("VariousConstructors")
        {
        }

        private IUnityContainer container;

        protected override void Arrange()
        {
            base.Arrange();
            this.container = new UnityContainer();
        }

        [TestMethod]
        public void Then_CanResolveMockDatabaseAndItCallsDefaultConstructor()
        {
            section.Configure(this.container, "defaultConstructor");
            var result = this.container.Resolve<MockDatabase>();
            Assert.IsTrue(result.DefaultConstructorCalled);
        }

        [TestMethod]
        public void Then_ConstructorsThatDoNotMatchThrowAnException()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    section.Configure(container, "invalidConstructor");
                });
        }

        // Disable obsolete warning for this one test
#pragma warning disable 618
        [TestMethod]
        public void Then_OldConfigureAPIStillWorks()
        {
            this.section.Containers["defaultConstructor"].Configure(this.container);
            var result = this.container.Resolve<MockDatabase>();
            Assert.IsTrue(result.DefaultConstructorCalled);
        }
#pragma warning restore 618
    }
}
