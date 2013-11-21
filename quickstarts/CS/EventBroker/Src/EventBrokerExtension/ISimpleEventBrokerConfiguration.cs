// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity;
using SimpleEventBroker;

namespace EventBrokerExtension
{
    public interface ISimpleEventBrokerConfiguration : IUnityContainerExtensionConfigurator
    {
        EventBroker Broker { get; }
    }
}
