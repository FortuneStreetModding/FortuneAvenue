
namespace CustomStreetManager
{
    partial class Form1
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

        private System.Windows.Forms.Label whichMapShouldWeReplaceLabel;
        private System.Windows.Forms.RadioButton trodainButton;
        private System.Windows.Forms.RadioButton observatoryButton;
        private System.Windows.Forms.RadioButton ghostShipButton;
        private System.Windows.Forms.RadioButton slimeniaButton;
        private System.Windows.Forms.RadioButton mtmagButton;
        private System.Windows.Forms.RadioButton rhrButton;
        private System.Windows.Forms.Label label2;
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.whichMapShouldWeReplaceLabel = new System.Windows.Forms.Label();
            this.trodainButton = new System.Windows.Forms.RadioButton();
            this.observatoryButton = new System.Windows.Forms.RadioButton();
            this.ghostShipButton = new System.Windows.Forms.RadioButton();
            this.slimeniaButton = new System.Windows.Forms.RadioButton();
            this.mtmagButton = new System.Windows.Forms.RadioButton();
            this.rhrButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.peachButton = new System.Windows.Forms.RadioButton();
            this.delfinoButton = new System.Windows.Forms.RadioButton();
            this.yoshiButton = new System.Windows.Forms.RadioButton();
            this.circuitButton = new System.Windows.Forms.RadioButton();
            this.starshipButton = new System.Windows.Forms.RadioButton();
            this.stadiumButton = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.alltradesButton = new System.Windows.Forms.RadioButton();
            this.colossusButton = new System.Windows.Forms.RadioButton();
            this.gegButton = new System.Windows.Forms.RadioButton();
            this.bowserButton = new System.Windows.Forms.RadioButton();
            this.smbButton = new System.Windows.Forms.RadioButton();
            this.alefgardButton = new System.Windows.Forms.RadioButton();
            this.replaceButton = new System.Windows.Forms.Button();
            this.noneButton = new System.Windows.Forms.RadioButton();
            this.addMapButton = new System.Windows.Forms.Button();
            this.clearListButton = new System.Windows.Forms.Button();
            this.listOfMapsToPatchIn = new System.Windows.Forms.ListView();
            this.customMapName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mapToReplace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filePathHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.isDynamic = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dynOrderHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.removeMapButton = new System.Windows.Forms.Button();
            this.addMapsDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deflaktorsASMHacksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeIntroMenuAndMapBgmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchToWiimmfiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutCustomStreetMapManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.setOutputLocationButton = new System.Windows.Forms.Button();
            this.setOutputPathLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.setInputISOLocation = new System.Windows.Forms.Label();
            this.dynMapLabel1 = new System.Windows.Forms.Label();
            this.dynMap2Label = new System.Windows.Forms.Label();
            this.dynMap3Label = new System.Windows.Forms.Label();
            this.dynMap4Label = new System.Windows.Forms.Label();
            this.map1OrderDropdown = new System.Windows.Forms.ComboBox();
            this.map2OrderDropdown = new System.Windows.Forms.ComboBox();
            this.map3OrderDropdown = new System.Windows.Forms.ComboBox();
            this.map4OrderDropdown = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.settingADynamicMap = new System.Windows.Forms.Label();
            this.dynTheObservatoryButton = new System.Windows.Forms.RadioButton();
            this.dynMtMagButton = new System.Windows.Forms.RadioButton();
            this.dynTheColossusButton = new System.Windows.Forms.RadioButton();
            this.dynamicMapPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.dynamicMapPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // whichMapShouldWeReplaceLabel
            // 
            this.whichMapShouldWeReplaceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.whichMapShouldWeReplaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.whichMapShouldWeReplaceLabel.Location = new System.Drawing.Point(3, 7);
            this.whichMapShouldWeReplaceLabel.Name = "whichMapShouldWeReplaceLabel";
            this.whichMapShouldWeReplaceLabel.Size = new System.Drawing.Size(393, 17);
            this.whichMapShouldWeReplaceLabel.TabIndex = 2;
            this.whichMapShouldWeReplaceLabel.Text = "Which map should we replace?";
            this.whichMapShouldWeReplaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trodainButton
            // 
            this.trodainButton.AutoSize = true;
            this.trodainButton.Location = new System.Drawing.Point(12, 53);
            this.trodainButton.Name = "trodainButton";
            this.trodainButton.Size = new System.Drawing.Size(93, 17);
            this.trodainButton.TabIndex = 3;
            this.trodainButton.Text = "Castle Trodain";
            this.trodainButton.UseVisualStyleBackColor = true;
            this.trodainButton.CheckedChanged += new System.EventHandler(this.trodainButton_CheckedChanged);
            // 
            // observatoryButton
            // 
            this.observatoryButton.AutoSize = true;
            this.observatoryButton.Location = new System.Drawing.Point(12, 76);
            this.observatoryButton.Name = "observatoryButton";
            this.observatoryButton.Size = new System.Drawing.Size(104, 17);
            this.observatoryButton.TabIndex = 5;
            this.observatoryButton.Text = "The Observatory";
            this.observatoryButton.UseVisualStyleBackColor = true;
            this.observatoryButton.CheckedChanged += new System.EventHandler(this.observatoryButton_CheckedChanged);
            // 
            // ghostShipButton
            // 
            this.ghostShipButton.AutoSize = true;
            this.ghostShipButton.Location = new System.Drawing.Point(12, 99);
            this.ghostShipButton.Name = "ghostShipButton";
            this.ghostShipButton.Size = new System.Drawing.Size(77, 17);
            this.ghostShipButton.TabIndex = 6;
            this.ghostShipButton.Text = "Ghost Ship";
            this.ghostShipButton.UseVisualStyleBackColor = true;
            this.ghostShipButton.CheckedChanged += new System.EventHandler(this.ghostShipButton_CheckedChanged);
            // 
            // slimeniaButton
            // 
            this.slimeniaButton.AutoSize = true;
            this.slimeniaButton.Location = new System.Drawing.Point(12, 122);
            this.slimeniaButton.Name = "slimeniaButton";
            this.slimeniaButton.Size = new System.Drawing.Size(64, 17);
            this.slimeniaButton.TabIndex = 7;
            this.slimeniaButton.Text = "Slimenia";
            this.slimeniaButton.UseVisualStyleBackColor = true;
            this.slimeniaButton.CheckedChanged += new System.EventHandler(this.slimeniaButton_CheckedChanged);
            // 
            // mtmagButton
            // 
            this.mtmagButton.AutoSize = true;
            this.mtmagButton.Location = new System.Drawing.Point(12, 145);
            this.mtmagButton.Name = "mtmagButton";
            this.mtmagButton.Size = new System.Drawing.Size(114, 17);
            this.mtmagButton.TabIndex = 8;
            this.mtmagButton.Text = "Mt. Magmageddon";
            this.mtmagButton.UseVisualStyleBackColor = true;
            this.mtmagButton.CheckedChanged += new System.EventHandler(this.mtmagButton_CheckedChanged);
            // 
            // rhrButton
            // 
            this.rhrButton.AutoSize = true;
            this.rhrButton.Location = new System.Drawing.Point(12, 168);
            this.rhrButton.Name = "rhrButton";
            this.rhrButton.Size = new System.Drawing.Size(120, 17);
            this.rhrButton.TabIndex = 9;
            this.rhrButton.Text = "Robbin\' Hood Ruins";
            this.rhrButton.UseVisualStyleBackColor = true;
            this.rhrButton.CheckedChanged += new System.EventHandler(this.robbinHoodRuinsButton_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 15);
            this.label2.TabIndex = 15;
            this.label2.Text = "Dragon Quest Tour";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(148, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 15);
            this.label3.TabIndex = 22;
            this.label3.Text = "Super Mario Tour";
            // 
            // peachButton
            // 
            this.peachButton.AutoSize = true;
            this.peachButton.Location = new System.Drawing.Point(156, 168);
            this.peachButton.Name = "peachButton";
            this.peachButton.Size = new System.Drawing.Size(95, 17);
            this.peachButton.TabIndex = 21;
            this.peachButton.Text = "Peach\'s Castle";
            this.peachButton.UseVisualStyleBackColor = true;
            this.peachButton.CheckedChanged += new System.EventHandler(this.peachButton_CheckedChanged);
            // 
            // delfinoButton
            // 
            this.delfinoButton.AutoSize = true;
            this.delfinoButton.Location = new System.Drawing.Point(156, 145);
            this.delfinoButton.Name = "delfinoButton";
            this.delfinoButton.Size = new System.Drawing.Size(87, 17);
            this.delfinoButton.TabIndex = 20;
            this.delfinoButton.Text = "Delfino Plaza";
            this.delfinoButton.UseVisualStyleBackColor = true;
            this.delfinoButton.CheckedChanged += new System.EventHandler(this.delfinoButton_CheckedChanged);
            // 
            // yoshiButton
            // 
            this.yoshiButton.AutoSize = true;
            this.yoshiButton.Location = new System.Drawing.Point(156, 122);
            this.yoshiButton.Name = "yoshiButton";
            this.yoshiButton.Size = new System.Drawing.Size(89, 17);
            this.yoshiButton.TabIndex = 19;
            this.yoshiButton.Text = "Yoshi\'s Island";
            this.yoshiButton.UseVisualStyleBackColor = true;
            this.yoshiButton.CheckedChanged += new System.EventHandler(this.yoshiButton_CheckedChanged);
            // 
            // circuitButton
            // 
            this.circuitButton.AutoSize = true;
            this.circuitButton.Location = new System.Drawing.Point(156, 99);
            this.circuitButton.Name = "circuitButton";
            this.circuitButton.Size = new System.Drawing.Size(83, 17);
            this.circuitButton.TabIndex = 18;
            this.circuitButton.Text = "Mario Circuit";
            this.circuitButton.UseVisualStyleBackColor = true;
            this.circuitButton.CheckedChanged += new System.EventHandler(this.circuitButton_CheckedChanged);
            // 
            // starshipButton
            // 
            this.starshipButton.AutoSize = true;
            this.starshipButton.Location = new System.Drawing.Point(156, 76);
            this.starshipButton.Name = "starshipButton";
            this.starshipButton.Size = new System.Drawing.Size(92, 17);
            this.starshipButton.TabIndex = 17;
            this.starshipButton.Text = "Starship Mario";
            this.starshipButton.UseVisualStyleBackColor = true;
            this.starshipButton.CheckedChanged += new System.EventHandler(this.starshipButton_CheckedChanged);
            // 
            // stadiumButton
            // 
            this.stadiumButton.AutoSize = true;
            this.stadiumButton.Location = new System.Drawing.Point(156, 53);
            this.stadiumButton.Name = "stadiumButton";
            this.stadiumButton.Size = new System.Drawing.Size(92, 17);
            this.stadiumButton.TabIndex = 16;
            this.stadiumButton.Text = "Mario Stadium";
            this.stadiumButton.UseVisualStyleBackColor = true;
            this.stadiumButton.CheckedChanged += new System.EventHandler(this.stadiumButton_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(275, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 15);
            this.label4.TabIndex = 29;
            this.label4.Text = "Special Tour";
            // 
            // alltradesButton
            // 
            this.alltradesButton.AutoSize = true;
            this.alltradesButton.Location = new System.Drawing.Point(281, 168);
            this.alltradesButton.Name = "alltradesButton";
            this.alltradesButton.Size = new System.Drawing.Size(98, 17);
            this.alltradesButton.TabIndex = 28;
            this.alltradesButton.Text = "Alltrades Abbey";
            this.alltradesButton.UseVisualStyleBackColor = true;
            this.alltradesButton.CheckedChanged += new System.EventHandler(this.alltradesButton_CheckedChanged);
            // 
            // colossusButton
            // 
            this.colossusButton.AutoSize = true;
            this.colossusButton.Location = new System.Drawing.Point(281, 145);
            this.colossusButton.Name = "colossusButton";
            this.colossusButton.Size = new System.Drawing.Size(67, 17);
            this.colossusButton.TabIndex = 27;
            this.colossusButton.Text = "Colossus";
            this.colossusButton.UseVisualStyleBackColor = true;
            this.colossusButton.CheckedChanged += new System.EventHandler(this.colossusButton_CheckedChanged);
            // 
            // gegButton
            // 
            this.gegButton.AutoSize = true;
            this.gegButton.Location = new System.Drawing.Point(281, 122);
            this.gegButton.Name = "gegButton";
            this.gegButton.Size = new System.Drawing.Size(108, 17);
            this.gegButton.TabIndex = 26;
            this.gegButton.Text = "Good Egg Galaxy";
            this.gegButton.UseVisualStyleBackColor = true;
            this.gegButton.CheckedChanged += new System.EventHandler(this.gegButton_CheckedChanged);
            // 
            // bowserButton
            // 
            this.bowserButton.AutoSize = true;
            this.bowserButton.Location = new System.Drawing.Point(281, 99);
            this.bowserButton.Name = "bowserButton";
            this.bowserButton.Size = new System.Drawing.Size(99, 17);
            this.bowserButton.TabIndex = 25;
            this.bowserButton.Text = "Bowser\'s Castle";
            this.bowserButton.UseVisualStyleBackColor = true;
            this.bowserButton.CheckedChanged += new System.EventHandler(this.bowserButton_CheckedChanged);
            // 
            // smbButton
            // 
            this.smbButton.AutoSize = true;
            this.smbButton.Location = new System.Drawing.Point(281, 76);
            this.smbButton.Name = "smbButton";
            this.smbButton.Size = new System.Drawing.Size(109, 17);
            this.smbButton.TabIndex = 24;
            this.smbButton.Text = "Super Mario Bros.";
            this.smbButton.UseVisualStyleBackColor = true;
            this.smbButton.CheckedChanged += new System.EventHandler(this.smbButton_CheckedChanged);
            // 
            // alefgardButton
            // 
            this.alefgardButton.AutoSize = true;
            this.alefgardButton.Location = new System.Drawing.Point(281, 53);
            this.alefgardButton.Name = "alefgardButton";
            this.alefgardButton.Size = new System.Drawing.Size(64, 17);
            this.alefgardButton.TabIndex = 23;
            this.alefgardButton.Text = "Alefgard";
            this.alefgardButton.UseVisualStyleBackColor = true;
            this.alefgardButton.CheckedChanged += new System.EventHandler(this.alefgardButton_CheckedChanged);
            // 
            // replaceButton
            // 
            this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.replaceButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replaceButton.Location = new System.Drawing.Point(290, 289);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(404, 31);
            this.replaceButton.TabIndex = 30;
            this.replaceButton.Text = "Go!";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.UpdateMapButton_Click);
            // 
            // noneButton
            // 
            this.noneButton.AutoSize = true;
            this.noneButton.Checked = true;
            this.noneButton.Location = new System.Drawing.Point(176, 191);
            this.noneButton.Margin = new System.Windows.Forms.Padding(1);
            this.noneButton.Name = "noneButton";
            this.noneButton.Size = new System.Drawing.Size(51, 17);
            this.noneButton.TabIndex = 34;
            this.noneButton.TabStop = true;
            this.noneButton.Text = "None";
            this.noneButton.UseVisualStyleBackColor = true;
            this.noneButton.CheckedChanged += new System.EventHandler(this.noneButton_CheckedChanged);
            // 
            // addMapButton
            // 
            this.addMapButton.Location = new System.Drawing.Point(3, 289);
            this.addMapButton.Margin = new System.Windows.Forms.Padding(1);
            this.addMapButton.Name = "addMapButton";
            this.addMapButton.Size = new System.Drawing.Size(95, 31);
            this.addMapButton.TabIndex = 35;
            this.addMapButton.Text = "Add map";
            this.addMapButton.UseVisualStyleBackColor = true;
            this.addMapButton.Click += new System.EventHandler(this.openMapButton_Click);
            // 
            // clearListButton
            // 
            this.clearListButton.Location = new System.Drawing.Point(191, 289);
            this.clearListButton.Margin = new System.Windows.Forms.Padding(1);
            this.clearListButton.Name = "clearListButton";
            this.clearListButton.Size = new System.Drawing.Size(95, 31);
            this.clearListButton.TabIndex = 37;
            this.clearListButton.Text = "Clear list";
            this.clearListButton.UseVisualStyleBackColor = true;
            this.clearListButton.Click += new System.EventHandler(this.clearListButton_Click);
            // 
            // listOfMapsToPatchIn
            // 
            this.listOfMapsToPatchIn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.customMapName,
            this.mapToReplace,
            this.filePathHeader,
            this.isDynamic,
            this.dynOrderHeader});
            this.listOfMapsToPatchIn.FullRowSelect = true;
            this.listOfMapsToPatchIn.HideSelection = false;
            this.listOfMapsToPatchIn.Location = new System.Drawing.Point(3, 25);
            this.listOfMapsToPatchIn.Margin = new System.Windows.Forms.Padding(1);
            this.listOfMapsToPatchIn.Name = "listOfMapsToPatchIn";
            this.listOfMapsToPatchIn.Size = new System.Drawing.Size(285, 262);
            this.listOfMapsToPatchIn.TabIndex = 39;
            this.listOfMapsToPatchIn.UseCompatibleStateImageBehavior = false;
            this.listOfMapsToPatchIn.View = System.Windows.Forms.View.Details;
            this.listOfMapsToPatchIn.SelectedIndexChanged += new System.EventHandler(this.listOfMapsToPatchIn_SelectedIndexChanged);
            // 
            // customMapName
            // 
            this.customMapName.Text = "Incoming Map";
            this.customMapName.Width = 89;
            // 
            // mapToReplace
            // 
            this.mapToReplace.Text = "Map To Replace";
            this.mapToReplace.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mapToReplace.Width = 108;
            // 
            // filePathHeader
            // 
            this.filePathHeader.Text = "File Path";
            this.filePathHeader.Width = 100;
            // 
            // isDynamic
            // 
            this.isDynamic.Text = "Is Dynamic?";
            // 
            // dynOrderHeader
            // 
            this.dynOrderHeader.Text = "Dynamic Order";
            this.dynOrderHeader.Width = 30;
            // 
            // removeMapButton
            // 
            this.removeMapButton.Location = new System.Drawing.Point(100, 289);
            this.removeMapButton.Margin = new System.Windows.Forms.Padding(1);
            this.removeMapButton.Name = "removeMapButton";
            this.removeMapButton.Size = new System.Drawing.Size(89, 31);
            this.removeMapButton.TabIndex = 40;
            this.removeMapButton.Text = "Remove map";
            this.removeMapButton.UseVisualStyleBackColor = true;
            this.removeMapButton.Click += new System.EventHandler(this.removeMapButton_Click);
            // 
            // addMapsDialog
            // 
            this.addMapsDialog.FileName = "openFileDialog1";
            this.addMapsDialog.ReadOnlyChecked = true;
            this.addMapsDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.addMapDialog_FileOk);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.alltradesButton);
            this.panel1.Controls.Add(this.rhrButton);
            this.panel1.Controls.Add(this.colossusButton);
            this.panel1.Controls.Add(this.noneButton);
            this.panel1.Controls.Add(this.trodainButton);
            this.panel1.Controls.Add(this.bowserButton);
            this.panel1.Controls.Add(this.observatoryButton);
            this.panel1.Controls.Add(this.ghostShipButton);
            this.panel1.Controls.Add(this.whichMapShouldWeReplaceLabel);
            this.panel1.Controls.Add(this.alefgardButton);
            this.panel1.Controls.Add(this.slimeniaButton);
            this.panel1.Controls.Add(this.mtmagButton);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.stadiumButton);
            this.panel1.Controls.Add(this.starshipButton);
            this.panel1.Controls.Add(this.circuitButton);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.yoshiButton);
            this.panel1.Controls.Add(this.gegButton);
            this.panel1.Controls.Add(this.delfinoButton);
            this.panel1.Controls.Add(this.smbButton);
            this.panel1.Controls.Add(this.peachButton);
            this.panel1.Location = new System.Drawing.Point(290, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 212);
            this.panel1.TabIndex = 41;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(698, 24);
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
            this.removeIntroMenuAndMapBgmToolStripMenuItem.Click += new System.EventHandler(this.removeIntroMenuAndMapBmgToolStripMenuItem_Click);
            // 
            // patchToWiimmfiToolStripMenuItem
            // 
            this.patchToWiimmfiToolStripMenuItem.CheckOnClick = true;
            this.patchToWiimmfiToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.patchToWiimmfiToolStripMenuItem.Name = "patchToWiimmfiToolStripMenuItem";
            this.patchToWiimmfiToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.patchToWiimmfiToolStripMenuItem.Text = "Wiimmfi";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutCustomStreetMapManagerToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutCustomStreetMapManagerToolStripMenuItem
            // 
            this.aboutCustomStreetMapManagerToolStripMenuItem.Name = "aboutCustomStreetMapManagerToolStripMenuItem";
            this.aboutCustomStreetMapManagerToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.aboutCustomStreetMapManagerToolStripMenuItem.Text = "About Custom Street Map Manager...";
            this.aboutCustomStreetMapManagerToolStripMenuItem.Click += new System.EventHandler(this.aboutCustomStreetMapManagerToolStripMenuItem_Click);
            // 
            // setOutputLocationButton
            // 
            this.setOutputLocationButton.Location = new System.Drawing.Point(290, 264);
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
            this.setOutputPathLabel.Location = new System.Drawing.Point(412, 264);
            this.setOutputPathLabel.Name = "setOutputPathLabel";
            this.setOutputPathLabel.Size = new System.Drawing.Size(282, 21);
            this.setOutputPathLabel.TabIndex = 44;
            this.setOutputPathLabel.Text = "None";
            this.setOutputPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(290, 240);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 23);
            this.button1.TabIndex = 45;
            this.button1.Text = "Set Input WBFS/ISO";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OpenFileDialog);
            // 
            // setInputISOLocation
            // 
            this.setInputISOLocation.Location = new System.Drawing.Point(412, 240);
            this.setInputISOLocation.Name = "setInputISOLocation";
            this.setInputISOLocation.Size = new System.Drawing.Size(282, 23);
            this.setInputISOLocation.TabIndex = 46;
            this.setInputISOLocation.Text = "None";
            this.setInputISOLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dynMapLabel1
            // 
            this.dynMapLabel1.Location = new System.Drawing.Point(1, 32);
            this.dynMapLabel1.Name = "dynMapLabel1";
            this.dynMapLabel1.Size = new System.Drawing.Size(96, 50);
            this.dynMapLabel1.TabIndex = 4;
            this.dynMapLabel1.Text = "map1";
            this.dynMapLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dynMap2Label
            // 
            this.dynMap2Label.Location = new System.Drawing.Point(102, 32);
            this.dynMap2Label.Name = "dynMap2Label";
            this.dynMap2Label.Size = new System.Drawing.Size(96, 47);
            this.dynMap2Label.TabIndex = 5;
            this.dynMap2Label.Text = "map2";
            this.dynMap2Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dynMap3Label
            // 
            this.dynMap3Label.Location = new System.Drawing.Point(204, 32);
            this.dynMap3Label.Name = "dynMap3Label";
            this.dynMap3Label.Size = new System.Drawing.Size(96, 47);
            this.dynMap3Label.TabIndex = 6;
            this.dynMap3Label.Text = "map3";
            this.dynMap3Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dynMap4Label
            // 
            this.dynMap4Label.Location = new System.Drawing.Point(303, 32);
            this.dynMap4Label.Name = "dynMap4Label";
            this.dynMap4Label.Size = new System.Drawing.Size(96, 47);
            this.dynMap4Label.TabIndex = 7;
            this.dynMap4Label.Text = "map4";
            this.dynMap4Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // map1OrderDropdown
            // 
            this.map1OrderDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.map1OrderDropdown.FormattingEnabled = true;
            this.map1OrderDropdown.Location = new System.Drawing.Point(27, 85);
            this.map1OrderDropdown.MaxDropDownItems = 4;
            this.map1OrderDropdown.Name = "map1OrderDropdown";
            this.map1OrderDropdown.Size = new System.Drawing.Size(40, 21);
            this.map1OrderDropdown.TabIndex = 8;
            // 
            // map2OrderDropdown
            // 
            this.map2OrderDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.map2OrderDropdown.FormattingEnabled = true;
            this.map2OrderDropdown.Location = new System.Drawing.Point(128, 85);
            this.map2OrderDropdown.MaxDropDownItems = 4;
            this.map2OrderDropdown.Name = "map2OrderDropdown";
            this.map2OrderDropdown.Size = new System.Drawing.Size(40, 21);
            this.map2OrderDropdown.TabIndex = 9;
            // 
            // map3OrderDropdown
            // 
            this.map3OrderDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.map3OrderDropdown.FormattingEnabled = true;
            this.map3OrderDropdown.Location = new System.Drawing.Point(229, 85);
            this.map3OrderDropdown.MaxDropDownItems = 4;
            this.map3OrderDropdown.Name = "map3OrderDropdown";
            this.map3OrderDropdown.Size = new System.Drawing.Size(40, 21);
            this.map3OrderDropdown.TabIndex = 10;
            // 
            // map4OrderDropdown
            // 
            this.map4OrderDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.map4OrderDropdown.FormattingEnabled = true;
            this.map4OrderDropdown.Location = new System.Drawing.Point(329, 85);
            this.map4OrderDropdown.MaxDropDownItems = 4;
            this.map4OrderDropdown.Name = "map4OrderDropdown";
            this.map4OrderDropdown.Size = new System.Drawing.Size(40, 21);
            this.map4OrderDropdown.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(396, 23);
            this.label1.TabIndex = 12;
            this.label1.Text = "Place the map files in the correct order:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingADynamicMap
            // 
            this.settingADynamicMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingADynamicMap.Location = new System.Drawing.Point(3, 114);
            this.settingADynamicMap.Name = "settingADynamicMap";
            this.settingADynamicMap.Size = new System.Drawing.Size(393, 25);
            this.settingADynamicMap.TabIndex = 13;
            this.settingADynamicMap.Text = "Choose which map to replace:";
            this.settingADynamicMap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dynTheObservatoryButton
            // 
            this.dynTheObservatoryButton.AutoSize = true;
            this.dynTheObservatoryButton.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.dynTheObservatoryButton.Location = new System.Drawing.Point(111, 142);
            this.dynTheObservatoryButton.Name = "dynTheObservatoryButton";
            this.dynTheObservatoryButton.Size = new System.Drawing.Size(172, 17);
            this.dynTheObservatoryButton.TabIndex = 14;
            this.dynTheObservatoryButton.TabStop = true;
            this.dynTheObservatoryButton.Text = "The Observatory (max: 4 maps)";
            this.dynTheObservatoryButton.UseVisualStyleBackColor = true;
            this.dynTheObservatoryButton.CheckedChanged += new System.EventHandler(this.dynTheObservatoryButton_CheckedChanged);
            // 
            // dynMtMagButton
            // 
            this.dynMtMagButton.AutoSize = true;
            this.dynMtMagButton.Location = new System.Drawing.Point(111, 165);
            this.dynMtMagButton.Name = "dynMtMagButton";
            this.dynMtMagButton.Size = new System.Drawing.Size(182, 17);
            this.dynMtMagButton.TabIndex = 15;
            this.dynMtMagButton.TabStop = true;
            this.dynMtMagButton.Text = "Mt. Magmageddon (max: 2 maps)";
            this.dynMtMagButton.UseVisualStyleBackColor = true;
            this.dynMtMagButton.CheckedChanged += new System.EventHandler(this.dynMtMagButton_CheckedChanged_1);
            // 
            // dynTheColossusButton
            // 
            this.dynTheColossusButton.AutoSize = true;
            this.dynTheColossusButton.Location = new System.Drawing.Point(111, 188);
            this.dynTheColossusButton.Name = "dynTheColossusButton";
            this.dynTheColossusButton.Size = new System.Drawing.Size(157, 17);
            this.dynTheColossusButton.TabIndex = 16;
            this.dynTheColossusButton.TabStop = true;
            this.dynTheColossusButton.Text = "The Colossus (max: 2 maps)";
            this.dynTheColossusButton.UseVisualStyleBackColor = true;
            this.dynTheColossusButton.CheckedChanged += new System.EventHandler(this.dynTheColossusButton_CheckedChanged_1);
            // 
            // dynamicMapPanel
            // 
            this.dynamicMapPanel.Controls.Add(this.dynTheColossusButton);
            this.dynamicMapPanel.Controls.Add(this.dynMtMagButton);
            this.dynamicMapPanel.Controls.Add(this.dynTheObservatoryButton);
            this.dynamicMapPanel.Controls.Add(this.settingADynamicMap);
            this.dynamicMapPanel.Controls.Add(this.label1);
            this.dynamicMapPanel.Controls.Add(this.map4OrderDropdown);
            this.dynamicMapPanel.Controls.Add(this.map3OrderDropdown);
            this.dynamicMapPanel.Controls.Add(this.map2OrderDropdown);
            this.dynamicMapPanel.Controls.Add(this.map1OrderDropdown);
            this.dynamicMapPanel.Controls.Add(this.dynMap4Label);
            this.dynamicMapPanel.Controls.Add(this.dynMap3Label);
            this.dynamicMapPanel.Controls.Add(this.dynMap2Label);
            this.dynamicMapPanel.Controls.Add(this.dynMapLabel1);
            this.dynamicMapPanel.Location = new System.Drawing.Point(290, 25);
            this.dynamicMapPanel.Name = "dynamicMapPanel";
            this.dynamicMapPanel.Size = new System.Drawing.Size(404, 212);
            this.dynamicMapPanel.TabIndex = 35;
            this.dynamicMapPanel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 327);
            this.Controls.Add(this.dynamicMapPanel);
            this.Controls.Add(this.setInputISOLocation);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.setOutputPathLabel);
            this.Controls.Add(this.setOutputLocationButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.removeMapButton);
            this.Controls.Add(this.listOfMapsToPatchIn);
            this.Controls.Add(this.clearListButton);
            this.Controls.Add(this.addMapButton);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Street Map Manager";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.dynamicMapPanel.ResumeLayout(false);
            this.dynamicMapPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton peachButton;
        private System.Windows.Forms.RadioButton delfinoButton;
        private System.Windows.Forms.RadioButton yoshiButton;
        private System.Windows.Forms.RadioButton circuitButton;
        private System.Windows.Forms.RadioButton starshipButton;
        private System.Windows.Forms.RadioButton stadiumButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton alltradesButton;
        private System.Windows.Forms.RadioButton colossusButton;
        private System.Windows.Forms.RadioButton gegButton;
        private System.Windows.Forms.RadioButton bowserButton;
        private System.Windows.Forms.RadioButton smbButton;
        private System.Windows.Forms.RadioButton alefgardButton;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.RadioButton noneButton;
        private System.Windows.Forms.Button addMapButton;
        private System.Windows.Forms.Button clearListButton;
        private System.Windows.Forms.ListView listOfMapsToPatchIn;
        private System.Windows.Forms.ColumnHeader customMapName;
        private System.Windows.Forms.ColumnHeader mapToReplace;
        private System.Windows.Forms.Button removeMapButton;
        private System.Windows.Forms.OpenFileDialog addMapsDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeIntroMenuAndMapBgmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patchToWiimmfiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutCustomStreetMapManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button setOutputLocationButton;
        private System.Windows.Forms.Label setOutputPathLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label setInputISOLocation;
        private System.Windows.Forms.ColumnHeader filePathHeader;
        private System.Windows.Forms.ColumnHeader isDynamic;
        private System.Windows.Forms.ColumnHeader dynOrderHeader;
        private System.Windows.Forms.ToolStripMenuItem deflaktorsASMHacksToolStripMenuItem;
        private System.Windows.Forms.Label dynMapLabel1;
        private System.Windows.Forms.Label dynMap2Label;
        private System.Windows.Forms.Label dynMap3Label;
        private System.Windows.Forms.Label dynMap4Label;
        private System.Windows.Forms.ComboBox map1OrderDropdown;
        private System.Windows.Forms.ComboBox map2OrderDropdown;
        private System.Windows.Forms.ComboBox map3OrderDropdown;
        private System.Windows.Forms.ComboBox map4OrderDropdown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label settingADynamicMap;
        private System.Windows.Forms.RadioButton dynTheObservatoryButton;
        private System.Windows.Forms.RadioButton dynMtMagButton;
        private System.Windows.Forms.RadioButton dynTheColossusButton;
        private System.Windows.Forms.Panel dynamicMapPanel;
    }
}

