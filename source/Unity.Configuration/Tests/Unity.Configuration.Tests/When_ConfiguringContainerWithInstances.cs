// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithInstances
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithInstances : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithInstances()
            : base("RegisteringInstances")
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
            base.Act();
            this.section.Configure(this.container);
        }

        [TestMethod]
        public void Then_DefaultStringInstanceIsRegistered()
        {
            Assert.AreEqual("AdventureWorks", this.container.Resolve<string>());
        }

        [TestMethod]
        public void Then_DefaultIntInstanceIsRegistered()
        {
            Assert.AreEqual(42, this.container.Resolve<int>());
        }

        [TestMethod]
        public void Then_NamedIntIsRegistered()
        {
            Assert.AreEqual(23, this.container.Resolve<int>("forward"));
        }

        [TestMethod]
        public void Then_InstanceUsingTypeConverterIsCreatedProperly()
        {
            Assert.AreEqual(-23, this.container.Resolve<int>("negated"));
        }
    }
}
