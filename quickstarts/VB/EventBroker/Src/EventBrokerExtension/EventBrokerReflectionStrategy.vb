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
Imports System.Reflection
Imports Microsoft.Practices.ObjectBuilder2
Imports SimpleEventBroker

Namespace EventBrokerExtension
    Public Class EventBrokerReflectionStrategy
        Inherits BuilderStrategy

        Public Overloads Overrides Sub PreBuildUp(ByVal context As IBuilderContext)
            If context.Policies.[Get](Of IEventBrokerInfoPolicy)(context.BuildKey) Is Nothing Then
                Dim policy As New EventBrokerInfoPolicy()
                context.Policies.[Set](Of IEventBrokerInfoPolicy)(policy, context.BuildKey)
                AddPublicationsToPolicy(context.BuildKey, policy)
                AddSubscriptionsToPolicy(context.BuildKey, policy)
            End If
        End Sub

        Private Sub AddPublicationsToPolicy(ByVal currentBuildKey As NamedTypeBuildKey, ByVal policy As EventBrokerInfoPolicy)
            For Each eventInfo As EventInfo In currentBuildKey.Type.GetEvents()
                Dim attrs As PublishesAttribute() = DirectCast(eventInfo.GetCustomAttributes(GetType(PublishesAttribute), True), PublishesAttribute())
                If attrs.Length > 0 Then
                    policy.AddPublication(attrs(0).EventName, eventInfo.Name)
                End If
            Next
        End Sub

        Private Sub AddSubscriptionsToPolicy(ByVal currentBuildKey As NamedTypeBuildKey, ByVal policy As EventBrokerInfoPolicy)
            For Each method As MethodInfo In currentBuildKey.Type.GetMethods()
                Dim attrs As SubscribesToAttribute() = DirectCast(method.GetCustomAttributes(GetType(SubscribesToAttribute), True), SubscribesToAttribute())
                If attrs.Length > 0 Then
                    policy.AddSubscription(attrs(0).EventName, method)
                End If
            Next
        End Sub
    End Class
End Namespace

