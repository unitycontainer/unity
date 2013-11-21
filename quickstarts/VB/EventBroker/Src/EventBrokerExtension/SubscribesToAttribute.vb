' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System

Namespace SimpleEventBroker
	<AttributeUsage(AttributeTargets.Method, AllowMultiple := True, Inherited := False)> _
	Public Class SubscribesToAttribute
		Inherits PublishSubscribeAttribute
		Public Sub New(ByVal eventName As String)
			MyBase.New(eventName)
		End Sub
	End Class
End Namespace
