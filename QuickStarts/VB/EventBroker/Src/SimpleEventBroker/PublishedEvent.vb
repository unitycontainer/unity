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

Namespace SimpleEventBroker
	Public Class PublishedEvent
    Private _publishers As List(Of Object)
    Private _subscribers As List(Of EventHandler)

    Public Sub New()
      _publishers = New List(Of Object)()
      _subscribers = New List(Of EventHandler)()
    End Sub

    Public ReadOnly Property Publishers() As IEnumerable(Of Object)
      Get
        Dim theList As New List(Of Object)
        For Each publisher As Object In _publishers
          theList.Add(publisher)
        Next
        Return theList
      End Get
    End Property

    Public ReadOnly Property Subscribers() As IEnumerable(Of EventHandler)
      Get
        Dim theList As New List(Of EventHandler)
        For Each subscriber As EventHandler In _subscribers
          theList.Add(subscriber)
        Next
        Return theList
      End Get
    End Property

    Public ReadOnly Property HasPublishers() As Boolean
      Get
        Return _publishers.Count > 0
      End Get
    End Property

    Public ReadOnly Property HasSubscribers() As Boolean
      Get
        Return _subscribers.Count > 0
      End Get
    End Property

    Public Sub AddPublisher(ByVal publisher As Object, ByVal eventName As String)
      _publishers.Add(publisher)
      Dim targetEvent As EventInfo = publisher.[GetType]().GetEvent(eventName)
      GuardEventExists(eventName, publisher, targetEvent)

      Dim addEventMethod As MethodInfo = targetEvent.GetAddMethod()
      GuardAddMethodExists(targetEvent)

      Dim newSubscriber As EventHandler = AddressOf OnPublisherFiring
      addEventMethod.Invoke(publisher, New Object() {newSubscriber})
    End Sub

    Public Sub RemovePublisher(ByVal publisher As Object, ByVal eventName As String)
      _publishers.Remove(publisher)
      Dim targetEvent As EventInfo = publisher.[GetType]().GetEvent(eventName)
      GuardEventExists(eventName, publisher, targetEvent)

      Dim removeEventMethod As MethodInfo = targetEvent.GetRemoveMethod()
      GuardRemoveMethodExists(targetEvent)

      Dim subscriber As EventHandler = AddressOf OnPublisherFiring
      removeEventMethod.Invoke(publisher, New Object() {subscriber})
    End Sub

		Public Sub AddSubscriber(ByVal subscriber As EventHandler)
      _subscribers.Add(subscriber)
		End Sub

		Public Sub RemoveSubscriber(ByVal subscriber As EventHandler)
      _subscribers.Remove(subscriber)
		End Sub

		Private Sub OnPublisherFiring(ByVal sender As Object, ByVal e As EventArgs)
      For Each subscriber As EventHandler In _subscribers
        subscriber.Invoke(sender, e)
      Next
		End Sub

		Private Shared Sub GuardEventExists(ByVal eventName As String, ByVal publisher As Object, ByVal targetEvent As EventInfo)
      If targetEvent Is Nothing Then
        Throw New ArgumentException(String.Format("The event '{0}' is not implemented on type '{1}'", eventName, publisher.[GetType]().Name))
      End If
		End Sub

		Private Shared Sub GuardAddMethodExists(ByVal targetEvent As EventInfo)
      If targetEvent.GetAddMethod() Is Nothing Then
        Throw New ArgumentException(String.Format("The event '{0}' does not have a public Add method", targetEvent.Name))
      End If
		End Sub

		Private Shared Sub GuardRemoveMethodExists(ByVal targetEvent As EventInfo)
      If targetEvent.GetRemoveMethod() Is Nothing Then
        Throw New ArgumentException(String.Format("The event '{0}' does not have a public Remove method", targetEvent.Name))
      End If
		End Sub
	End Class
End Namespace
