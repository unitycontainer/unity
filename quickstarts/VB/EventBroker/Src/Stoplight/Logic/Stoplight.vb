' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
    Private _currentColor As StoplightColors

		Public Property CurrentColor() As StoplightColors
			Get
        Return _currentColor
			End Get
			Private Set
        _currentColor = Value
			End Set
		End Property

		Public Sub New(ByVal color As StoplightColors)
			CurrentColor = color
		End Sub
	End Class


  Public Delegate Sub StoplightChangedHandler(ByVal sender As Object, ByVal e As LightChangedEventArgs)


	Public Class Stoplight
		Public Const Green As StoplightColors = StoplightColors.Green
		Public Const Yellow As StoplightColors = StoplightColors.Yellow
		Public Const Red As StoplightColors = StoplightColors.Red

    Private _currentColor As StoplightColors = StoplightColors.Green
    Private _logger As ILogger = New NullLogger()

    Public Event Changed As StoplightChangedHandler


		Public ReadOnly Property CurrentColor() As StoplightColors
			Get
        Return _currentColor
			End Get
		End Property

		<Dependency()> _
		Public Property Logger() As ILogger
			Get
        Return _logger
			End Get
			Set
        _logger = Value
			End Set
		End Property

		Public Sub [Next]()
      _currentColor += 1
      If _currentColor > StoplightColors.Red Then
        _currentColor = StoplightColors.Green
      End If
			RaiseChanged()
			logger.Write(String.Format("LIGHT CHANGED TO {0}", currentColor))
		End Sub

		Protected Sub RaiseChanged()
      Dim handlers As StoplightChangedHandler = ChangedEvent
      If Not handlers Is Nothing Then
        Dim e As New LightChangedEventArgs(_currentColor)
        RaiseEvent Changed(Me, e)
      End If
    End Sub

	End Class
End Namespace
