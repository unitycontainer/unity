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
