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

Imports StopLight.ServiceInterfaces

Namespace StopLight.ServiceImplementations
	Friend Class NullLogger
    Implements ILogger

#Region "ILogger Members"

    Public Sub Write(ByVal message As String) Implements ILogger.Write
    End Sub

#End Region

  End Class
End Namespace
