namespace wfFileInventory
{
    /// <summary>
    /// Modal form for displaying scan progress
    /// </summary>
    partial class modalScanProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(modalScanProgress));
            this.lCurrentDirectory = new System.Windows.Forms.Label();
            this.bStopScan = new System.Windows.Forms.Button();
            this.pbScanProgress = new System.Windows.Forms.ProgressBar();
            this.lTimer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lCurrentDirectory
            // 
            resources.ApplyResources(this.lCurrentDirectory, "lCurrentDirectory");
            this.lCurrentDirectory.Name = "lCurrentDirectory";
            // 
            // bStopScan
            // 
            resources.ApplyResources(this.bStopScan, "bStopScan");
            this.bStopScan.Name = "bStopScan";
            this.bStopScan.UseVisualStyleBackColor = true;
            this.bStopScan.Click += new System.EventHandler(this.bStopScan_Click);
            // 
            // pbScanProgress
            // 
            resources.ApplyResources(this.pbScanProgress, "pbScanProgress");
            this.pbScanProgress.Name = "pbScanProgress";
            // 
            // lTimer
            // 
            resources.ApplyResources(this.lTimer, "lTimer");
            this.lTimer.Name = "lTimer";
            // 
            // modalScanProgress
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lTimer);
            this.Controls.Add(this.pbScanProgress);
            this.Controls.Add(this.bStopScan);
            this.Controls.Add(this.lCurrentDirectory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "modalScanProgress";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lCurrentDirectory;
        private System.Windows.Forms.Button bStopScan;
        private System.Windows.Forms.ProgressBar pbScanProgress;
        private System.Windows.Forms.Label lTimer;
    }
}