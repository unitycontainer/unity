' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Windows.Forms
Imports StopLight.ServiceInterfaces

Namespace StopLight.ServiceImplementations
	Friend Class RealTimeTimer
		Implements IStoplightTimer
    Private _timer As Timer

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
        _timer.Interval = value.TotalMilliseconds
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
        handlers(Me, EventArgs.Empty)
      End If
    End Sub

	End Class
End Namespace

