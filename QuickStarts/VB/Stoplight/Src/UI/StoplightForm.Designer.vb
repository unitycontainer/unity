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

Namespace StopLight.UI
	Partial Class StoplightForm
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing AndAlso (Not components Is Nothing) Then
        components.Dispose()
      End If
			MyBase.Dispose(disposing)
		End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
      Me.components = New System.ComponentModel.Container()
      Me.stopLightPanel = New System.Windows.Forms.Panel()
      Me.label1 = New System.Windows.Forms.Label()
      Me.label2 = New System.Windows.Forms.Label()
      Me.label3 = New System.Windows.Forms.Label()
      Me.greenDurationTextBox = New System.Windows.Forms.TextBox()
      Me.yellowDurationTextBox = New System.Windows.Forms.TextBox()
      Me.redDurationTextBox = New System.Windows.Forms.TextBox()
      Me.updateScheduleButton = New System.Windows.Forms.Button()
      Me.forceChangeButton = New System.Windows.Forms.Button()
      Me.errorProvider = New System.Windows.Forms.ErrorProvider(Me.components)
      CType(Me.errorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      ' 
      ' stopLightPanel
      ' 
      Me.stopLightPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.stopLightPanel.Location = New System.Drawing.Point(45, 39)
      Me.stopLightPanel.Name = "stopLightPanel"
      Me.stopLightPanel.Size = New System.Drawing.Size(213, 32)
      Me.stopLightPanel.TabIndex = 0
      ' 
      ' label1
      ' 
      Me.label1.AutoSize = True
      Me.label1.Location = New System.Drawing.Point(42, 87)
      Me.label1.Name = "label1"
      Me.label1.Size = New System.Drawing.Size(82, 13)
      Me.label1.TabIndex = 1
      Me.label1.Text = "&Green Duration:"
      ' 
      ' label2
      ' 
      Me.label2.AutoSize = True
      Me.label2.Location = New System.Drawing.Point(42, 117)
      Me.label2.Name = "label2"
      Me.label2.Size = New System.Drawing.Size(84, 13)
      Me.label2.TabIndex = 3
      Me.label2.Text = "&Yellow Duration:"
      ' 
      ' label3
      ' 
      Me.label3.AutoSize = True
      Me.label3.Location = New System.Drawing.Point(42, 144)
      Me.label3.Name = "label3"
      Me.label3.Size = New System.Drawing.Size(73, 13)
      Me.label3.TabIndex = 5
      Me.label3.Text = "&Red Duration:"
      ' 
      ' greenDurationTextBox
      ' 
      Me.greenDurationTextBox.Location = New System.Drawing.Point(131, 87)
      Me.greenDurationTextBox.Name = "greenDurationTextBox"
      Me.greenDurationTextBox.Size = New System.Drawing.Size(127, 20)
      Me.greenDurationTextBox.TabIndex = 2
      ' 
      ' yellowDurationTextBox
      ' 
      Me.yellowDurationTextBox.Location = New System.Drawing.Point(131, 114)
      Me.yellowDurationTextBox.Name = "yellowDurationTextBox"
      Me.yellowDurationTextBox.Size = New System.Drawing.Size(127, 20)
      Me.yellowDurationTextBox.TabIndex = 4
      ' 
      ' redDurationTextBox
      ' 
      Me.redDurationTextBox.Location = New System.Drawing.Point(131, 141)
      Me.redDurationTextBox.Name = "redDurationTextBox"
      Me.redDurationTextBox.Size = New System.Drawing.Size(127, 20)
      Me.redDurationTextBox.TabIndex = 6
      ' 
      ' updateScheduleButton
      ' 
      Me.updateScheduleButton.Location = New System.Drawing.Point(93, 188)
      Me.updateScheduleButton.Name = "updateScheduleButton"
      Me.updateScheduleButton.Size = New System.Drawing.Size(126, 23)
      Me.updateScheduleButton.TabIndex = 7
      Me.updateScheduleButton.Text = "&Update Schedule"
      Me.updateScheduleButton.UseVisualStyleBackColor = True
      AddHandler Me.updateScheduleButton.Click, AddressOf Me.OnUpdateScheduleClicked
      ' 
      ' forceChangeButton
      ' 
      Me.forceChangeButton.Location = New System.Drawing.Point(93, 218)
      Me.forceChangeButton.Name = "forceChangeButton"
      Me.forceChangeButton.Size = New System.Drawing.Size(126, 23)
      Me.forceChangeButton.TabIndex = 8
      Me.forceChangeButton.Text = "&Change Light"
      Me.forceChangeButton.UseVisualStyleBackColor = True
      AddHandler Me.forceChangeButton.Click, AddressOf Me.OnChangeLightClicked
      ' 
      ' errorProvider
      ' 
      Me.errorProvider.ContainerControl = Me
      ' 
      ' StoplightForm
      ' 
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(312, 292)
      Me.Controls.Add(Me.forceChangeButton)
      Me.Controls.Add(Me.updateScheduleButton)
      Me.Controls.Add(Me.redDurationTextBox)
      Me.Controls.Add(Me.yellowDurationTextBox)
      Me.Controls.Add(Me.greenDurationTextBox)
      Me.Controls.Add(Me.label3)
      Me.Controls.Add(Me.label2)
      Me.Controls.Add(Me.label1)
      Me.Controls.Add(Me.stopLightPanel)
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
      Me.Name = "StoplightForm"
      Me.Text = "Stoplight Simulator"
      CType(Me.errorProvider, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

    End Sub
#End Region

		Private stopLightPanel As System.Windows.Forms.Panel
		Private label1 As System.Windows.Forms.Label
		Private label2 As System.Windows.Forms.Label
		Private label3 As System.Windows.Forms.Label
		Private greenDurationTextBox As System.Windows.Forms.TextBox
		Private yellowDurationTextBox As System.Windows.Forms.TextBox
		Private redDurationTextBox As System.Windows.Forms.TextBox
		Private updateScheduleButton As System.Windows.Forms.Button
		Private forceChangeButton As System.Windows.Forms.Button
		Private errorProvider As System.Windows.Forms.ErrorProvider
	End Class
End Namespace

