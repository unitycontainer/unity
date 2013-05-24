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

Imports EventBrokerExtension
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Practices.Unity
Imports SimpleEventBroker
Imports Tests.EventBrokerExtension.Tests.EventBrokerExtension.TestObjects

<TestClass()> _
Public Class SimpleEventBrokerExtensionFixture
    <TestMethod()> _
    Public Sub ContainerCanWireEvents()
        Dim container As IUnityContainer = New UnityContainer() _
                .AddNewExtension (Of SimpleEventBrokerExtension)()

        Dim clipboard As ClipboardManager = container.Resolve (Of ClipboardManager)()

        Dim broker As EventBroker = container.Configure (Of ISimpleEventBrokerConfiguration)().Broker

        Dim registeredEvents As New List(Of String) (broker.RegisteredEvents)
        registeredEvents.Sort()

        Dim expectedEvents As New List(Of String) (New String() {"cut", "copy", "paste", "clipboard data available"})
        expectedEvents.Sort()

        CollectionAssert.AreEqual (expectedEvents, registeredEvents)
    End Sub
End Class
