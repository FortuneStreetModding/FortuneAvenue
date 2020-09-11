
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomStreetManager
{
    partial class CSMM
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSMM));
            this.clearListButton = new System.Windows.Forms.Button();
            this.addMapsDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deflaktorsASMHacksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeIntroMenuAndMapBgmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchToWiimmfiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.setOutputLocationButton = new System.Windows.Forms.Button();
            this.setOutputPathLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.setInputISOLocation = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameENDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.targetAmountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.themeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ruleSetDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.frbFile1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.frbFile2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.frbFile3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.frbFile4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Background = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BGMID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Venture_Card_Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Export = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ImportMd = new System.Windows.Forms.DataGridViewButtonColumn();
            this.mapDescriptorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Go = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapDescriptorBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // clearListButton
            // 
            this.clearListButton.Location = new System.Drawing.Point(1, 1);
            this.clearListButton.Margin = new System.Windows.Forms.Padding(1);
            this.clearListButton.Name = "clearListButton";
            this.clearListButton.Size = new System.Drawing.Size(95, 31);
            this.clearListButton.TabIndex = 37;
            this.clearListButton.Text = "Clear Changes";
            this.clearListButton.UseVisualStyleBackColor = true;
            this.clearListButton.Click += new System.EventHandler(this.clearListButton_Click);
            // 
            // addMapsDialog
            // 
            this.addMapsDialog.FileName = "openFileDialog1";
            this.addMapsDialog.ReadOnlyChecked = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1357, 24);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deflaktorsASMHacksToolStripMenuItem,
            this.removeIntroMenuAndMapBgmToolStripMenuItem,
            this.patchToWiimmfiToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.optionsToolStripMenuItem.Text = "Optional Patches";
            // 
            // deflaktorsASMHacksToolStripMenuItem
            // 
            this.deflaktorsASMHacksToolStripMenuItem.CheckOnClick = true;
            this.deflaktorsASMHacksToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deflaktorsASMHacksToolStripMenuItem.Name = "deflaktorsASMHacksToolStripMenuItem";
            this.deflaktorsASMHacksToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.deflaktorsASMHacksToolStripMenuItem.Text = "Deflaktor\'s ASM Hacks (PAL ONLY)";
            this.deflaktorsASMHacksToolStripMenuItem.Click += new System.EventHandler(this.deflaktorsASMHacksToolStripMenuItem_Click);
            // 
            // removeIntroMenuAndMapBgmToolStripMenuItem
            // 
            this.removeIntroMenuAndMapBgmToolStripMenuItem.CheckOnClick = true;
            this.removeIntroMenuAndMapBgmToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.removeIntroMenuAndMapBgmToolStripMenuItem.Name = "removeIntroMenuAndMapBgmToolStripMenuItem";
            this.removeIntroMenuAndMapBgmToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.removeIntroMenuAndMapBgmToolStripMenuItem.Text = "Disable All Music";
            // 
            // patchToWiimmfiToolStripMenuItem
            // 
            this.patchToWiimmfiToolStripMenuItem.CheckOnClick = true;
            this.patchToWiimmfiToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.patchToWiimmfiToolStripMenuItem.Name = "patchToWiimmfiToolStripMenuItem";
            this.patchToWiimmfiToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.patchToWiimmfiToolStripMenuItem.Text = "Wiimmfi";
            // 
            // setOutputLocationButton
            // 
            this.setOutputLocationButton.Location = new System.Drawing.Point(3, 3);
            this.setOutputLocationButton.Name = "setOutputLocationButton";
            this.setOutputLocationButton.Size = new System.Drawing.Size(116, 23);
            this.setOutputLocationButton.TabIndex = 43;
            this.setOutputLocationButton.Text = "Set Output ISO";
            this.setOutputLocationButton.UseVisualStyleBackColor = true;
            this.setOutputLocationButton.Click += new System.EventHandler(this.SaveFileDialog);
            // 
            // setOutputPathLabel
            // 
            this.setOutputPathLabel.AutoEllipsis = true;
            this.setOutputPathLabel.Location = new System.Drawing.Point(125, 0);
            this.setOutputPathLabel.Name = "setOutputPathLabel";
            this.setOutputPathLabel.Size = new System.Drawing.Size(282, 21);
            this.setOutputPathLabel.TabIndex = 44;
            this.setOutputPathLabel.Text = "None";
            this.setOutputPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 23);
            this.button1.TabIndex = 45;
            this.button1.Text = "Set Input WBFS/ISO";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OpenFileDialog);
            // 
            // setInputISOLocation
            // 
            this.setInputISOLocation.Location = new System.Drawing.Point(125, 0);
            this.setInputISOLocation.Name = "setInputISOLocation";
            this.setInputISOLocation.Size = new System.Drawing.Size(282, 23);
            this.setInputISOLocation.TabIndex = 46;
            this.setInputISOLocation.Text = "None";
            this.setInputISOLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Go, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1357, 922);
            this.tableLayoutPanel1.TabIndex = 47;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.setOutputLocationButton);
            this.flowLayoutPanel1.Controls.Add(this.setOutputPathLabel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(681, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(673, 34);
            this.flowLayoutPanel1.TabIndex = 42;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.clearListButton);
            this.flowLayoutPanel2.Controls.Add(this.checkBox1);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 885);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(672, 34);
            this.flowLayoutPanel2.TabIndex = 43;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBox1.Location = new System.Drawing.Point(100, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(93, 27);
            this.checkBox1.TabIndex = 38;
            this.checkBox1.Text = "Show Unused";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.updateDataGridData);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.button1);
            this.flowLayoutPanel3.Controls.Add(this.setInputISOLocation);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(672, 34);
            this.flowLayoutPanel3.TabIndex = 44;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Lavender;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.nameENDataGridViewTextBoxColumn,
            this.targetAmountDataGridViewTextBoxColumn,
            this.themeDataGridViewTextBoxColumn,
            this.ruleSetDataGridViewTextBoxColumn,
            this.frbFile1DataGridViewTextBoxColumn,
            this.frbFile2DataGridViewTextBoxColumn,
            this.frbFile3DataGridViewTextBoxColumn,
            this.frbFile4DataGridViewTextBoxColumn,
            this.Background,
            this.BGMID,
            this.Venture_Card_Count,
            this.Export,
            this.ImportMd});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView1, 2);
            this.dataGridView1.DataSource = this.mapDescriptorBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 43);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Size = new System.Drawing.Size(1351, 836);
            this.dataGridView1.TabIndex = 45;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridView1_RowPrePaint);
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Width = 30;
            // 
            // nameENDataGridViewTextBoxColumn
            // 
            this.nameENDataGridViewTextBoxColumn.DataPropertyName = "Name_EN";
            this.nameENDataGridViewTextBoxColumn.HeaderText = "Name_EN";
            this.nameENDataGridViewTextBoxColumn.Name = "nameENDataGridViewTextBoxColumn";
            this.nameENDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameENDataGridViewTextBoxColumn.Width = 150;
            // 
            // targetAmountDataGridViewTextBoxColumn
            // 
            this.targetAmountDataGridViewTextBoxColumn.DataPropertyName = "TargetAmount";
            this.targetAmountDataGridViewTextBoxColumn.HeaderText = "TargetAmount";
            this.targetAmountDataGridViewTextBoxColumn.Name = "targetAmountDataGridViewTextBoxColumn";
            this.targetAmountDataGridViewTextBoxColumn.ReadOnly = true;
            this.targetAmountDataGridViewTextBoxColumn.Width = 80;
            // 
            // themeDataGridViewTextBoxColumn
            // 
            this.themeDataGridViewTextBoxColumn.DataPropertyName = "Theme";
            this.themeDataGridViewTextBoxColumn.HeaderText = "Theme";
            this.themeDataGridViewTextBoxColumn.Name = "themeDataGridViewTextBoxColumn";
            this.themeDataGridViewTextBoxColumn.ReadOnly = true;
            this.themeDataGridViewTextBoxColumn.Width = 90;
            // 
            // ruleSetDataGridViewTextBoxColumn
            // 
            this.ruleSetDataGridViewTextBoxColumn.DataPropertyName = "RuleSet";
            this.ruleSetDataGridViewTextBoxColumn.HeaderText = "RuleSet";
            this.ruleSetDataGridViewTextBoxColumn.Name = "ruleSetDataGridViewTextBoxColumn";
            this.ruleSetDataGridViewTextBoxColumn.ReadOnly = true;
            this.ruleSetDataGridViewTextBoxColumn.Width = 70;
            // 
            // frbFile1DataGridViewTextBoxColumn
            // 
            this.frbFile1DataGridViewTextBoxColumn.DataPropertyName = "FrbFile1";
            this.frbFile1DataGridViewTextBoxColumn.HeaderText = "FrbFile1";
            this.frbFile1DataGridViewTextBoxColumn.Name = "frbFile1DataGridViewTextBoxColumn";
            this.frbFile1DataGridViewTextBoxColumn.ReadOnly = true;
            this.frbFile1DataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // frbFile2DataGridViewTextBoxColumn
            // 
            this.frbFile2DataGridViewTextBoxColumn.DataPropertyName = "FrbFile2";
            this.frbFile2DataGridViewTextBoxColumn.HeaderText = "FrbFile2";
            this.frbFile2DataGridViewTextBoxColumn.Name = "frbFile2DataGridViewTextBoxColumn";
            this.frbFile2DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // frbFile3DataGridViewTextBoxColumn
            // 
            this.frbFile3DataGridViewTextBoxColumn.DataPropertyName = "FrbFile3";
            this.frbFile3DataGridViewTextBoxColumn.HeaderText = "FrbFile3";
            this.frbFile3DataGridViewTextBoxColumn.Name = "frbFile3DataGridViewTextBoxColumn";
            this.frbFile3DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // frbFile4DataGridViewTextBoxColumn
            // 
            this.frbFile4DataGridViewTextBoxColumn.DataPropertyName = "FrbFile4";
            this.frbFile4DataGridViewTextBoxColumn.HeaderText = "FrbFile4";
            this.frbFile4DataGridViewTextBoxColumn.Name = "frbFile4DataGridViewTextBoxColumn";
            this.frbFile4DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Background
            // 
            this.Background.DataPropertyName = "Background";
            this.Background.HeaderText = "Background";
            this.Background.Name = "Background";
            this.Background.ReadOnly = true;
            // 
            // BGMID
            // 
            this.BGMID.DataPropertyName = "BGMID";
            this.BGMID.HeaderText = "BGMID";
            this.BGMID.Name = "BGMID";
            this.BGMID.ReadOnly = true;
            this.BGMID.Width = 50;
            // 
            // Venture_Card_Count
            // 
            this.Venture_Card_Count.DataPropertyName = "VentureCardActiveCount";
            this.Venture_Card_Count.HeaderText = "Venture Card Count";
            this.Venture_Card_Count.Name = "Venture_Card_Count";
            this.Venture_Card_Count.ReadOnly = true;
            this.Venture_Card_Count.Width = 115;
            // 
            // Export
            // 
            this.Export.HeaderText = "Export md";
            this.Export.Name = "Export";
            this.Export.ReadOnly = true;
            this.Export.Text = "Export md";
            this.Export.UseColumnTextForButtonValue = true;
            // 
            // ImportMd
            // 
            this.ImportMd.HeaderText = "Import md";
            this.ImportMd.Name = "ImportMd";
            this.ImportMd.ReadOnly = true;
            this.ImportMd.Text = "Import md";
            this.ImportMd.UseColumnTextForButtonValue = true;
            // 
            // mapDescriptorBindingSource
            // 
            this.mapDescriptorBindingSource.DataSource = typeof(FSEditor.MapDescriptor.MapDescriptor);
            // 
            // Go
            // 
            this.Go.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Go.Enabled = false;
            this.Go.Location = new System.Drawing.Point(681, 885);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(673, 34);
            this.Go.TabIndex = 46;
            this.Go.Text = "Go !";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // CSMM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1357, 946);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "CSMM";
            this.RightToLeftLayout = true;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Street Map Manager";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapDescriptorBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button clearListButton;
        private System.Windows.Forms.OpenFileDialog addMapsDialog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeIntroMenuAndMapBgmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patchToWiimmfiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button setOutputLocationButton;
        private System.Windows.Forms.Label setOutputPathLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label setInputISOLocation;
        private System.Windows.Forms.ToolStripMenuItem deflaktorsASMHacksToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource mapDescriptorBindingSource;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameENDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetAmountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn themeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ruleSetDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn frbFile1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn frbFile2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn frbFile3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn frbFile4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Background;
        private System.Windows.Forms.DataGridViewTextBoxColumn BGMID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Venture_Card_Count;
        private System.Windows.Forms.DataGridViewButtonColumn Export;
        private System.Windows.Forms.DataGridViewButtonColumn ImportMd;
    }
}

