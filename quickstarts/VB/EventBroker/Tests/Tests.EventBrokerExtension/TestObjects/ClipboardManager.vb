' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports SimpleEventBroker

Namespace Tests.EventBrokerExtension.TestObjects
	Class ClipboardManager
		' turn off "event is never used" warning
    <Publishes("cut")> _
  Public Event Cut As EventHandler

		<Publishes("copy")> _
		Public Event Copy As EventHandler

		<Publishes("paste")> _
		Public Event Paste As EventHandler


		<SubscribesTo("copy")> _
		Public Sub OnCopy(ByVal sender As Object, ByVal e As EventArgs)
		End Sub

		<SubscribesTo("clipboard data available")> _
		Public Sub OnClipboardDataAvailable(ByVal sender As Object, ByVal e As EventArgs)

		End Sub
	End Class
End Namespace
