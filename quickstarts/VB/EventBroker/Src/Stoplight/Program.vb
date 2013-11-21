' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Windows.Forms
Imports EventBrokerExtension
Imports Microsoft.Practices.Unity
Imports StopLight.ServiceImplementations
Imports StopLight.ServiceInterfaces
Imports StopLight.UI

Namespace StopLight
    Public Class Program
        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()> _
        Public Shared Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            Dim container As IUnityContainer = New UnityContainer() _
            .AddNewExtension(Of SimpleEventBrokerExtension)() _
            .RegisterType(Of ILogger, TraceLogger)() _
            .RegisterType(Of IStoplightTimer, RealTimeTimer)()

            Application.Run(container.Resolve(Of StoplightForm)())
        End Sub
    End Class
End Namespace
