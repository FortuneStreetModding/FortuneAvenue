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
            this.SuspendLayout();
            // 
            // mapReplaceProgressBar
            // 
            this.mapReplaceProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapReplaceProgressBar.BackColor = System.Drawing.SystemColors.ControlDark;
            this.mapReplaceProgressBar.ForeColor = System.Drawing.Color.DarkOrchid;
            this.mapReplaceProgressBar.Location = new System.Drawing.Point(12, 112);
            this.mapReplaceProgressBar.Name = "mapReplaceProgressBar";
            this.mapReplaceProgressBar.Size = new System.Drawing.Size(565, 53);
            this.mapReplaceProgressBar.Step = 20;
            this.mapReplaceProgressBar.TabIndex = 0;
            this.mapReplaceProgressBar.UseWaitCursor = true;
            this.mapReplaceProgressBar.Value = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(185, 200);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(223, 72);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.UseWaitCursor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.Location = new System.Drawing.Point(12, 25);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(565, 63);
            this.progressLabel.TabIndex = 2;
            this.progressLabel.Text = "label1";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.progressLabel.UseWaitCursor = true;
            this.progressLabel.Click += new System.EventHandler(this.progressLabel_Click);
            // 
            // ProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(592, 296);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.mapReplaceProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ProgressBar";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updating ISO";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.ProgressBar_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar mapReplaceProgressBar;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label progressLabel;
    }
}