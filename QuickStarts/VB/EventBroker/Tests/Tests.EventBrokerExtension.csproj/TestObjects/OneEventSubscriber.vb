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

