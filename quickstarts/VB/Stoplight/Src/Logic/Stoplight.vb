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
Imports Microsoft.Practices.Unity
Imports StopLight.ServiceImplementations
Imports StopLight.ServiceInterfaces

Namespace StopLight.Logic
    Public Enum StoplightColors
        Green
        Yellow
        Red
    End Enum

    Public Class LightChangedEventArgs
        Inherits EventArgs
        Private currColor As StoplightColors

        Public Property CurrentColor() As StoplightColors
            Get
                Return currColor
            End Get
            Private Set(ByVal value As StoplightColors)
                currColor = Value
            End Set
        End Property

        Public Sub New(ByVal color As StoplightColors)
            currColor = color
        End Sub
    End Class

    Public Delegate Sub StoplightChangedHandler(ByVal sender As Object, ByVal e As LightChangedEventArgs)

    Public Class Stoplight
        Public Const Green As StoplightColors = StoplightColors.Green
        Public Const Yellow As StoplightColors = StoplightColors.Yellow
        Public Const Red As StoplightColors = StoplightColors.Red

        Private currColor As StoplightColors = StoplightColors.Green
        Private _logger As ILogger = New NullLogger()
        Public Event Changed As StoplightChangedHandler

        Public ReadOnly Property CurrentColor() As StoplightColors
            Get
                Return currColor
            End Get
        End Property

        <Dependency()> _
        Public Property Logger() As ILogger
            Get
                Return _logger
            End Get
            Set(ByVal value As ILogger)
                _logger = value
            End Set
        End Property

        Public Sub [Next]()
            currColor += 1
            If currColor > StoplightColors.Red Then
                currColor = StoplightColors.Green
            End If
            RaiseChanged()
            _logger.Write(String.Format("LIGHT CHANGED TO {0}", CurrentColor))
        End Sub

        Protected Sub RaiseChanged()
            Dim handlers As StoplightChangedHandler = ChangedEvent
            If Not handlers Is Nothing Then
                Dim e As New LightChangedEventArgs(currColor)
                handlers(Me, e)
            End If
        End Sub

    End Class
End Namespace

