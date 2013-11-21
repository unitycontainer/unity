' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
