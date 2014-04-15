// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
            this.Container = new UnityContainer();
        }

        protected override void Act()
        {
            base.Act();
            this.section.Configure(this.Container, this.containerName);
        }
    }
}
