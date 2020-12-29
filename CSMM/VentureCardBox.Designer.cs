using System.Drawing;
using System.Windows.Forms;

namespace CustomStreetManager
{
    partial class VentureCardBox
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Lavender;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;

            this.cancelButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ventureCardId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ventureCardEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ventureCardDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cancelButton.Location = new System.Drawing.Point(0, 365);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(1);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(820, 37);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Close";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += CancelButton_Click;
            //
            // venturecard id
            //
            this.ventureCardId.HeaderText = "ID";
            this.ventureCardId.Name = "ID";
            this.ventureCardId.ReadOnly = true;
            this.ventureCardId.DataPropertyName = "Id";
            //
            // venturecard enabled
            //
            this.ventureCardEnabled.HeaderText = "Enabled";
            this.ventureCardEnabled.Name = "Enabled";
            this.ventureCardEnabled.ReadOnly = true;
            //
            // venturecard description
            //
            this.ventureCardDesc.HeaderText = "Description";
            this.ventureCardDesc.Name = "Description";
            this.ventureCardDesc.ReadOnly = true;
            this.ventureCardDesc.DataPropertyName = "Description";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.AutoGenerateColumns = false;
            var SBind = new BindingSource();
            SBind.DataSource = VanillaDatabase.getVentureCardTable();
            this.dataGridView1.DataSource = SBind;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ventureCardId,
            this.ventureCardEnabled,
            this.ventureCardDesc});
            // 
            // VentureCardBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(800, 900);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.Name = "VentureCardBox";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activated Venture Cards";
            this.Shown += VentureCardBox_Shown;
            this.ResumeLayout(false);

        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        #endregion
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ventureCardId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ventureCardEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn ventureCardDesc;
    }
}