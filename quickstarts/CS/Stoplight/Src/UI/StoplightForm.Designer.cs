// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace StopLight.UI
{
    partial class StoplightForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.stopLightPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.greenDurationTextBox = new System.Windows.Forms.TextBox();
            this.yellowDurationTextBox = new System.Windows.Forms.TextBox();
            this.redDurationTextBox = new System.Windows.Forms.TextBox();
            this.updateScheduleButton = new System.Windows.Forms.Button();
            this.forceChangeButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // stopLightPanel
            // 
            this.stopLightPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stopLightPanel.Location = new System.Drawing.Point(45, 39);
            this.stopLightPanel.Name = "stopLightPanel";
            this.stopLightPanel.Size = new System.Drawing.Size(213, 32);
            this.stopLightPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Green Duration:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Yellow Duration:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "&Red Duration:";
            // 
            // greenDurationTextBox
            // 
            this.greenDurationTextBox.Location = new System.Drawing.Point(131, 87);
            this.greenDurationTextBox.Name = "greenDurationTextBox";
            this.greenDurationTextBox.Size = new System.Drawing.Size(127, 20);
            this.greenDurationTextBox.TabIndex = 2;
            // 
            // yellowDurationTextBox
            // 
            this.yellowDurationTextBox.Location = new System.Drawing.Point(131, 114);
            this.yellowDurationTextBox.Name = "yellowDurationTextBox";
            this.yellowDurationTextBox.Size = new System.Drawing.Size(127, 20);
            this.yellowDurationTextBox.TabIndex = 4;
            // 
            // redDurationTextBox
            // 
            this.redDurationTextBox.Location = new System.Drawing.Point(131, 141);
            this.redDurationTextBox.Name = "redDurationTextBox";
            this.redDurationTextBox.Size = new System.Drawing.Size(127, 20);
            this.redDurationTextBox.TabIndex = 6;
            // 
            // updateScheduleButton
            // 
            this.updateScheduleButton.Location = new System.Drawing.Point(93, 188);
            this.updateScheduleButton.Name = "updateScheduleButton";
            this.updateScheduleButton.Size = new System.Drawing.Size(126, 23);
            this.updateScheduleButton.TabIndex = 7;
            this.updateScheduleButton.Text = "&Update Schedule";
            this.updateScheduleButton.UseVisualStyleBackColor = true;
            this.updateScheduleButton.Click += new System.EventHandler(this.OnUpdateScheduleClicked);
            // 
            // forceChangeButton
            // 
            this.forceChangeButton.Location = new System.Drawing.Point(93, 218);
            this.forceChangeButton.Name = "forceChangeButton";
            this.forceChangeButton.Size = new System.Drawing.Size(126, 23);
            this.forceChangeButton.TabIndex = 8;
            this.forceChangeButton.Text = "&Change Light";
            this.forceChangeButton.UseVisualStyleBackColor = true;
            this.forceChangeButton.Click += new System.EventHandler(this.OnChangeLightClicked);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // StoplightForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 292);
            this.Controls.Add(this.forceChangeButton);
            this.Controls.Add(this.updateScheduleButton);
            this.Controls.Add(this.redDurationTextBox);
            this.Controls.Add(this.yellowDurationTextBox);
            this.Controls.Add(this.greenDurationTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stopLightPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "StoplightForm";
            this.Text = "Stoplight Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel stopLightPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox greenDurationTextBox;
        private System.Windows.Forms.TextBox yellowDurationTextBox;
        private System.Windows.Forms.TextBox redDurationTextBox;
        private System.Windows.Forms.Button updateScheduleButton;
        private System.Windows.Forms.Button forceChangeButton;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
