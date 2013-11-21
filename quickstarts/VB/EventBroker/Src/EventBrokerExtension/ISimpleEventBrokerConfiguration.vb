' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports Microsoft.Practices.Unity
Imports SimpleEventBroker

Namespace EventBrokerExtension
	Public Interface ISimpleEventBrokerConfiguration
		Inherits IUnityContainerExtensionConfigurator
		ReadOnly Property Broker() As EventBroker
	End Interface
End Namespace
