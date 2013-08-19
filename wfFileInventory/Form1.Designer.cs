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
            this.tbFolderPath = new System.Windows.Forms.TextBox();
            this.bSelectFolder = new System.Windows.Forms.Button();
            this.bStartScan = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tvInventory = new System.Windows.Forms.TreeView();
            this.cbMeasureUnit = new System.Windows.Forms.ComboBox();
            this.lMeasureUnit = new System.Windows.Forms.Label();
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbFolderPath
            // 
            resources.ApplyResources(this.tbFolderPath, "tbFolderPath");
            this.tbFolderPath.Name = "tbFolderPath";
            // 
            // bSelectFolder
            // 
            resources.ApplyResources(this.bSelectFolder, "bSelectFolder");
            this.bSelectFolder.Name = "bSelectFolder";
            this.bSelectFolder.UseVisualStyleBackColor = true;
            this.bSelectFolder.Click += new System.EventHandler(this.bSelectFolder_Click);
            // 
            // bStartScan
            // 
            resources.ApplyResources(this.bStartScan, "bStartScan");
            this.bStartScan.Name = "bStartScan";
            this.bStartScan.UseVisualStyleBackColor = true;
            this.bStartScan.Click += new System.EventHandler(this.bStartScan_Click);
            // 
            // folderBrowserDialog1
            // 
            resources.ApplyResources(this.folderBrowserDialog1, "folderBrowserDialog1");
            // 
            // tabMain
            // 
            resources.ApplyResources(this.tabMain, "tabMain");
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.tvInventory);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tvInventory
            // 
            resources.ApplyResources(this.tvInventory, "tvInventory");
            this.tvInventory.Name = "tvInventory";
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
            // fMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lMeasureUnit);
            this.Controls.Add(this.cbMeasureUnit);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.bStartScan);
            this.Controls.Add(this.bSelectFolder);
            this.Controls.Add(this.tbFolderPath);
            this.Name = "fMain";
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFolderPath;
        private System.Windows.Forms.Button bSelectFolder;
        private System.Windows.Forms.Button bStartScan;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView tvInventory;
        private System.Windows.Forms.ComboBox cbMeasureUnit;
        private System.Windows.Forms.Label lMeasureUnit;
    }
}

