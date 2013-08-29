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

namespace wfFileInventory
{
    /// <summary>
    /// Main form of WinForms-application
    /// </summary>
    public partial class fMain : Form
    {
        ResourceManager LocRM;
        wfNode<DirInfo> internal_root;
        SortOrder current_sort_order;
        modalScanProgress modalForm;
        BackgroundWorker bw;
        
        string _path;
        long[] measure_units = new long[] { 1024, 1024 * 1024, 1024 * 1024 * 1024 };
        long active_measure_unit;
        public fMain()
        {
            LocRM = new ResourceManager("wfFileInventory.wfResources", typeof(fMain).Assembly);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            InitializeComponent();
            cbMeasureUnit.SelectedIndex = 0;
            current_sort_order = SortOrder.Weight;
            rbTotalWeight.Checked = true;
        }

        private void bSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbFolderPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void bStartScan_Click(object sender, EventArgs e)
        {
            
            try
            {

                DirectoryInfo di = new DirectoryInfo(tbFolderPath.Text);
                if (di.Exists)
                {
                    _path = tbFolderPath.Text;
                    InitialPopulateTreeView();
                }
            }
            catch (Exception E)
            { MessageBox.Show(LocRM.GetString("Error_PathNotFound")+": ["+tbFolderPath.Text+"]\n"+E.Message); }
        }

        // <summary>
        // Starts actual scanning in a non-UI thread
        // </summary>
        private void InitialPopulateTreeView()
        {
            tvInventory.Nodes.Clear();
            internal_root = new wfNode<DirInfo>();
            internal_root.Value.Name = _path;
            if (modalForm == null) { modalForm = new modalScanProgress(); } 
            
            if (bw == null) 
            { 
                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.ProgressChanged += new ProgressChangedEventHandler(modalForm.backgroundWorker1_ProgressChanged);
                bw.DoWork += new DoWorkEventHandler(bw_DoWorkEventHandler);
                bw.RunWorkerCompleted += (e, a) => FinalizeScan();
            }


            lbLogs.Items.Clear();
            modalForm.StartTimer(this);
            bw.RunWorkerAsync();
            modalForm.ShowDialog();
            
            //MessageBox.Show("I'm after showing modal form!");
        }

        
        // <summary>
        // Method for rebuilding TreeView by already built "virtual" Tree
        // </summary>
        private void RepopulateTreeView(string path)
        {
            MyTreeNode start = new MyTreeNode(path);
            tvInventory.Nodes.Clear();
            tvInventory.Nodes.Add(start);
            
            CopyVirtualBranch(start, internal_root);
            start.Text = DirInfoToString(internal_root.Value);
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
                if (current_sort_order == SortOrder.Alpha)
                {
                    items = (IOrderedEnumerable<wfNode<DirInfo>>)root.Items.OrderBy(t => t.Value.Name);
                }
                else
                {
                    items = (IOrderedEnumerable<wfNode<DirInfo>>)root.Items.OrderByDescending(t => t.Value.TotalWeight);
                }

                foreach (wfNode<DirInfo> item in items)
                {
                    MyTreeNode treenode = new MyTreeNode(DirInfoToString(item.Value));
                    treenode.BackColor = GetColorByDirInfo(item.Value, treenode.ForeColor);
                    start.Nodes.Add(treenode);
                    CopyVirtualBranch(treenode, item);
                }
            }
        }

        // <summary>
        // Returns directory info as text (human readable) for TreeView nodes
        // </summary>
        private string DirInfoToString(DirInfo value)
        {
            string _total = LocRM.GetString("Title_TotalWeight");
            string _own = LocRM.GetString("Title_OwnWeight");
            return String.Format("{0} [{1}: {2:N2}, {3}: {4:N2}]", value.Name, _total, (double) value.TotalWeight / active_measure_unit, _own, (double) value.OwnWeight / active_measure_unit);
        }


