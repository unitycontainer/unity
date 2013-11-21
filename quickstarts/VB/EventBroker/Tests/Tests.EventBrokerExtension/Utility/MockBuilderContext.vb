' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports Microsoft.Practices.ObjectBuilder2
Imports Microsoft.Practices.Unity

Namespace Tests.EventBrokerExtension.Utility
    Public Class MockBuilderContext
        Implements IBuilderContext
        Private _lifetime As ILifetimeContainer = New LifetimeContainer()
        Private _originalBuildKey As NamedTypeBuildKey = Nothing
        Private _persistentPolicies As IPolicyList = New PolicyList()
        Private _transientPolicies As IPolicyList
        Private _strategies As New StrategyChain()
        Private _recoveryStack As New RecoveryStack()

        Private _buildKey As NamedTypeBuildKey = Nothing
        Private _existing As Object = Nothing
        Private _buildComplete As Boolean

        Public Sub New()
            _transientPolicies = New PolicyList(_persistentPolicies)
        End Sub

        Public ReadOnly Property Lifetime() As ILifetimeContainer Implements IBuilderContext.Lifetime
            Get
                Return _lifetime
            End Get
        End Property

        Public ReadOnly Property OriginalBuildKey() As NamedTypeBuildKey Implements IBuilderContext.OriginalBuildKey
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

        Public Sub AddResolverOverrides(ByVal newOverrides As IEnumerable(Of ResolverOverride)) Implements IBuilderContext.AddResolverOverrides
            Throw New NotImplementedException()
        End Sub

        Public Function GetOverriddenResolver(ByVal dependencyType As Type) As IDependencyResolverPolicy Implements IBuilderContext.GetOverriddenResolver
            Throw New NotImplementedException()
        End Function

        Public Function NewBuildUp(ByVal newBuildKey As NamedTypeBuildKey) As Object Implements IBuilderContext.NewBuildUp
            Dim newContext As New MockBuilderContext

            With newContext
                ._strategies = _strategies
                ._persistentPolicies = _persistentPolicies
                ._transientPolicies = _transientPolicies
                ._lifetime = _lifetime
                ._originalBuildKey = newBuildKey
                ._buildKey = newBuildKey
            End With

            _childContext = newContext

            Dim result As Object = newContext.ExecuteBuildUp(newBuildKey, Nothing)
            _childContext = Nothing
            Return result

        End Function
        Public Function NewBuildUp(ByVal newBuildKey As NamedTypeBuildKey, ByVal childCustomizationBlock As Action(Of IBuilderContext)) Implements IBuilderContext.NewBuildUp
            Dim newContext As New MockBuilderContext

            With newContext
                ._strategies = _strategies
                ._persistentPolicies = _persistentPolicies
                ._transientPolicies = _transientPolicies
                ._lifetime = _lifetime
                ._originalBuildKey = newBuildKey
                ._buildKey = newBuildKey
            End With

            childCustomizationBlock(newContext)

            _childContext = newContext

            Dim result As Object = newContext.ExecuteBuildUp(newBuildKey, Nothing)
            _childContext = Nothing
            Return result


        End Function


        Public Shadows ReadOnly Property Strategies() As StrategyChain
            Get
                Return _strategies
            End Get
        End Property

        Public Shadows ReadOnly Property ContextStrategies() As IStrategyChain Implements IBuilderContext.Strategies
            Get
                Return _strategies
            End Get
        End Property

        Public Property BuildKey() As NamedTypeBuildKey Implements IBuilderContext.BuildKey
            Get
                Return _buildKey
            End Get
            Set(ByVal value As NamedTypeBuildKey)
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

        Private _currentOperation As Object

        Public Property CurrentOperation() As Object Implements IBuilderContext.CurrentOperation
            Get
                Return _currentOperation
            End Get
            Set(ByVal value As Object)
                _currentOperation = value
            End Set
        End Property

        Private _childContext As IBuilderContext

        Public ReadOnly Property ChildContext() As IBuilderContext Implements IBuilderContext.ChildContext
            Get
                Return _childContext
            End Get
        End Property

        Public ReadOnly Property RecoveryStack() As IRecoveryStack Implements IBuilderContext.RecoveryStack
            Get
                Return _recoveryStack
            End Get
        End Property

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
