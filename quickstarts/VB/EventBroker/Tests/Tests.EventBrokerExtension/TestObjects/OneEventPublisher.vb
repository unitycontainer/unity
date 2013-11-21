' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports SimpleEventBroker

Namespace Tests.EventBrokerExtension.TestObjects
	Class OneEventPublisher
    <Publishes("paste")> _
  Public Event Pasting As EventHandler
	End Class
End Namespace

