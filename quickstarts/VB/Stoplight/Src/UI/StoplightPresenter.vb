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
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.Practices.Unity
Imports StopLight.Logic

Namespace StopLight.UI
	Public Class StoplightPresenter
    Private _stopLight As StopLight.Logic.Stoplight
    Private _schedule As StoplightSchedule

    <Dependency()> _
    Public Property Stoplight() As StopLight.Logic.Stoplight
      Get
        Return _stopLight
      End Get
      Set(ByVal value As StopLight.Logic.Stoplight)
        _stopLight = value
      End Set
    End Property

    <Dependency()> _
    Public Property Schedule() As StoplightSchedule
      Get
        Return _schedule
      End Get
      Set(ByVal value As StoplightSchedule)
        _schedule = value
      End Set
    End Property

    Private view As IStoplightView


    Public Sub SetView(ByVal view As IStoplightView)
      Me.view = view
      AddHandler view.PropertyChanged, AddressOf OnViewPropertyChanged
      AddHandler view.UpdateClicked, AddressOf OnViewUpdateClicked
      AddHandler view.ForceChangeClicked, AddressOf OnViewForceChangeClicked

      AddHandler Schedule.ChangeLight, AddressOf CallStopLightNext
      AddHandler Stoplight.Changed, AddressOf OnStoplightChanged

      view.GreenDuration = "3000"
      view.YellowDuration = "500"
      view.RedDuration = "5000"
      view.CurrentColor = Color.Green

      _schedule.Update(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(5000))
      _schedule.Start()
    End Sub

    Private Sub CallStopLightNext(ByVal sender As Object, ByVal e As EventArgs)
      Stoplight.Next()
    End Sub

    Private Delegate Function ColorDelegate() As String

    Private Sub OnViewPropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)

      If e.PropertyName = StoplightViewProperties.CurrentColor Then
        Return
      End If

      Dim callGreenDelegate As New ColorDelegate(AddressOf GetGreenDuration)
      Dim callYellowDelegate As New ColorDelegate(AddressOf GetYellowDuration)
      Dim callRedDelegate As New ColorDelegate(AddressOf GetRedDuration)

      ActOnProperty(e.PropertyName, callGreenDelegate, callYellowDelegate, callRedDelegate)

    End Sub

    Private Function GetGreenDuration() As String
      Return view.GreenDuration
    End Function

    Private Function GetYellowDuration() As String
      Return view.YellowDuration
    End Function

    Private Function GetRedDuration() As String
      Return view.RedDuration
    End Function

    Private Sub ActOnProperty(ByVal propertyName As String, ByVal greenAction As ColorDelegate, ByVal yellowAction As ColorDelegate, ByVal redAction As ColorDelegate)
      Select Case propertyName
        Case StoplightViewProperties.GreenDuration
          greenAction()
          Exit Select
        Case StoplightViewProperties.YellowDuration
          yellowAction()
          Exit Select
        Case StoplightViewProperties.RedDuration
          redAction()
          Exit Select
        Case StoplightViewProperties.CurrentColor
          Return
      End Select
    End Sub

    Private Sub OnViewForceChangeClicked(ByVal sender As Object, ByVal e As EventArgs)
      _schedule.ForceChange()
    End Sub

    Private durationsAsString As String() = New String(3) {}
    Private Delegate Function UpdateGreenDelegate() As String
    Private Delegate Function UpdateYellowDelegate() As String
    Private Delegate Function UpdateRedDelegate() As String

    Private Sub OnViewUpdateClicked(ByVal sender As Object, ByVal e As EventArgs)
      Dim propNames As New List(Of String)(New String() {StoplightViewProperties.GreenDuration, StoplightViewProperties.YellowDuration, StoplightViewProperties.RedDuration})

      Dim callGreenDelegate As New ColorDelegate(AddressOf UpdateGreenDuration)
      Dim callYellowDelegate As New ColorDelegate(AddressOf UpdateYellowDuration)
      Dim callRedDelegate As New ColorDelegate(AddressOf UpdateRedDuration)

      For Each propName As String In propNames
        ActOnProperty(propName, callGreenDelegate, callYellowDelegate, callRedDelegate)
      Next

      Dim timeSpans(2) As TimeSpan
      For i As Integer = 0 To 2
        timeSpans(i) = GetTheTimeSpan(durationsAsString(i))
      Next
      _schedule.Update(timeSpans(0), timeSpans(1), timeSpans(2))

    End Sub

    Private Function UpdateGreenDuration() As String
      durationsAsString(0) = view.GreenDuration
      Return view.GreenDuration
    End Function

    Private Function UpdateYellowDuration() As String
      durationsAsString(1) = view.YellowDuration
      Return view.YellowDuration
    End Function

    Private Function UpdateRedDuration() As String
      durationsAsString(2) = view.RedDuration
      Return view.RedDuration
    End Function

    Private Function GetTheTimeSpan(ByVal s As String) As TimeSpan
      Return TimeSpan.FromMilliseconds(Integer.Parse(s))
    End Function

    Private Sub OnStoplightChanged(ByVal sender As Object, ByVal e As LightChangedEventArgs)
      Select Case e.CurrentColor
        Case StoplightColors.Green
          view.CurrentColor = Color.Green
          Exit Select
        Case StoplightColors.Yellow

          view.CurrentColor = Color.Yellow
          Exit Select
        Case StoplightColors.Red

          view.CurrentColor = Color.Red
          Exit Select
      End Select
    End Sub
  End Class
End Namespace

