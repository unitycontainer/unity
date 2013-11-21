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

namespace Microsoft.Practices.Unity.TestSupport.Configuration
{
    public abstract class ContainerConfiguringFixture<TResourceLocator> : SectionLoadingFixture<TResourceLocator>
    {
        private readonly string containerName;

        protected ContainerConfiguringFixture(string configFileName, string containerName)
            : base(configFileName)
        {
            this.containerName = containerName;
        }

        protected ContainerConfiguringFixture(string configFileName, string sectionName, string containerName)
            : base(configFileName, sectionName)
        {
            this.containerName = containerName;
        }

        protected IUnityContainer Container { get; private set; }

        protected override void Arrange()
        {
            base.Arrange();
            Container = new UnityContainer();
        }

        protected override void Act()
        {
            base.Act();
            Section.Configure(Container, containerName);
        }
    }
}
