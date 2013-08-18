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
        public fMain()
        {
            LocRM = new ResourceManager("wfFileInventory.wfResources", typeof(fMain).Assembly);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            InitializeComponent();
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
                    TreeNode root = tvInventory.Nodes.Add(di.FullName);
                    
                    PopulateTreeView(tbFolderPath.Text, root);
                }
            }
            catch (Exception E)
            { MessageBox.Show(LocRM.GetString("Error_PathNotFound")+": ["+tbFolderPath.Text+"]\n"+E.Message); }
        }

        private void PopulateTreeView(string path, TreeNode start)
        {
            wfNode<DirInfo> root = new wfNode<DirInfo>();
            PopulateDirectoryBranch(path, root);
            CopyVirtualBranch(start, root);
        }

        private void CopyVirtualBranch(TreeNode start, wfNode<DirInfo> root)
        {
            if (root.Items.Count > 0)
            {
                foreach (wfNode<DirInfo> item in root.Items.OrderBy(t=>t.Value.Name))
                {
                    TreeNode treenode = start.Nodes.Add(item.Value.Name+" ["+item.Value.TotalWeight.ToString()+"]");
                    CopyVirtualBranch(treenode, item);
                }
            }
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
    }
}
