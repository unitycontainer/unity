' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.Practices.Unity

Namespace StopLight.UI
	Public Partial Class StoplightForm
		Inherits Form
		Implements IStoplightView
    Private _presenter As StoplightPresenter

    Public Sub New()
      InitializeComponent()
    End Sub

    <Dependency()> _
    Public Property Presenter() As StoplightPresenter
      Get
        Return _presenter
      End Get
      Set(ByVal value As StoplightPresenter)
        _presenter = value
        presenter.SetView(Me)
      End Set
    End Property

#Region "IStoplightView Members"

    Public Property CurrentColor() As Color Implements IStoplightView.CurrentColor
      Get
        Return stopLightPanel.BackColor
      End Get
      Set(ByVal value As Color)
        stopLightPanel.BackColor = value
        RaisePropertyChanged(StoplightViewProperties.CurrentColor)
      End Set
    End Property

    Public Property GreenDuration() As String Implements IStoplightView.GreenDuration
      Get
        Return greenDurationTextBox.Text
      End Get
      Set(ByVal value As String)
        greenDurationTextBox.Text = value
        RaisePropertyChanged(StoplightViewProperties.GreenDuration)
      End Set
    End Property

    Public Property YellowDuration() As String Implements IStoplightView.YellowDuration
      Get
        Return yellowDurationTextBox.Text
      End Get
      Set(ByVal value As String)
        yellowDurationTextBox.Text = value
        RaisePropertyChanged(StoplightViewProperties.YellowDuration)
      End Set
    End Property

    Public Property RedDuration() As String Implements IStoplightView.RedDuration
      Get
        Return redDurationTextBox.Text
      End Get
      Set(ByVal value As String)
        redDurationTextBox.Text = value
        RaisePropertyChanged(StoplightViewProperties.RedDuration)
      End Set
    End Property

    Public Event UpdateClicked As EventHandler Implements IStoplightView.UpdateClicked

    Public Event ForceChangeClicked As EventHandler Implements IStoplightView.ForceChangeClicked

    Public Sub SetError(ByVal propertyName As String, ByVal errorMessage As String) Implements IStoplightView.SetError
      Dim controlsByName As New Dictionary(Of String, Control)()
      controlsByName.Add(StoplightViewProperties.GreenDuration, greenDurationTextBox)
      controlsByName.Add(StoplightViewProperties.YellowDuration, yellowDurationTextBox)
      controlsByName.Add(StoplightViewProperties.RedDuration, redDurationTextBox)


      If controlsByName.ContainsKey(propertyName) Then
        errorProvider.SetError(controlsByName(propertyName), errorMessage)
      End If
    End Sub

#End Region

#Region "INotifyPropertyChanged Members"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#End Region

		' Event firing helpers

		Protected Overridable Sub RaisePropertyChanged(ByVal propertyName As String)
      Dim handlers As PropertyChangedEventHandler = PropertyChangedEvent
      If Not handlers Is Nothing Then
        handlers(Me, New PropertyChangedEventArgs(propertyName))
      End If
		End Sub

		Protected Overridable Sub RaiseUpdateClicked()
      Dim handlers As EventHandler = UpdateClickedEvent
      If Not handlers Is Nothing Then
        handlers(Me, EventArgs.Empty)
      End If
		End Sub

		Protected Overridable Sub RaiseForceChangeClicked()
      Dim handlers As EventHandler = ForceChangeClickedEvent
      If Not handlers Is Nothing Then
        handlers(Me, EventArgs.Empty)
      End If
		End Sub

		Private Sub OnUpdateScheduleClicked(ByVal sender As Object, ByVal e As EventArgs)
			RaiseUpdateClicked()
		End Sub

		Private Sub OnChangeLightClicked(ByVal sender As Object, ByVal e As EventArgs)
			RaiseForceChangeClicked()
		End Sub
	End Class
End Namespace

