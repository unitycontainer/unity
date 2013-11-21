' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
