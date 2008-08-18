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
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StopLight.Logic
Imports StopLightTests.TestDoubles

Namespace StopLightTests

  ''' <summary>
  ''' Summary description for StoplightScheduleFixture
  ''' </summary>
  <TestClass()> _
  Public Class StoplightScheduleFixture

    Dim lightChangedFired As Boolean
    Dim eventFired As Boolean

    Private Sub SetLightChangedFired(ByVal sender As Object, ByVal e As EventArgs)
      lightChangedFired = True
    End Sub

    Private Sub SetEventFired(ByVal sender As Object, ByVal e As EventArgs)
      eventFired = True
    End Sub

    <TestMethod()> _
    Public Sub ShouldFireChangeLightOnTimeExpiration()
      Dim _timer As New MockStoplightTimer()
      Dim schedule As New StoplightSchedule(_timer)
      lightChangedFired = False
      AddHandler schedule.ChangeLight, AddressOf SetLightChangedFired
      _timer.Expire()
      Assert.IsTrue(lightChangedFired)
    End Sub

    <TestMethod()> _
    Public Sub ShouldSetTimerDurationForNextLight()
      Dim timer As New MockStoplightTimer()
      Dim schedule As New StoplightSchedule(timer)
      Dim greenSchedule As New TimeSpan(800)
      Dim yellowSchedule As New TimeSpan(1500)
      Dim redSchedule As New TimeSpan(100)
      schedule.Update(greenSchedule, yellowSchedule, redSchedule)

      timer.Expire()
      Assert.AreEqual(yellowSchedule, timer.Duration)
    End Sub


    <TestMethod()> _
    Public Sub ShouldFireEventAndUpdateScheduleOnForceChange()
      Dim _timer As New MockStoplightTimer()
      Dim schedule As New StoplightSchedule(_timer)
      Dim greenSchedule As New TimeSpan(800)
      Dim yellowSchedule As New TimeSpan(1500)
      Dim redSchedule As New TimeSpan(100)
      schedule.Update(greenSchedule, yellowSchedule, redSchedule)
      eventFired = False
      AddHandler schedule.ChangeLight, AddressOf SetEventFired
      schedule.ForceChange()
      Assert.IsTrue(eventFired)
      Assert.AreEqual(yellowSchedule, _timer.Duration)
    End Sub

    <TestMethod()> _
    Public Sub ShouldLogOnUpdate()
      Dim timer As New MockStoplightTimer()
      Dim logger As New MockLogger()
      Dim schedule As New StoplightSchedule(timer)
      schedule.Logger = logger

      Dim greenSchedule As New TimeSpan(800)
      Dim yellowSchedule As New TimeSpan(1500)
      Dim redSchedule As New TimeSpan(100)

      schedule.Update(greenSchedule, yellowSchedule, redSchedule)

      Assert.IsNotNull(logger.LastMessage)
      Assert.IsTrue(logger.LastMessage.StartsWith("UPDATE"))
    End Sub

    <TestMethod()> _
    Public Sub ShouldLogOnForceChange()
      Dim timer As New MockStoplightTimer()
      Dim logger As New MockLogger()
      Dim schedule As New StoplightSchedule(timer)
      schedule.Logger = logger
      Dim greenSchedule As New TimeSpan(800)
      Dim yellowSchedule As New TimeSpan(1500)
      Dim redSchedule As New TimeSpan(100)
      schedule.Update(greenSchedule, yellowSchedule, redSchedule)

      schedule.ForceChange()
      Assert.IsNotNull(logger.LastMessage)
      Assert.IsTrue(logger.LastMessage.StartsWith("FORCED CHANGE"))
    End Sub
  End Class
End Namespace

