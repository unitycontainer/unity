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

Namespace EventBrokerTests.TestObjects
	Class ClipboardMonitor
    Private _numberOfCopyOperations As Integer
    Private _numberOfPasteOperations As Integer

		Public Sub New()
      _numberOfPasteOperations = 0
      _numberOfCopyOperations = 0
		End Sub

		Public ReadOnly Property NumberOfCopyOperations() As Integer
			Get
        Return _numberOfCopyOperations
			End Get
		End Property

		Public ReadOnly Property NumberOfPasteOperations() As Integer
			Get
        Return _numberOfPasteOperations
			End Get
		End Property

		Public Sub OnClipboardCopy(ByVal sender As Object, ByVal e As EventArgs)
      System.Threading.Interlocked.Increment(_numberOfCopyOperations)
		End Sub

		Public Sub OnClipboardPaste(ByVal sender As Object, ByVal e As EventArgs)
      System.Threading.Interlocked.Increment(_numberOfPasteOperations)
		End Sub
	End Class
End Namespace
