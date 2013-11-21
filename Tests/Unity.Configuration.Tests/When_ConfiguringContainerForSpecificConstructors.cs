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
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
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
        public void Then_ConstructorsThatDoNotMatchThrowAnException()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    Section.Configure(container, "invalidConstructor");
                });
        }

        // Disable obsolete warning for this one test
#pragma warning disable 618
        [TestMethod]
        public void Then_OldConfigureAPIStillWorks()
        {
            Section.Containers["defaultConstructor"].Configure(container);
            var result = container.Resolve<MockDatabase>();
            Assert.IsTrue(result.DefaultConstructorCalled);
        }
#pragma warning restore 618
    }
}
