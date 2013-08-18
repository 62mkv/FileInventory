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
            /*TreeNode root=tvInventory.Nodes.Add("Root");
            root.Nodes.Add("Child1");
             */
            try
            {

                DirectoryInfo di = new DirectoryInfo(tbFolderPath.Text);
                if (di.Exists)
                {
                    TreeNode root = tvInventory.Nodes.Add(di.FullName);
                    
                    PopulateDirectoryBranch(tbFolderPath.Text, root);
                }
            }
            catch (Exception E)
            { MessageBox.Show(LocRM.GetString("Error_PathNotFound")+": ["+tbFolderPath.Text+"]\n"+E.Message); }
        }

        private void PopulateDirectoryBranch(string path, TreeNode start)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            IEnumerable<String> dirs = di.EnumerateDirectories().OrderBy(t => t.Name).Select(t => t.Name);
            foreach (string dir in dirs)
            {
                TreeNode node = start.Nodes.Add(dir);
                PopulateDirectoryBranch(path + "\\" + dir, node);
            }
        }

    }
}
