' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports StopLight.ServiceInterfaces

Namespace StopLightTests.TestDoubles
	Friend Class MockLogger
		Implements ILogger
    Private lastMsg As String

    Public Property LastMessage() As String
      Get
        Return lastMsg
      End Get
      Private Set(ByVal value As String)
        lastMsg = value
      End Set
    End Property

#Region "ILogger Members"

    Public Sub Write(ByVal message As String) Implements ILogger.Write
      LastMessage = message
    End Sub

#End Region

  End Class
End Namespace


