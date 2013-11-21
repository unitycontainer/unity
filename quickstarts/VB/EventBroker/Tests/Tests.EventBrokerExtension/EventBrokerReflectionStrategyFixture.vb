' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Collections.Generic
Imports EventBrokerExtension
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Practices.ObjectBuilder2
Imports Tests.EventBrokerExtension.Tests.EventBrokerExtension.TestObjects
Imports Tests.EventBrokerExtension.Tests.EventBrokerExtension.Utility

''' <summary>
''' Tests of the strategies that look at objects and
''' reflects over them looking for event subscriptions.
''' </summary>
''' <remarks></remarks>
<TestClass()> Public Class EventBrokerReflectionStrategyFixture

    <TestMethod()> _
    Public Sub ReflectingOverObjectWithoutSubscriptionResultsInEmptyPolicy()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As NamedTypeBuildKey = NamedTypeBuildKey.Make(Of Object)()

        context.ExecuteBuildUp(buildKey, Nothing)

        Dim policy As IEventBrokerInfoPolicy = context.Policies.Get(Of IEventBrokerInfoPolicy)(buildKey)
        Assert.IsNotNull(policy)

        Dim publications As New List(Of PublicationInfo)(policy.Publications)
        Dim subscriptions As New List(Of SubscriptionInfo)(policy.Subscriptions)

        Assert.AreEqual(0, publications.Count)
        Assert.AreEqual(0, subscriptions.Count)
    End Sub

    <TestMethod()> _
    Public Sub StrategyDOesntOverwriteAnExistingPolicy()

        Dim context As MockBuilderContext = CreateContext()

        Dim buildKey As New NamedTypeBuildKey(GetType(Object))
        Dim policy As New EventBrokerInfoPolicy
        context.Policies.Set(Of IEventBrokerInfoPolicy)(policy, buildKey)

        context.ExecuteBuildUp(buildKey, Nothing)

        Dim setPolicy As IEventBrokerInfoPolicy = context.Policies.Get(Of IEventBrokerInfoPolicy)(buildKey)

        Assert.AreSame(policy, setPolicy)
    End Sub

    <TestMethod()> _
    Public Sub ReflectingOerPublishingTypeResultsInCorrectPolicy()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As New NamedTypeBuildKey(GetType(OneEventPublisher))

        context.ExecuteBuildUp(buildKey, Nothing)

        Dim policy As IEventBrokerInfoPolicy = context.Policies.Get(Of IEventBrokerInfoPolicy)(buildKey)

        Assert.IsNotNull(policy)

        Dim publications As New List(Of PublicationInfo)(policy.Publications)
        Dim subscriptions As New List(Of SubscriptionInfo)(policy.Subscriptions)

        Assert.AreEqual(0, subscriptions.Count)

        CollectionAssert.AreEqual(New PublicationInfo() {New PublicationInfo("paste", "Pasting")}, publications)

    End Sub

    <TestMethod()> _
    Public Sub ReflectingOverSubscribingTypeResultsInCorrectPolicy()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As New NamedTypeBuildKey(GetType(OneEventSubscriber))

        context.ExecuteBuildUp(buildKey, Nothing)

        Dim policy As IEventBrokerInfoPolicy = context.Policies.Get(Of IEventBrokerInfoPolicy)(buildKey)
        Assert.IsNotNull(policy)

        Dim publications As New List(Of PublicationInfo)(policy.Publications)
        Dim subscriptions As New List(Of SubscriptionInfo)(policy.Subscriptions)

        Assert.AreEqual(0, publications.Count)

        CollectionAssert.AreEqual(New SubscriptionInfo() {New SubscriptionInfo("copy", GetType(OneEventSubscriber).GetMethod("OnCopy"))}, subscriptions)

    End Sub

    <TestMethod()> _
    Public Sub OneTypeCanPublishAndSubscribeMultipleTimes()
        Dim context As MockBuilderContext = CreateContext()
        Dim buildKey As New NamedTypeBuildKey(GetType(ClipboardManager))

        context.ExecuteBuildUp(buildKey, Nothing)

        Dim policy As IEventBrokerInfoPolicy = context.Policies.Get(Of IEventBrokerInfoPolicy)(buildKey)
        Assert.IsNotNull(policy)

        Dim publications As New List(Of PublicationInfo)(policy.Publications)
        Dim subscriptions As New List(Of SubscriptionInfo)(policy.Subscriptions)

        publications.Sort(Function(a, b) a.PublishedEventName.CompareTo(b.PublishedEventName))
        subscriptions.Sort(Function(a, b) a.PublishedEventName.CompareTo(b.PublishedEventName))

        CollectionAssert.AreEqual( _
            New PublicationInfo() { _
                New PublicationInfo("copy", "Copy"), _
                New PublicationInfo("cut", "Cut"), _
                New PublicationInfo("paste", "Paste") _
            }, publications)

        CollectionAssert.AreEqual( _
            New SubscriptionInfo() { _
                New SubscriptionInfo("clipboard data available", GetType(ClipboardManager).GetMethod("OnClipboardDataAvailable")), _
                New SubscriptionInfo("copy", GetType(ClipboardManager).GetMethod("OnCopy")) _
            }, subscriptions)

    End Sub

    Private Function CreateContext() As MockBuilderContext
        Dim context As New MockBuilderContext()
        context.Strategies.Add(New EventBrokerReflectionStrategy())

        Return context
    End Function

End Class