        // <summary>
        // Scans directory and populates virtual tree; called recursively
        // </summary>
        private void PopulateDirectoryBranch(bool is_top, string path, wfNode<DirInfo> root)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileAttributes fa = File.GetAttributes(path);
            if ((fa & FileAttributes.ReparsePoint) > 0) 
            {
                root.Value.Result = DirOpenResult.E_HARDLINK;
                root.Value.TotalWeight = 0;
                this.Invoke((MethodInvoker)delegate
                {
                    LogFolder(LocRM.GetString("LOG_HardLink") + ": " + path); // runs on UI thread
                });
                return;
            }
            IEnumerable<String> dirs = null;
            try
            {
                dirs = di.EnumerateDirectories().Select(t => t.Name);
                InitializeDirInfo(ref root.Value, path);
            }
            catch (UnauthorizedAccessException e)
            {
                root.Value.Result = DirOpenResult.E_ACCESSDENIED;
                root.Value.TotalWeight = 0;
                this.Invoke((MethodInvoker)delegate
                {
                    LogFolder(LocRM.GetString("LOG_AccessDenied")+": "+path); // runs on UI thread
                });
            }
            int i = 0;
            if (dirs != null)
            {
                foreach (string dir in dirs)
                {
                    //                TreeNode node = start.Nodes.Add(dir);
                    if (bw.CancellationPending) { break; }
                    i++;
                    this.Invoke((MethodInvoker)delegate
                    {
                        modalForm.UpdateDirectory(path); // runs on UI thread
                    });
                    wfNode<DirInfo> item = new wfNode<DirInfo>(root);
                    Application.DoEvents();
                    item.Value.Name = dir;
                    string full_path = path + "\\" + dir;
                    root.Items.Add(item);
                    PopulateDirectoryBranch(false, full_path, item);
                    if (is_top)
                    {
                        bw.ReportProgress(i * 100 / dirs.Count());
                    }
                    root.Value.SubWeight += item.Value.TotalWeight;
                }
                root.Value.TotalWeight = root.Value.OwnWeight + root.Value.SubWeight;
            }
        }

        private void InitializeDirInfo(ref DirInfo di, string path)
        {
            DirectoryInfo _dirinfo = new DirectoryInfo(path);
            IEnumerable<FileInfo> files = _dirinfo.EnumerateFiles();
            Int32 _count = 0;
            long _weight = 0;
            foreach (FileInfo file in files)
            {
                _count++;
                _weight += file.Length;
            }
            di.FileCount = _count;
            di.OwnWeight = _weight;
            di.Result = DirOpenResult.OK;
        }

        private void cbMeasureUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("Divide by "+measure_units[cbMeasureUnit.SelectedIndex]);
            active_measure_unit = measure_units[cbMeasureUnit.SelectedIndex];
            if (internal_root != null)
            {
                /*
                 * This is obsolete: now am trying to update tree visualization quickly 
                 * and without recreating it
                tvInventory.Nodes.Clear();
                RepopulateTreeView(_path);
                 */
                UpdateAllVisibleTreeNodes((MyTreeNode)tvInventory.Nodes[0]);
            }
        }

        private void UpdateAllVisibleTreeNodes(MyTreeNode root)
        {
            if (root.IsVisible)
            {
                root.Text = DirInfoToString(root.virtualNode.Value);
            }
            foreach (MyTreeNode node in root.Nodes)
            {
                UpdateAllVisibleTreeNodes(node);
            }
        }

        private void rbTotalWeight_Click(object sender, EventArgs e)
        {
            SortOrder prev_sort_order = current_sort_order;
            if (rbAlphabetically.Checked) {
                current_sort_order = SortOrder.Alpha;
            } else {
                current_sort_order = SortOrder.Weight;
            }
            if (internal_root != null)
            {
                if (prev_sort_order != current_sort_order)
                {
                    tvInventory.Nodes.Clear();
                    RepopulateTreeView(_path);
                }
            }
        }

        private void FinalizeScan()
        {
            RepopulateTreeView(_path);
            modalForm.StopTimer();
            modalForm.Close();
            lScanTime.Text = modalForm.Duration.ToString();
        }

        public void CancelScan()
        {
            bw.CancelAsync();
        }

        private void bw_DoWorkEventHandler(object sender, DoWorkEventArgs e)
        {
            PopulateDirectoryBranch(true, _path, internal_root);
        }

        private Color GetColorByDirInfo(DirInfo di, Color defaultColor) 
        {
            if (di.Result == DirOpenResult.E_ACCESSDENIED)
            {
                return System.Drawing.Color.LightCoral;
            }
            else if (di.Result == DirOpenResult.E_HARDLINK) {
                return System.Drawing.Color.MediumTurquoise;
            } else 
            {
                return defaultColor;
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

    }
}
