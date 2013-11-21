' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Diagnostics
Imports StopLight.ServiceInterfaces

Namespace StopLight.ServiceImplementations
	Friend Class TraceLogger
		Implements ILogger

#Region "ILogger Members"

    Public Sub Write(ByVal message As String) Implements ILogger.Write
      Trace.WriteLine(String.Format("{0}: {1}", DateTime.Now, message))
    End Sub

#End Region

  End Class
End Namespace
