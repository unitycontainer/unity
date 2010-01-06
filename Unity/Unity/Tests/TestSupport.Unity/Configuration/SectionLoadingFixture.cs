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

using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.TestSupport.Configuration
{
    public abstract class SectionLoadingFixture<TResourceLocator>
    {
        protected UnityConfigurationSection Section;
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
            Section = loader.GetSection<UnityConfigurationSection>(SectionName);
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
