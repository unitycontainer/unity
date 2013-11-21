' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SimpleEventBroker

Namespace EventBrokerTests
    ''' <summary>
    ''' Summary description for PublishedEventFixture
    ''' </summary>
    <TestClass()> _
    Public Class PublishedEventFixture
        <TestMethod()> _
        Public Sub ShouldAddDelegateToPublishingEventOnAddingPublisher()
            Dim pe As New PublishedEvent()
            Dim publisher As New EventSource1()

            pe.AddPublisher(publisher, "Event1")

            Assert.AreEqual(1, publisher.NumberOfEvent1Delegates)
        End Sub

        <TestMethod()> _
        Public Sub ShouldRemoveDelegateWhenRemovingPublisher()
            Dim pe As New PublishedEvent()
            Dim publisher As New EventSource1()
            pe.AddPublisher(publisher, "Event1")

            pe.RemovePublisher(publisher, "Event1")

            Assert.AreEqual(0, publisher.NumberOfEvent1Delegates)
        End Sub

        Private Class SimpleSubscriber
            Public SubscriberFired As Boolean = False

            Public Sub OnMyEvent(ByVal sender As Object, ByVal e As EventArgs)
                SubscriberFired = True
            End Sub
        End Class

        <TestMethod()> _
        Public Sub ShouldCallSubscriberWhenPublisherFiresEvent()
            Dim pe As New PublishedEvent()
            Dim publisher As New EventSource1()
            pe.AddPublisher(publisher, "Event1")
            Dim subscriberCalled As Boolean = False
            Dim subscriberObject As SimpleSubscriber = New SimpleSubscriber
            Dim subscriber As EventHandler = New EventHandler(AddressOf subscriberObject.OnMyEvent)
            pe.AddSubscriber(subscriber)

            publisher.FireEvent1()

            Assert.IsTrue(subscriberObject.SubscriberFired)
        End Sub

        Private Class CountingSubscriber
            Public NumberOfCalls As Integer = 0

            Public Sub OnEvent1(ByVal sender As Object, ByVal e As EventArgs)
                NumberOfCalls += 1
            End Sub
        End Class

        <TestMethod()> _
        Public Sub ShouldNotCallSubscriberAfterRemoval()
            Dim pe As New PublishedEvent()
            Dim publisher As New EventSource1()
            pe.AddPublisher(publisher, "Event1")
            Dim subscriber As New CountingSubscriber
            pe.AddSubscriber(New EventHandler(AddressOf subscriber.OnEvent1))

            publisher.FireEvent1()

            pe.RemoveSubscriber(New EventHandler(AddressOf subscriber.OnEvent1))
            publisher.FireEvent1()

            Assert.AreEqual(1, subscriber.NumberOfCalls)
        End Sub

        <TestMethod()> _
        Public Sub ShouldMulticastEventsToMultipleSubscribers()
            Dim pe As New PublishedEvent()
            Dim publisher As New EventSource1()
            pe.AddPublisher(publisher, "Event1")
            Dim subscriber1 As New SimpleSubscriber
            Dim subscriber2 As New SimpleSubscriber
            pe.AddSubscriber(New EventHandler(AddressOf subscriber1.OnMyEvent))
            pe.AddSubscriber(New EventHandler(AddressOf subscriber2.OnMyEvent))

            publisher.FireEvent1()

            Assert.IsTrue(subscriber1.SubscriberFired)
            Assert.IsTrue(subscriber2.SubscriberFired)
        End Sub

    End Class
End Namespace
