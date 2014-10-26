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
        
        private System.Windows.Forms.Timer timer;
        private DateTime dt0;
        private TimeSpan _duration;
        fMain mainForm;
      
        
        public modalScanProgress()
        {
            InitializeComponent();
            ShowInTaskbar = false;
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

        public void SetMainFormTime()
        {
            mainForm.SetScanTime(_duration.ToString());
        }


        public void StartTimer(fMain caller)
        {
            if (timer == null)
            {
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 100;
                timer.Tick += (e, a) => TimerTick();
            }
            mainForm = caller;
            bStopScan.Enabled = true;
            dt0 = DateTime.Now;
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
            _duration = DateTime.Now - dt0;
        }

        private void TimerTick()
        {
            DateTime dt = DateTime.Now;
            DisplayCurrentTime((dt-dt0).ToString());
        }

        private void bStopScan_Click(object sender, EventArgs e)
        {
            mainForm.CancelScan();
            bStopScan.Enabled = false;
        }

    }
}
