namespace wfFileInventory
{
    partial class fMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.bStartScan = new System.Windows.Forms.Button();
            this.dlgChooseFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.bSelectFolder = new System.Windows.Forms.Button();
            this.tbFolderPath = new System.Windows.Forms.TextBox();
            this.lbLogs = new System.Windows.Forms.ListBox();
            this.tvInventory = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tvSavedInventory = new System.Windows.Forms.TreeView();
            this.cbMeasureUnit = new System.Windows.Forms.ComboBox();
            this.lMeasureUnit = new System.Windows.Forms.Label();
            this.lOrderBy = new System.Windows.Forms.Label();
            this.rbAlphabetically = new System.Windows.Forms.RadioButton();
            this.rbTotalWeight = new System.Windows.Forms.RadioButton();
            this.lScanLabel = new System.Windows.Forms.Label();
            this.lScanTime = new System.Windows.Forms.Label();
            this.bFileOpen = new System.Windows.Forms.Button();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.bSaveInventory = new System.Windows.Forms.Button();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bStartScan
            // 
            resources.ApplyResources(this.bStartScan, "bStartScan");
            this.bStartScan.Name = "bStartScan";
            this.bStartScan.UseVisualStyleBackColor = true;
            this.bStartScan.Click += new System.EventHandler(this.bStartScan_Click);
            // 
            // tabMain
            // 
            resources.ApplyResources(this.tabMain, "tabMain");
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabPage2);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.bSelectFolder);
            this.tabPage1.Controls.Add(this.tbFolderPath);
            this.tabPage1.Controls.Add(this.lbLogs);
            this.tabPage1.Controls.Add(this.tvInventory);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // bSelectFolder
            // 
            resources.ApplyResources(this.bSelectFolder, "bSelectFolder");
            this.bSelectFolder.Name = "bSelectFolder";
            this.bSelectFolder.UseVisualStyleBackColor = true;
            this.bSelectFolder.Click += new System.EventHandler(this.bSelectFolder_Click);
            // 
            // tbFolderPath
            // 
            resources.ApplyResources(this.tbFolderPath, "tbFolderPath");
            this.tbFolderPath.Name = "tbFolderPath";
            // 
            // lbLogs
            // 
            resources.ApplyResources(this.lbLogs, "lbLogs");
            this.lbLogs.FormattingEnabled = true;
            this.lbLogs.Name = "lbLogs";
            // 
            // tvInventory
            // 
            resources.ApplyResources(this.tvInventory, "tvInventory");
            this.tvInventory.Name = "tvInventory";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tvSavedInventory);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tvSavedInventory
            // 
            resources.ApplyResources(this.tvSavedInventory, "tvSavedInventory");
            this.tvSavedInventory.Name = "tvSavedInventory";
            // 
            // cbMeasureUnit
            // 
            resources.ApplyResources(this.cbMeasureUnit, "cbMeasureUnit");
            this.cbMeasureUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMeasureUnit.FormattingEnabled = true;
            this.cbMeasureUnit.Items.AddRange(new object[] {
            resources.GetString("cbMeasureUnit.Items"),
            resources.GetString("cbMeasureUnit.Items1"),
            resources.GetString("cbMeasureUnit.Items2")});
            this.cbMeasureUnit.Name = "cbMeasureUnit";
            this.cbMeasureUnit.SelectedIndexChanged += new System.EventHandler(this.cbMeasureUnit_SelectedIndexChanged);
            // 
            // lMeasureUnit
            // 
            resources.ApplyResources(this.lMeasureUnit, "lMeasureUnit");
            this.lMeasureUnit.Name = "lMeasureUnit";
            // 
            // lOrderBy
            // 
            resources.ApplyResources(this.lOrderBy, "lOrderBy");
            this.lOrderBy.Name = "lOrderBy";
            // 
            // rbAlphabetically
            // 
            resources.ApplyResources(this.rbAlphabetically, "rbAlphabetically");
            this.rbAlphabetically.Name = "rbAlphabetically";
            this.rbAlphabetically.TabStop = true;
            this.rbAlphabetically.UseVisualStyleBackColor = true;
            this.rbAlphabetically.Click += new System.EventHandler(this.rbTotalWeight_Click);
            // 
            // rbTotalWeight
            // 
            resources.ApplyResources(this.rbTotalWeight, "rbTotalWeight");
            this.rbTotalWeight.Name = "rbTotalWeight";
            this.rbTotalWeight.TabStop = true;
            this.rbTotalWeight.UseVisualStyleBackColor = true;
            this.rbTotalWeight.Click += new System.EventHandler(this.rbTotalWeight_Click);
            // 
            // lScanLabel
            // 
            resources.ApplyResources(this.lScanLabel, "lScanLabel");
            this.lScanLabel.Name = "lScanLabel";
            // 
            // lScanTime
            // 
            resources.ApplyResources(this.lScanTime, "lScanTime");
            this.lScanTime.Name = "lScanTime";
            // 
            // bFileOpen
            // 
            resources.ApplyResources(this.bFileOpen, "bFileOpen");
            this.bFileOpen.Name = "bFileOpen";
            this.bFileOpen.UseVisualStyleBackColor = true;
            this.bFileOpen.Click += new System.EventHandler(this.bFileOpen_Click);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.FileName = "openFileDialog1";
            // 
            // bSaveInventory
            // 
            resources.ApplyResources(this.bSaveInventory, "bSaveInventory");
            this.bSaveInventory.Name = "bSaveInventory";
            this.bSaveInventory.UseVisualStyleBackColor = true;
            this.bSaveInventory.Click += new System.EventHandler(this.bSaveInventory_Click);
            // 
            // fMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bSaveInventory);
            this.Controls.Add(this.bFileOpen);
            this.Controls.Add(this.lScanTime);
            this.Controls.Add(this.lScanLabel);
            this.Controls.Add(this.rbTotalWeight);
            this.Controls.Add(this.rbAlphabetically);
            this.Controls.Add(this.lOrderBy);
            this.Controls.Add(this.lMeasureUnit);
            this.Controls.Add(this.cbMeasureUnit);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.bStartScan);
            this.Name = "fMain";
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bStartScan;
        private System.Windows.Forms.FolderBrowserDialog dlgChooseFolder;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView tvInventory;
        private System.Windows.Forms.ComboBox cbMeasureUnit;
        private System.Windows.Forms.Label lMeasureUnit;
        private System.Windows.Forms.Label lOrderBy;
        private System.Windows.Forms.RadioButton rbAlphabetically;
        private System.Windows.Forms.RadioButton rbTotalWeight;
        private System.Windows.Forms.Label lScanLabel;
        private System.Windows.Forms.Label lScanTime;
        private System.Windows.Forms.Button bFileOpen;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox lbLogs;
        private System.Windows.Forms.Button bSelectFolder;
        private System.Windows.Forms.TextBox tbFolderPath;
        private System.Windows.Forms.TreeView tvSavedInventory;
        private System.Windows.Forms.Button bSaveInventory;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
    }
}

