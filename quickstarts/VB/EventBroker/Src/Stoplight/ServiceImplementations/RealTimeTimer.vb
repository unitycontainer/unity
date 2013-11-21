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
Imports System.Windows.Forms
Imports SimpleEventBroker
Imports StopLight.ServiceInterfaces

Namespace StopLight.ServiceImplementations
	Friend Class RealTimeTimer
		Implements IStoplightTimer
    Private _timer As Timer

    <Publishes("TimerTick")> _
    Public Event Expired As EventHandler Implements IStoplightTimer.Expired

    Public Sub New()
      _timer = New Timer()
      AddHandler _timer.Tick, AddressOf OnTick
    End Sub

    Public Property Duration() As TimeSpan Implements IStoplightTimer.Duration
      Get
        Return TimeSpan.FromMilliseconds(_timer.Interval)
      End Get
      Set(ByVal value As TimeSpan)
        _timer.Interval = CType(value.TotalMilliseconds, Integer)
      End Set
    End Property

    Public Sub Start() Implements IStoplightTimer.Start
      _timer.Start()
    End Sub

    Private Sub OnTick(ByVal sender As Object, ByVal e As EventArgs)
      _timer.[Stop]()
      OnExpired(Me)
    End Sub

		Protected Overridable Sub OnExpired(ByVal sender As Object)
      Dim handlers As EventHandler = ExpiredEvent
      If Not handlers Is Nothing Then
        RaiseEvent Expired(Me, EventArgs.Empty)
      End If
		End Sub
	End Class
End Namespace

