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
Imports System.Text
Imports System.Collections.Generic
Imports EventBrokerExtension
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Practices.ObjectBuilder2
Imports SimpleEventBroker
Imports Tests.EventBrokerExtension.Tests.EventBrokerExtension.TestObjects
Imports Tests.EventBrokerExtension.Tests.EventBrokerExtension.Utility
Imports Microsoft.Practices.Unity

<TestClass()> Public Class EventBrokerWireupStrategyFixture

    <TestMethod(), ExpectedException(GetType(InvalidOperationException))> _
    Public Sub StrategyThrowsIfWireupIsNeededAndBrokerNotInLocator()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As New NamedTypeBuildKey(GetType(OneEventPublisher))
        Dim policy As New EventBrokerInfoPolicy()
        policy.AddPublication("paste", "Pasting")
        context.Policies.Set(Of IEventBrokerInfoPolicy)(policy, buildKey)

        Dim existing As New OneEventPublisher()
        context.ExecuteBuildUp(buildKey, existing)
    End Sub

    <TestMethod()> _
    Public Sub NoExceptionIfExistingObjectDoesntAndNoBroker()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As New NamedTypeBuildKey(GetType(OneEventPublisher))
        Dim policy As New EventBrokerInfoPolicy()
        policy.AddPublication("paste", "Pasting")
        context.Policies.Set(Of IEventBrokerInfoPolicy)(policy, buildKey)

        context.ExecuteBuildUp(buildKey, Nothing)
        ' No assert needed, if we got here, we're ok
    End Sub

    <TestMethod(), ExpectedException(GetType(InvalidOperationException))> _
    Public Sub ExceptionIfNoWireupNeededAndNoBroker()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As New NamedTypeBuildKey(GetType(OneEventPublisher))
        Dim policy As New EventBrokerInfoPolicy()
        context.Policies.Set(Of IEventBrokerInfoPolicy)(policy, buildKey)

        context.ExecuteBuildUp(buildKey, New Object())

    End Sub

    <TestMethod()> _
    Public Sub StrategyProperlyWiresEvents()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As New NamedTypeBuildKey(GetType(ClipboardManager))

        Dim broker As New EventBroker()
        Dim brokerLifetime As New ExternallyControlledLifetimeManager
        brokerLifetime.SetValue(broker)
        context.Policies.Set(Of ILifetimePolicy)(brokerLifetime, NamedTypeBuildKey.Make(Of EventBroker)())

        Dim policy As New EventBrokerInfoPolicy
        policy.AddPublication("cut", "Cut")
        policy.AddPublication("copy", "Copy")
        policy.AddPublication("paste", "Paste")

        policy.AddSubscription("copy", GetType(ClipboardManager).GetMethod("OnCopy"))
        policy.AddSubscription("clipboard data available", _
                               GetType(ClipboardManager).GetMethod("OnClipboardDataAvailable"))

        context.Policies.Set(Of IEventBrokerInfoPolicy)(policy, buildKey)

        Dim existing As New ClipboardManager()

        context.ExecuteBuildUp(buildKey, existing)

        Dim registeredEvents As New List(Of String)(broker.RegisteredEvents)
        registeredEvents.Sort()

        Dim expectedEvents As New List(Of String)(New String() { _
            "cut", _
            "copy", _
            "paste", _
            "clipboard data available" _
        })

        expectedEvents.Sort()

        CollectionAssert.AreEqual(expectedEvents, registeredEvents)
    End Sub

    Private Function CreateContext() As MockBuilderContext
        Dim context As New MockBuilderContext()
        context.Strategies.Add(New LifetimeStrategy())
        context.Strategies.Add(New EventBrokerWireupStrategy())
        Return context
    End Function
End Class
