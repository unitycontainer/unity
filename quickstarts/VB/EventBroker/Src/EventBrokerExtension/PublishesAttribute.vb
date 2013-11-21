' Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace SimpleEventBroker
	<AttributeUsage(AttributeTargets.[Event], Inherited := True)> _
	Public Class PublishesAttribute
		Inherits PublishSubscribeAttribute
		Public Sub New(ByVal publishedEventName As String)
			MyBase.New(publishedEventName)
		End Sub
	End Class
End Namespace
