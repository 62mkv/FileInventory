using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wfFileInventory
{
    public partial class modalScanProgress : Form
    {
        public modalScanProgress()
        {
            InitializeComponent();
        }

        public void UpdateDirectory(string path)
        {
            lCurrentDirectory.Text = path;
        }
        
        public void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // The progress percentage is a property of e
            pbScanProgress.Value = e.ProgressPercentage;
        }

        public void DisplayCurrentTime(string time)
        {

            lTimer.Text = time;
        }

    }
}
