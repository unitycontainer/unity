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
	''' <summary>
	''' Base class for the two publish / subscribe attributes. Stores
	''' the event name to be published or subscribed to.
	''' </summary>
	Public MustInherit Class PublishSubscribeAttribute
		Inherits Attribute
    Private _eventName As String

		Protected Sub New(ByVal eventName As String)
      _eventName = eventName
		End Sub

		Public Property EventName() As String
			Get
        Return _eventName
			End Get
			Set
        _eventName = Value
			End Set
		End Property
	End Class
End Namespace
