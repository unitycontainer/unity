// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Configuration;

namespace Unity.TestSupport.Configuration
{
    public abstract class SectionLoadingFixture<TResourceLocator>
    {
        protected UnityConfigurationSection section;
        private readonly string configFileName;
        protected string SectionName { get; private set; }

        protected SectionLoadingFixture(string configFileName)
            : this(configFileName, "unity")
        {
        }

        protected SectionLoadingFixture(string configFileName, string sectionName)
        {
            this.configFileName = configFileName;
            this.SectionName = sectionName;
        }

        protected virtual void Arrange()
        {
            var loader = new ConfigFileLoader<TResourceLocator>(configFileName);
            section = loader.GetSection<UnityConfigurationSection>(SectionName);
        }

        protected virtual void Act()
        {
        }

        protected virtual void Teardown()
        {
        }

        [TestInitialize]
        public void MainSetup()
        {
            Arrange();
            Act();
        }

        [TestCleanup]
        public void MainTeardown()
        {
            Teardown();
        }
    }
}
