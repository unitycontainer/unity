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

Imports Microsoft.Practices.ObjectBuilder2

Namespace Tests.EventBrokerExtension.Utility
    Public Class MockBuilderContext
        Implements IBuilderContext
        Private _lifetime As ILifetimeContainer = New LifetimeContainer()
        Private _locator As IReadWriteLocator
        Private _originalBuildKey As Object = Nothing
        Private _persistentPolicies As IPolicyList = New PolicyList()
        Private _transientPolicies As IPolicyList
        Private _strategies As New StrategyChain()

        Private _buildKey As Object = Nothing
        Private _existing As Object = Nothing
        Private _buildComplete As Boolean

        Public Sub New()
            Me.New(New Locator())
        End Sub

        Public Sub New(ByVal locator As IReadWriteLocator)
            Me._locator = locator
        End Sub

        Public ReadOnly Property Lifetime() As ILifetimeContainer Implements IBuilderContext.Lifetime
            Get
                Return _lifetime
            End Get
        End Property

        Public ReadOnly Property Locator() As IReadWriteLocator Implements IBuilderContext.Locator
            Get
                Return _locator
            End Get
        End Property

        Public ReadOnly Property OriginalBuildKey() As Object Implements IBuilderContext.OriginalBuildKey
            Get
                Return _originalBuildKey
            End Get
        End Property

        Public ReadOnly Property PersistentPolicies() As IPolicyList Implements IBuilderContext.PersistentPolicies
            Get
                Return _persistentPolicies
            End Get
        End Property

        Public ReadOnly Property Policies() As IPolicyList Implements IBuilderContext.Policies
            Get
                Return _transientPolicies
            End Get
        End Property

        Public ReadOnly Property Strategies() As IStrategyChain Implements IBuilderContext.Strategies
            Get
                Return _strategies
            End Get
        End Property

        Public Property BuildKey() As Object Implements IBuilderContext.BuildKey
            Get
                Return _buildKey
            End Get
            Set(ByVal value As Object)
                _buildKey = value
            End Set
        End Property

        Public Property Existing() As Object Implements IBuilderContext.Existing
            Get
                Return _existing
            End Get
            Set(ByVal value As Object)
                _existing = value
            End Set
        End Property

        Public Property BuildComplete() As Boolean Implements IBuilderContext.BuildComplete
            Get
                Return _buildComplete
            End Get
            Set(ByVal value As Boolean)
                _buildComplete = value
            End Set
        End Property

        Public Function CloneForNewBuild(ByVal newBuildKey As Object, ByVal newExistingObject As Object) As IBuilderContext Implements IBuilderContext.CloneForNewBuild
            Dim newContext As New MockBuilderContext(Me._locator)
            newContext._strategies = _strategies
            newContext._persistentPolicies = _persistentPolicies
            newContext._transientPolicies = _transientPolicies
            newContext._lifetime = _lifetime
            newContext._originalBuildKey = newBuildKey
            newContext._buildKey = newBuildKey
            newContext._existing = newExistingObject
            Return newContext
        End Function

        Public Function ExecuteBuildUp(ByVal buildKey As Object, ByVal existing As Object)
            Me._buildKey = buildKey
            Me._existing = existing

            Return _strategies.ExecuteBuildUp(Me)
        End Function

        Public Function ExecuteTearDown(ByVal existing As Object) As Object
            Me._buildKey = Nothing
            Me._existing = existing

            _strategies.Reverse().ExecuteTearDown(Me)
            Return existing
        End Function
    End Class
End Namespace
