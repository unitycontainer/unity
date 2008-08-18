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
Imports SimpleEventBroker

Namespace SimpleEventBroker
  Public Class EventBroker

    Private eventPublishers As New Dictionary(Of String, PublishedEvent)()

    Public ReadOnly Property RegisteredEvents() As IEnumerable(Of String)
      Get
        Dim theList As New List(Of String)
        For Each eventName As String In eventPublishers.Keys
          theList.Add(eventName)
        Next
        Return theList
      End Get
    End Property

    Public Sub RegisterPublisher(ByVal publishedEventName As String, ByVal publisher As Object, ByVal eventName As String)
      Dim _event As PublishedEvent = GetEvent(publishedEventName)
      _event.AddPublisher(publisher, eventName)
    End Sub

    Public Sub UnregisterPublisher(ByVal publishedEventName As String, ByVal publisher As Object, ByVal eventName As String)
      Dim _event As PublishedEvent = GetEvent(publishedEventName)
      _event.RemovePublisher(publisher, eventName)
      RemoveDeadEvents()
    End Sub

    Public Sub RegisterSubscriber(ByVal publishedEventName As String, ByVal subscriber As EventHandler)
      Dim publishedEvent As PublishedEvent = GetEvent(publishedEventName)
      publishedEvent.AddSubscriber(subscriber)
    End Sub

    Public Sub UnregisterSubscriber(ByVal publishedEventName As String, ByVal subscriber As EventHandler)
      Dim _event As PublishedEvent = GetEvent(publishedEventName)
      _event.RemoveSubscriber(subscriber)
      RemoveDeadEvents()
    End Sub

    Public Function GetPublishersFor(ByVal publishedEvent As String) As IEnumerable(Of Object)
      Dim theList As New List(Of Object)
      For Each publisher As Object In GetEvent(publishedEvent).Publishers
        theList.Add(publisher)
      Next
      Return theList
    End Function

    Public Function GetSubscribersFor(ByVal publishedEvent As String) As IEnumerable(Of EventHandler)
      Dim theList As New List(Of EventHandler)
      For Each subscriber As EventHandler In GetEvent(publishedEvent).Subscribers
        theList.Add(subscriber)
      Next
      Return theList
    End Function

    Private Function GetEvent(ByVal eventName As String) As PublishedEvent
      If Not eventPublishers.ContainsKey(eventName) Then
        eventPublishers(eventName) = New PublishedEvent()
      End If
      Return eventPublishers(eventName)
    End Function

    Private Sub RemoveDeadEvents()
      Dim deadEvents As New List(Of String)()
      For Each publishedEvent As KeyValuePair(Of String, PublishedEvent) In eventPublishers
        If Not publishedEvent.Value.HasPublishers AndAlso Not publishedEvent.Value.HasSubscribers Then
          deadEvents.Add(publishedEvent.Key)
        End If
      Next

      For Each pEvent As String In deadEvents
        eventPublishers.Remove(pEvent)
      Next

    End Sub

  End Class
End Namespace

