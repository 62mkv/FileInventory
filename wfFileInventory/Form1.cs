using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;


namespace wfFileInventory
{
    /// <summary>
    /// Main form of WinForms-application
    /// </summary>
    public partial class fMain : Form
    {
        SortOrder current_sort_order;
        FolderInventory fi;
        ResourceManager _LocRM;
        long[] measure_units = new long[] { 1024, 1024 * 1024, 1024 * 1024 * 1024 };


        public fMain()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            InitializeComponent();
            cbMeasureUnit.SelectedIndex = 0;
            current_sort_order = SortOrder.Weight;
            rbTotalWeight.Checked = true;
            _LocRM = new ResourceManager("wfFileInventory.wfResources", typeof(fMain).Assembly);
            
            fi = new FolderInventory(this, _LocRM, tvInventory);
            fi.active_measure_unit = measure_units[cbMeasureUnit.SelectedIndex];
            dlgOpenFile.Filter = _LocRM.GetString("Inventory_Files") + " (*.fin)|*.fin";
        }

        public ListBox.ObjectCollection Log { get { return lbLogs.Items; } }
    

    
        private void bSelectFolder_Click(object sender, EventArgs e)
        {
            if (dlgChooseFolder.ShowDialog() == DialogResult.OK)
            {
                tbFolderPath.Text = dlgChooseFolder.SelectedPath;
            }
        }

        private void bStartScan_Click(object sender, EventArgs e)
        {
            
            try
            {

                DirectoryInfo di = new DirectoryInfo(tbFolderPath.Text);
                if (di.Exists)
                {
                    string _path = tbFolderPath.Text;
                    fi.InitialPopulateTreeView(_path);
                }
            }
            catch (Exception E)
            { MessageBox.Show(_LocRM.GetString("Error_PathNotFound")+": ["+tbFolderPath.Text+"]\n"+E.Message); }
        }

       
        public void SetScanTime(string tm)
        {
            lScanTime.Text = tm;

        }
        private void cbMeasureUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("Divide by "+measure_units[cbMeasureUnit.SelectedIndex]);
            if (fi != null)
            {
                fi.active_measure_unit = measure_units[cbMeasureUnit.SelectedIndex];
            }

      if (fi != null)
            {
                tvInventory.BeginUpdate();
                UpdateAllVisibleTreeNodes((MyTreeNode)tvInventory.TopNode);
                tvInventory.EndUpdate();
            }
        }

        private void UpdateAllVisibleTreeNodes(MyTreeNode root)
        {
            
            if (root.virtualNode != null)
            {
              root.Text = root.virtualNode.Value.ToString(fi.active_measure_unit, _LocRM);
            }

            foreach (MyTreeNode node in root.Nodes)
            {
                UpdateAllVisibleTreeNodes(node);
            }
 
        }

        private void rbTotalWeight_Click(object sender, EventArgs e)
        {
            SortOrder prev_sort_order = fi.current_sort_order;
            if (rbAlphabetically.Checked) {
                fi.current_sort_order = SortOrder.Alpha;
            } else {
                fi.current_sort_order = SortOrder.Weight;
            }
            //if (internal_root != null)
            {
                if (prev_sort_order != current_sort_order)
                {
                    fi.RepopulateTreeView();
                }
            }
        }


        public void CancelScan()
        {
            fi.CancelScan();
        }
                
        /// <summary>
        /// Adds a string to a log
        /// </summary>
        /// <param name="str"></param>
        public void LogFolder(string str)
        {
            lbLogs.Items.Add(str);
        }

        private void bFileOpen_Click(object sender, EventArgs e)
        {
            string path = "";
            if (dlgOpenFile.ShowDialog() == DialogResult.OK ) {
                if (fi.OpenInventory(dlgOpenFile.FileName))
                {
                    fi.RepopulateTreeView();
                }
            }; 
        }
       
    }
}
