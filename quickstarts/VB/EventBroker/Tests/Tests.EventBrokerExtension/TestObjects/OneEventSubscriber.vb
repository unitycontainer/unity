' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports SimpleEventBroker

Namespace Tests.EventBrokerExtension.TestObjects
	Class OneEventSubscriber
		<SubscribesTo("copy")> _
		Public Sub OnCopy(ByVal sender As Object, ByVal e As EventArgs)
			' Nothing needed
		End Sub
	End Class
End Namespace

