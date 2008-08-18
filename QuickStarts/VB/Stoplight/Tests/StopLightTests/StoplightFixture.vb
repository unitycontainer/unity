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

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StopLight.Logic
Imports StopLightTests.TestDoubles

Namespace StopLightTests
	''' <summary>
	''' Summary description for StopLightFixture
	''' </summary>
  <TestClass()> _
  Public Class StoplightFixture

    Dim newColor As StoplightColors
    Dim eventFired As Boolean

    Private Sub SetColorAndEventFired(ByVal sender As Object, ByVal e As LightChangedEventArgs)
      eventFired = True
      newColor = e.CurrentColor
    End Sub

    <TestMethod()> _
    Public Sub ShouldInitializeToGreen()
      Dim light As New StopLight.Logic.Stoplight()
      Assert.AreEqual(StopLight.Logic.Stoplight.Green, light.CurrentColor)
    End Sub

    <TestMethod()> _
    Public Sub ShouldTransitionGreenToYellow()
      Dim light As New StopLight.Logic.Stoplight()
      light.[Next]()

      Assert.AreEqual(StopLight.Logic.Stoplight.Yellow, light.CurrentColor)
    End Sub

    <TestMethod()> _
    Public Sub ShouldTransitionYellowToRed()
      Dim light As New StopLight.Logic.Stoplight()
      light.[Next]()
      light.[Next]()

      Assert.AreEqual(StopLight.Logic.Stoplight.Red, light.CurrentColor)
    End Sub

    <TestMethod()> _
    Public Sub ShouldTransitionRedToGreen()
      Dim light As New StopLight.Logic.Stoplight()
      light.[Next]()
      light.[Next]()
      light.[Next]()

      Assert.AreEqual(StopLight.Logic.Stoplight.Green, light.CurrentColor)
    End Sub

    <TestMethod()> _
    Public Sub ShouldRaiseChangedEventOnTransition()
      Dim light As New StopLight.Logic.Stoplight()
      eventFired = False
      newColor = StoplightColors.Green
      AddHandler light.Changed, AddressOf SetColorAndEventFired
      light.[Next]()
      Assert.IsTrue(eventFired)
      Assert.AreEqual(StopLight.Logic.Stoplight.Yellow, newColor)
    End Sub

    <TestMethod()> _
    Public Sub ShouldLogMessageOnChange()
      Dim light As New StopLight.Logic.Stoplight()
      Dim logger As New MockLogger()
      light.Logger = logger
      light.[Next]()
      Assert.IsNotNull(logger.LastMessage)
      Assert.IsTrue(logger.LastMessage.StartsWith("LIGHT CHANGED TO"))
    End Sub
  End Class
End Namespace

