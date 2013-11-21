' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.ComponentModel
Imports System.Drawing

Namespace StopLight.UI
	Public Interface IStoplightView
		Inherits INotifyPropertyChanged
		Property CurrentColor() As Color

		Property GreenDuration() As String
		Property YellowDuration() As String
		Property RedDuration() As String

		Event UpdateClicked As EventHandler
		Event ForceChangeClicked As EventHandler

		Sub SetError(ByVal propertyName As String, ByVal errorMessage As String)
	End Interface

  Public Class StoplightViewProperties
    Public Const CurrentColor As String = "CurrentColor"
    Public Const GreenDuration As String = "GreenDuration"
    Public Const YellowDuration As String = "YellowDuration"
    Public Const RedDuration As String = "RedDuration"
  End Class
End Namespace
