namespace CustomStreetManager
{
    partial class AboutPanel
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
            this.aboutOKButton = new System.Windows.Forms.Button();
            this.csmmNameLabel = new System.Windows.Forms.Label();
            this.companyLabel = new System.Windows.Forms.Label();
            this.nikkumsHouseURLLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // aboutOKButton
            // 
            this.aboutOKButton.Location = new System.Drawing.Point(83, 95);
            this.aboutOKButton.Name = "aboutOKButton";
            this.aboutOKButton.Size = new System.Drawing.Size(94, 41);
            this.aboutOKButton.TabIndex = 0;
            this.aboutOKButton.Text = "OK";
            this.aboutOKButton.UseVisualStyleBackColor = true;
            this.aboutOKButton.Click += new System.EventHandler(this.aboutOKButton_Click);
            // 
            // csmmNameLabel
            // 
            this.csmmNameLabel.AutoSize = true;
            this.csmmNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.csmmNameLabel.Location = new System.Drawing.Point(40, 21);
            this.csmmNameLabel.Name = "csmmNameLabel";
            this.csmmNameLabel.Size = new System.Drawing.Size(196, 13);
            this.csmmNameLabel.TabIndex = 1;
            this.csmmNameLabel.Text = "Custom Street Map Manager 0.94";
            // 
            // companyLabel
            // 
            this.companyLabel.AutoSize = true;
            this.companyLabel.Location = new System.Drawing.Point(71, 44);
            this.companyLabel.Name = "companyLabel";
            this.companyLabel.Size = new System.Drawing.Size(118, 13);
            this.companyLabel.TabIndex = 2;
            this.companyLabel.Text = "©2020 Nikkums House";
            // 
            // nikkumsHouseURLLabel
            // 
            this.nikkumsHouseURLLabel.AutoSize = true;
            this.nikkumsHouseURLLabel.Location = new System.Drawing.Point(71, 57);
            this.nikkumsHouseURLLabel.Name = "nikkumsHouseURLLabel";
            this.nikkumsHouseURLLabel.Size = new System.Drawing.Size(125, 13);
            this.nikkumsHouseURLLabel.TabIndex = 3;
            this.nikkumsHouseURLLabel.TabStop = true;
            this.nikkumsHouseURLLabel.Text = "https://nikkums.io/csmm";
            // 
            // AboutPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 148);
            this.Controls.Add(this.nikkumsHouseURLLabel);
            this.Controls.Add(this.companyLabel);
            this.Controls.Add(this.csmmNameLabel);
            this.Controls.Add(this.aboutOKButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "AboutPanel";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About CSMM 0.94";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button aboutOKButton;
        private System.Windows.Forms.Label csmmNameLabel;
        private System.Windows.Forms.Label companyLabel;
        private System.Windows.Forms.LinkLabel nikkumsHouseURLLabel;
    }
}