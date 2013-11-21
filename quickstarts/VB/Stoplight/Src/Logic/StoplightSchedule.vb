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
    Public Class StoplightSchedule
        Private _timer As IStoplightTimer
        Private _logger As ILogger = New NullLogger()
        Private lightTimes As TimeSpan() = New TimeSpan(3) {}
        Private currentLight As Integer = 0

        Public Event ChangeLight As EventHandler

        Public Sub New(ByVal timer As IStoplightTimer)
            Me._timer = timer
            AddHandler _timer.Expired, AddressOf OnTimerExpired
        End Sub

        <Dependency()> _
        Public Property Logger() As ILogger
            Get
                Return _logger
            End Get
            Set(ByVal value As ILogger)
                _logger = value
            End Set
        End Property

        Public Sub Start()
            _timer.Start()
        End Sub

        Public Sub Update(ByVal green As TimeSpan, ByVal yellow As TimeSpan, ByVal red As TimeSpan)
            lightTimes(0) = green
            lightTimes(1) = yellow
            lightTimes(2) = red
            _logger.Write(String.Format("UPDATE SCHEDULE: {0} {1} {2}", green, yellow, red))
        End Sub

        Public Sub ForceChange()
            OnTimerExpired(Me, EventArgs.Empty)
            _logger.Write(String.Format("FORCED CHANGE"))
        End Sub

        Private Sub OnTimerExpired(ByVal sender As Object, ByVal e As EventArgs)
            Dim handlers As EventHandler = ChangeLightEvent
            If Not handlers Is Nothing Then
                handlers(Me, EventArgs.Empty)
            End If
            currentLight = (currentLight + 1) Mod 3
            _timer.Duration = lightTimes(currentLight)
            _timer.Start()
        End Sub
    End Class
End Namespace

