namespace wfFileInventory
{
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
            this.lCurrentDirectory = new System.Windows.Forms.Label();
            this.bStopScan = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lCurrentDirectory
            // 
            this.lCurrentDirectory.AutoSize = true;
            this.lCurrentDirectory.Location = new System.Drawing.Point(13, 13);
            this.lCurrentDirectory.Name = "lCurrentDirectory";
            this.lCurrentDirectory.Size = new System.Drawing.Size(35, 13);
            this.lCurrentDirectory.TabIndex = 0;
            this.lCurrentDirectory.Text = "label1";
            // 
            // bStopScan
            // 
            this.bStopScan.Location = new System.Drawing.Point(205, 69);
            this.bStopScan.Name = "bStopScan";
            this.bStopScan.Size = new System.Drawing.Size(96, 23);
            this.bStopScan.TabIndex = 1;
            this.bStopScan.Text = "Stop scanning";
            this.bStopScan.UseVisualStyleBackColor = true;
            // 
            // modalScanProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 104);
            this.Controls.Add(this.bStopScan);
            this.Controls.Add(this.lCurrentDirectory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "modalScanProgress";
            this.ShowIcon = false;
            this.Text = "Form2";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lCurrentDirectory;
        private System.Windows.Forms.Button bStopScan;
    }
}