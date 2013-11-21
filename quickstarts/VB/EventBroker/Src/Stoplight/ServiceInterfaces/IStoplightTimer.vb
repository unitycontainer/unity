' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System

Namespace StopLight.ServiceInterfaces
	Public Interface IStoplightTimer
		Property Duration() As TimeSpan
		Sub Start()
		Event Expired As EventHandler
	End Interface
End Namespace

