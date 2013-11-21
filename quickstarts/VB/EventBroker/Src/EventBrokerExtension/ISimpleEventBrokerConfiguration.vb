'===============================================================================
' Microsoft patterns & practices
' Unity Application Block
'===============================================================================
' Copyright © Microsoft Corporation.  All rights reserved.
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
' OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
' LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
' FITNESS FOR A PARTICULAR PURPOSE.
'===============================================================================

Imports Microsoft.Practices.Unity
Imports SimpleEventBroker

Namespace EventBrokerExtension
	Public Interface ISimpleEventBrokerConfiguration
		Inherits IUnityContainerExtensionConfigurator
		ReadOnly Property Broker() As EventBroker
	End Interface
End Namespace
