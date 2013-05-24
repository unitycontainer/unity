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

Namespace SimpleEventBroker
	<AttributeUsage(AttributeTargets.Method, AllowMultiple := True, Inherited := False)> _
	Public Class SubscribesToAttribute
		Inherits PublishSubscribeAttribute
		Public Sub New(ByVal eventName As String)
			MyBase.New(eventName)
		End Sub
	End Class
End Namespace
