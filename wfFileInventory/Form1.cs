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
    public partial class fMain : Form
    {
        ResourceManager LocRM;
        long[] measure_units = new long[] { 1024, 1024 * 1024, 1024 * 1024 * 1024 };
        long active_measure_unit;
        wfNode<DirInfo> internal_root;
        SortOrder current_sort_order;
        string _path;
        modalScanProgress modalForm;
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
                    InitialPopulateTreeView(_path);
                }
            }
            catch (Exception E)
            { MessageBox.Show(LocRM.GetString("Error_PathNotFound")+": ["+tbFolderPath.Text+"]\n"+E.Message); }
        }

        private void InitialPopulateTreeView(string path)
        {
            tvInventory.Nodes.Clear();
            internal_root = new wfNode<DirInfo>();
            internal_root.Value.Name = path;
            BackgroundWorker bw = new BackgroundWorker();
            if (modalForm == null) { modalForm = new modalScanProgress();}

            bw.DoWork +=  ( e, a ) => PopulateDirectoryBranch(path, internal_root);
            bw.RunWorkerCompleted += (e, a) => FinalizeScan(path);
            bw.RunWorkerAsync();
            modalForm.ShowDialog();
        }

        private void RepopulateTreeView(string path)
        {
            MyTreeNode start = new MyTreeNode(path);
            tvInventory.Nodes.Add(start);
            
            CopyVirtualBranch(start, internal_root);
            start.Text = DirInfoToString(internal_root.Value);
        }

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
                    start.Nodes.Add(treenode);
                    CopyVirtualBranch(treenode, item);
                }
            }
        }

        private string DirInfoToString(DirInfo value)
        {
            string _total = LocRM.GetString("Title_TotalWeight");
            string _own = LocRM.GetString("Title_OwnWeight");
            return String.Format("{0} [{1}: {2:N2}, {3}: {4:N2}]", value.Name, _total, (double) value.TotalWeight / active_measure_unit, _own, (double) value.OwnWeight / active_measure_unit);
        }

        private void PopulateDirectoryBranch(string path, wfNode<DirInfo> root)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            IEnumerable<String> dirs = di.EnumerateDirectories().Select(t => t.Name);
            InitializeDirInfo(ref root.Value, path);
            foreach (string dir in dirs)
            {
//                TreeNode node = start.Nodes.Add(dir);
                wfNode<DirInfo> item = new wfNode<DirInfo>(root);
                Application.DoEvents();
                item.Value.Name = dir;
                string full_path = path + "\\" + dir;
                root.Items.Add(item); 
                PopulateDirectoryBranch(full_path, item);
                root.Value.SubWeight += item.Value.TotalWeight;
            }
            root.Value.TotalWeight = root.Value.OwnWeight + root.Value.SubWeight;
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

        private void FinalizeScan(string path)
        {
            RepopulateTreeView(path);
            modalForm.Close();

        }

        
    }
}
