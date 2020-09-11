namespace CustomStreetManager
{
    partial class ProgressBar
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
            this.mapReplaceProgressBar = new System.Windows.Forms.ProgressBar();
            this.cancelButton = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mapReplaceProgressBar
            // 
            this.mapReplaceProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.mapReplaceProgressBar.BackColor = System.Drawing.SystemColors.ControlDark;
            this.mapReplaceProgressBar.ForeColor = System.Drawing.Color.DarkOrchid;
            this.mapReplaceProgressBar.Location = new System.Drawing.Point(13, 140);
            this.mapReplaceProgressBar.Margin = new System.Windows.Forms.Padding(1);
            this.mapReplaceProgressBar.Name = "mapReplaceProgressBar";
            this.mapReplaceProgressBar.Size = new System.Drawing.Size(396, 28);
            this.mapReplaceProgressBar.Step = 20;
            this.mapReplaceProgressBar.TabIndex = 0;
            this.mapReplaceProgressBar.Value = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(104, 179);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(1);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(212, 32);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Close";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel.Location = new System.Drawing.Point(10, 11);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(401, 26);
            this.progressLabel.TabIndex = 2;
            this.progressLabel.Text = "label1";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(13, 51);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox.Size = new System.Drawing.Size(396, 72);
            this.textBox.TabIndex = 3;
            // 
            // ProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(421, 221);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.mapReplaceProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.Name = "ProgressBar";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updating ISO";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProgressBar_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar mapReplaceProgressBar;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.TextBox textBox;
    }
}