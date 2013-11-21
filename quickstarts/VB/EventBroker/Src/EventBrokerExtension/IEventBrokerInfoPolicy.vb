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
Imports System.Collections.Generic
Imports System.Reflection
Imports Microsoft.Practices.ObjectBuilder2

Namespace EventBrokerExtension
	''' <summary>
	''' This policy interface allows access to saved publication and
	''' subscription information.
	''' </summary>
	Public Interface IEventBrokerInfoPolicy
		Inherits IBuilderPolicy
		ReadOnly Property Publications() As IEnumerable(Of PublicationInfo)
		ReadOnly Property Subscriptions() As IEnumerable(Of SubscriptionInfo)
	End Interface

	Public Structure PublicationInfo
		Public PublishedEventName As String
		Public EventName As String

    Public Sub New(ByVal pEventName As String, ByVal eEventName As String)
      PublishedEventName = pEventName
      EventName = eEventName
    End Sub
	End Structure

	Public Structure SubscriptionInfo
		Public PublishedEventName As String
		Public Subscriber As MethodInfo

    Public Sub New(ByVal pEventName As String, ByVal subs As MethodInfo)
      PublishedEventName = pEventName
      Subscriber = subs
    End Sub
	End Structure
End Namespace
