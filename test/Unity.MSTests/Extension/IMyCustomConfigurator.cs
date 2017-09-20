// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Unity.Tests.Extension
{
    public interface IMyCustomConfigurator : IUnityContainerExtensionConfigurator
    {
        IMyCustomConfigurator AddPolicy();
    }
}