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
        //_modalForm.backgroundWorker1_ProgressChanged
        BackgroundWorker _bw;
        SortOrder _current_sort_order;
        ProgressChangedEventHandler _progressChangeEH;
        FolderInventory _folder_inventory;
        ResourceManager _LocRM;
        modalScanProgress _modalForm;
        long[] _measure_units = new long[] { 1024, 1024 * 1024, 1024 * 1024 * 1024 };
        long _active_measure_unit;

        public fMain()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            InitializeComponent();
            cbMeasureUnit.SelectedIndex = 0;
            _current_sort_order = SortOrder.Weight;
            rbTotalWeight.Checked = true;
            _LocRM = new ResourceManager("wfFileInventory.wfResources", typeof(fMain).Assembly);
            bSaveInventory.Enabled = false;
            _folder_inventory = new FolderInventory(this, _LocRM, tvInventory);
            _active_measure_unit = _measure_units[cbMeasureUnit.SelectedIndex];
            dlgOpenFile.Filter = _LocRM.GetString("Inventory_Files") + " (*.fin)|*.fin";
        }

        public ListBox.ObjectCollection Log { get { return lbLogs.Items; } }

        public void InitialPopulateTreeView(string path, ProgressChangedEventHandler progressChangeEH)
        {
            _folder_inventory.InitializeInventory(path);
            if (_bw == null)
            {
                _bw = new BackgroundWorker();
                _bw.WorkerReportsProgress = true;
                _bw.WorkerSupportsCancellation = true;
                _bw.ProgressChanged += progressChangeEH;
                _bw.DoWork += new DoWorkEventHandler(_folder_inventory.bw_DoWorkEventHandler);
                _bw.RunWorkerCompleted += (e, a) => FinalizeScan();
            }
            //_mainForm.Log.Clear();
            _folder_inventory.ReportProgressHandler = _bw.ReportProgress;
            _folder_inventory.UpdateDirectoryLabelHandler = UpdateDirectoryMethod;
            _bw.RunWorkerAsync();
            _modalForm.StartTimer(this);
            _modalForm.ShowDialog();

            //MessageBox.Show("I'm after showing modal form!");
        }

        private void UpdateDirectoryMethod (string path)
        {
            _modalForm.Invoke((MethodInvoker)delegate
            {
                _modalForm.UpdateDirectory(path); // runs on UI thread
            });
        }        
        private void FinalizeScan()
        {
            _modalForm.StopTimer();
            _modalForm.Close();
            _modalForm.SetMainFormTime();
            RepopulateTreeView();
            lbLogs.Items.Clear();
            lbLogs.Items.AddRange(_folder_inventory.Log.ToArray());
            bSaveInventory.Enabled = true;
        }

        public void CancelScan()
        {
            _folder_inventory.CancellationPending = true;
            _bw.CancelAsync();

        }
        // <summary>
        // Method for rebuilding TreeView by already built "virtual" Tree
        // </summary>
        public void RepopulateTreeView()
        {
            wfNode<DirInfo> internal_root = _folder_inventory.Root;
            string path = internal_root.Value.Name;
            MyTreeNode start = new MyTreeNode(path);
            tvInventory.Nodes.Clear();

            tvInventory.Nodes.Add(start);

            CopyVirtualBranch(start, internal_root);
            start.Text = internal_root.Value.ToString(_active_measure_unit, _LocRM);

        }
        // <summary>
        // Method for copying "virtual" tree branch; called recursively
        // </summary>
        private void CopyVirtualBranch(MyTreeNode start, wfNode<DirInfo> root)
        {
            start.virtualNode = root;
            if (root.Items.Count > 0)
            {
                IOrderedEnumerable<wfNode<DirInfo>> items;
                if (_current_sort_order == SortOrder.Alpha)
                {
                    items = (IOrderedEnumerable<wfNode<DirInfo>>)root.Items.OrderBy(t => t.Value.Name);
                }
                else
                {
                    items = (IOrderedEnumerable<wfNode<DirInfo>>)root.Items.OrderByDescending(t => t.Value.TotalWeight);
                }

                foreach (wfNode<DirInfo> item in items)
                {
                    MyTreeNode treenode = new MyTreeNode(item.Value.ToString(_active_measure_unit, _LocRM));
                    treenode.BackColor = item.Value.GetColorByDirInfo(treenode.ForeColor);
                    start.Nodes.Add(treenode);

                    CopyVirtualBranch(treenode, item);
                }
            }
        }


    
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
                    if (_modalForm == null) { 
                      _modalForm = new modalScanProgress();
                      _progressChangeEH = new ProgressChangedEventHandler(_modalForm.backgroundWorker1_ProgressChanged);
                    }
                    
                    InitialPopulateTreeView(_path, _progressChangeEH);
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
            if (_folder_inventory != null)
            {
                _active_measure_unit = _measure_units[cbMeasureUnit.SelectedIndex];
            }

      if (_folder_inventory != null)
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
              root.Text = root.virtualNode.Value.ToString(_active_measure_unit, _LocRM);
            }

            foreach (MyTreeNode node in root.Nodes)
            {
                UpdateAllVisibleTreeNodes(node);
            }
 
        }

        private void rbTotalWeight_Click(object sender, EventArgs e)
        {
            SortOrder prev_sort_order = _folder_inventory.current_sort_order;
            if (rbAlphabetically.Checked) {
                _folder_inventory.current_sort_order = SortOrder.Alpha;
            } else {
                _folder_inventory.current_sort_order = SortOrder.Weight;
            }
            //if (internal_root != null)
            {
                if (prev_sort_order != _current_sort_order)
                {
                    RepopulateTreeView();
                }
            }
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
            
            if (dlgOpenFile.ShowDialog() == DialogResult.OK ) {
                if (_folder_inventory.OpenInventory(dlgOpenFile.FileName))
                {
                    RepopulateTreeView();
                    bSaveInventory.Enabled = true;
                }
            }; 
        }

        private void bSaveInventory_Click(object sender, EventArgs e)
        {
            if (dlgSaveFile.ShowDialog() == DialogResult.OK) {
                _folder_inventory.SaveInventory(dlgSaveFile.FileName);
            }

        }

    }
}
