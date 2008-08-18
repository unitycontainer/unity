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

