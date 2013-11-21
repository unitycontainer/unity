// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.Unity.TestSupport
{
    public class MockContainerExtension : UnityContainerExtension, IMockConfiguration
    {
        private bool initializeWasCalled = false;

        public bool InitializeWasCalled
        {
            get { return initializeWasCalled; }
        }

        public new ExtensionContext Context
        {
            get { return base.Context; }
        }

        protected override void Initialize()
        {
            initializeWasCalled = true;
        }
    }

    public interface IMockConfiguration : IUnityContainerExtensionConfigurator
    {
        
    }
}
