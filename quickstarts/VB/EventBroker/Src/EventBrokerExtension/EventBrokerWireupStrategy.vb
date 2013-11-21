' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports Microsoft.Practices.ObjectBuilder2
Imports SimpleEventBroker

Namespace EventBrokerExtension
	Public Class EventBrokerWireupStrategy
		Inherits BuilderStrategy
		Public Overloads Overrides Sub PreBuildUp(ByVal context As IBuilderContext)
      If Not context.Existing Is Nothing Then
        Dim policy As IEventBrokerInfoPolicy = context.Policies.[Get](Of IEventBrokerInfoPolicy)(context.BuildKey)
        If Not policy Is Nothing Then
          Dim broker As EventBroker = GetBroker(context)
          For Each pub As PublicationInfo In policy.Publications
            broker.RegisterPublisher(pub.PublishedEventName, context.Existing, pub.EventName)
          Next
          For Each [sub] As SubscriptionInfo In policy.Subscriptions
            broker.RegisterSubscriber([sub].PublishedEventName, DirectCast([Delegate].CreateDelegate(GetType(EventHandler), context.Existing, [sub].Subscriber), EventHandler))
          Next
        End If
      End If
		End Sub

		Private Function GetBroker(ByVal context As IBuilderContext) As EventBroker
            Dim broker As EventBroker = context.NewBuildUp(Of EventBroker)()
      If broker Is Nothing Then
        Throw New InvalidOperationException("No event broker available")
      End If
			Return broker
		End Function
	End Class
End Namespace

