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

namespace Microsoft.Practices.Unity.Tests.TestDoubles
{
    public class ContainerExtensionWithNonDefaultConstructor : UnityContainerExtension
    {
        public ContainerExtensionWithNonDefaultConstructor(IUnityContainer container)
        {
                
        }

        protected override void Initialize()
        {
        }
    }
}
