' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SimpleEventBroker

Namespace EventBrokerTests
    ''' <summary>
    ''' Summary description for EventBrokerFixture
    ''' </summary>
    <TestClass()> _
    Public Class EventBrokerFixture
        <TestMethod()> _
        Public Sub ShouldRegisterOnePublisher()
            Dim broker As New EventBroker()
            Dim publisher As New EventSource1()

            broker.RegisterPublisher("PublishedEvent1", publisher, "Event1")

            Dim published As New List(Of String)(broker.RegisteredEvents)
            Dim publishers As New List(Of Object)(broker.GetPublishersFor("PublishedEvent1"))

            Assert.IsTrue(published.Contains("PublishedEvent1"))
            Assert.AreEqual(1, publishers.Count)
            Assert.AreSame(publisher, publishers(0))
        End Sub

        <TestMethod()> _
        Public Sub ShouldRegisterOneSubscriber()
            Dim broker As New EventBroker()

            Dim subscriber As EventHandler = Nothing

            broker.RegisterSubscriber("SubscribedEvent1", subscriber)

            Dim published As New List(Of String)(broker.RegisteredEvents)
            Dim publishers As New List(Of Object)(broker.GetPublishersFor("SubscribedEvent1"))
            Dim subscribers As New List(Of EventHandler)(broker.GetSubscribersFor("SubscribedEvent1"))

            Assert.AreEqual(1, published.Count)
            Assert.AreEqual("SubscribedEvent1", published(0))
            Assert.AreEqual(0, publishers.Count)
            Assert.AreEqual(1, subscribers.Count)
            Assert.AreSame(subscriber, subscribers(0))
        End Sub

        <TestMethod()> _
        Public Sub ShouldRegisterOnePublisherAndOneSubscriber()
            Dim broker As New EventBroker()
            Dim publisher As New EventSource1()
            Dim publishedEventName As String = "MyEvent"
            Dim subscriber As EventHandler = Nothing

            broker.RegisterPublisher(publishedEventName, publisher, "Event1")
            broker.RegisterSubscriber(publishedEventName, subscriber)

            Dim published As New List(Of String)(broker.RegisteredEvents)
            Dim publishers As New List(Of Object)(broker.GetPublishersFor(publishedEventName))
            Dim subscribers As New List(Of EventHandler)(broker.GetSubscribersFor(publishedEventName))

            Assert.AreEqual(1, published.Count)
            Assert.AreEqual(publishedEventName, published(0))

            Assert.AreEqual(1, publishers.Count)
            Assert.AreSame(publisher, publishers(0))

            Assert.AreEqual(1, subscribers.Count)
            Assert.AreSame(subscriber, subscribers(0))
        End Sub

        Private Class SimpleSubscriber
            Public SubscriberFired As Boolean = False

            Public Sub OnMyEvent(ByVal sender As Object, ByVal e As EventArgs)
                SubscriberFired = True
            End Sub
        End Class

        <TestMethod()> _
        Public Sub ShouldCallSubscriberWhenPublisherFiresEvent()
            Dim broker As New EventBroker()
            Dim publisher As New EventSource1()
            Dim publishedEventName As String = "MyEvent"
            Dim subscriberFired As Boolean = False
            Dim subscriberObject As New SimpleSubscriber
            Dim subscriber As EventHandler = New EventHandler(AddressOf subscriberObject.OnMyEvent)

            broker.RegisterPublisher(publishedEventName, publisher, "Event1")
            broker.RegisterSubscriber(publishedEventName, subscriber)

            publisher.FireEvent1()

            Assert.IsTrue(subscriberObject.SubscriberFired)
        End Sub

        <TestMethod()> _
        Public Sub ShouldRemovePublisherFromListOnUnregistration()
            Dim broker As New EventBroker()
            Dim publisher As New EventSource1()
            Dim publishedEventName As String = "MyEvent"
            broker.RegisterPublisher(publishedEventName, publisher, "Event1")

            broker.UnregisterPublisher(publishedEventName, publisher, "Event1")

            Assert.AreEqual(0, New List(Of Object)(broker.GetPublishersFor(publishedEventName)).Count)
        End Sub

        <TestMethod()> _
        Public Sub ShouldRemoveSubscriberFromListOnUnregistration()
            Dim broker As New EventBroker()
            Dim publishedEventName As String = "SomeEvent"
            Dim subscriber As EventHandler = Nothing
            broker.RegisterSubscriber(publishedEventName, subscriber)

            broker.UnregisterSubscriber(publishedEventName, subscriber)

            Assert.AreEqual(0, New List(Of EventHandler)(broker.GetSubscribersFor(publishedEventName)).Count)
        End Sub

    End Class
End Namespace

