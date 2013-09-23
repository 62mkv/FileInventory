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
    /// Enumeration for sort modes
    /// </summary>
    public enum SortOrder
    {
        Alpha = 1, Weight = 2
    }

    /// <summary>
    /// A primitive class for constructing a tree of elements of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class wfNode<T>
    {
        public T Value;
        private List<wfNode<T>> items;
        private wfNode<T> parent;
        public wfNode() { items = new List<wfNode<T>>(); parent = null;}
        public wfNode(wfNode<T> _parent) : this () { parent = _parent; }
        public List<wfNode<T>> Items { get { return items; } }
        public wfNode<T> Parent { get { return parent; } } 
    }

    /// <summary>
    /// Enum for results of "read folder" operation
    /// </summary>
    public enum DirOpenResult { OK = 0, E_ACCESSDENIED = 1, E_HARDLINK = 2 };

    /// <summary>
    /// Struct to represent directory information internally
    /// </summary>
    public struct DirInfo 
    { 
        public string Name;
        public long OwnWeight;
        public long SubWeight;
        public long TotalWeight;
        public Int32 FileCount;
        public DirOpenResult Result;

        public string ToString(long active_measure_unit, ResourceManager LocRM) {
          string _total = LocRM.GetString("Title_TotalWeight");
          string _own = LocRM.GetString("Title_OwnWeight");
          return String.Format("{0} [{1}: {2:N2}, {3}: {4:N2}]", Name, _total, 
               (double) TotalWeight / active_measure_unit, _own, (double) OwnWeight / active_measure_unit);
        }

        public void InitializeDirInfo(string path)
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
            FileCount = _count;
            OwnWeight = _weight;
            Result = DirOpenResult.OK;
        }

        public Color GetColorByDirInfo(Color defaultColor)
        {
            if (Result == DirOpenResult.E_ACCESSDENIED)
            {
                return System.Drawing.Color.LightCoral;
            }
            else if (Result == DirOpenResult.E_HARDLINK)
            {
                return System.Drawing.Color.MediumTurquoise;
            }
            else
            {
                return defaultColor;
            }
        }
    }

    public class MyTreeNode : TreeNode
    {
        public wfNode<DirInfo> virtualNode;
        public MyTreeNode(string str) : base(str) { 
            virtualNode = null;   
        }
    }

    public class FolderInventory
    {
        private TreeView _treeview;
        wfNode<DirInfo> _internal_root;
        fMain _mainForm;
        modalScanProgress _modalForm;
        BackgroundWorker _bw;
        string _path;
        ResourceManager _LocRM;

        public SortOrder current_sort_order { get; set; }
        public wfNode<DirInfo> Root { get { return _internal_root; } }
        public FolderInventory(fMain mainForm, ResourceManager locRM, TreeView treeview) 
        {
            _mainForm = mainForm;
            _LocRM = locRM;
            _treeview = treeview;
        }

        public void InitialPopulateTreeView(string path)
        {
            _path = path;
            
            _internal_root = new wfNode<DirInfo>();
            _internal_root.Value.Name = _path;
            if (_modalForm == null) { _modalForm = new modalScanProgress(); }

            if (_bw == null)
            {
                _bw = new BackgroundWorker();
                _bw.WorkerReportsProgress = true;
                _bw.WorkerSupportsCancellation = true;
                _bw.ProgressChanged += new ProgressChangedEventHandler(_modalForm.backgroundWorker1_ProgressChanged);
                _bw.DoWork += new DoWorkEventHandler(bw_DoWorkEventHandler);
                _bw.RunWorkerCompleted += (e, a) => FinalizeScan();
            }
            _mainForm.Log.Clear();
            _modalForm.StartTimer(_mainForm);
            _bw.RunWorkerAsync();
            _modalForm.ShowDialog();

            //MessageBox.Show("I'm after showing modal form!");
        }

        private void FinalizeScan()
        {
            
            _modalForm.StopTimer();
            _modalForm.Close();
            _modalForm.SetMainFormTime();
        }

        public void CancelScan()
        {
            _bw.CancelAsync();
        }

        private void bw_DoWorkEventHandler(object sender, DoWorkEventArgs e)
        {
            PopulateDirectoryBranch(true, _path, _internal_root);
        }

        public bool OpenInventory(string filename)
        {
            string line;
            string full_name;
            int counter = 0;
            DirInfo di = new DirInfo();

            Regex rx = new Regex(@"(\d+)\s+(\d+)\s+(.+)", RegexOptions.Compiled);
            StreamReader file = new System.IO.StreamReader(filename, Encoding.GetEncoding(1251));
            Match match;

            _internal_root = new wfNode<DirInfo>();

            wfNode<DirInfo> root = _internal_root;

            bool first_string_processing = true;
            while ((line = file.ReadLine()) != null)
            {
                match = rx.Match(line);

                if (match.Success)
                {
                    GroupCollection groups = match.Groups;
                    di.TotalWeight = Convert.ToInt64(groups[1].ToString());
                    di.OwnWeight = Convert.ToInt64(groups[2].ToString());
                    full_name = groups[3].ToString();
                    if (first_string_processing)
                    {
                        di.Name = full_name; //dir.FullName;
                        root.Value = di;
                        
                        first_string_processing = false;
                    }
                    else
                    {

                        di.Name = GetFolderName(full_name);
                        string parent_path = GetParentPath(full_name);
                        string fp = FullPath(root);
                        if (fp != parent_path)
                        {
                            //                            lbLogs.Items.Add("Full path for root (" + fp + ") not equal to parent path:" + parent_path);
                        }
                        while (root != null && fp != parent_path)
                        {
                            root = root.Parent;
                            if (root != null) { fp = FullPath(root); }
                        }

                        if (root == null) { break; }
                        //lbLogs.Items.Add("Processing " + dir.FullName + " as a child for " + fp);

                        wfNode<DirInfo> item = new wfNode<DirInfo>(root);
                        item.Value = di;
                        root.Items.Add(item);
                        root = item;
                    }
                }
                counter++;
            }

            file.Close();
            return true;
        }

        /// <summary>
        /// Saves current inventory to a file 
        /// </summary>
        /// <param name="filename">Filename and path to save a file</param>
        public void SaveInventory(string filename)
        { 
            StreamWriter file = new StreamWriter(filename, false, Encoding.GetEncoding(1251));
            file.WriteLine(Root.Value.ToString());
            file.Close();
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
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    _mainForm.LogFolder(_LocRM.GetString("LOG_HardLink") + ": " + path); // runs on UI thread
                });
                return;
            }
            IEnumerable<String> dirs = null;
            try
            {
                dirs = di.EnumerateDirectories().Select(t => t.Name);
                root.Value.InitializeDirInfo(path);
            }
            catch (UnauthorizedAccessException e)
            {
                root.Value.Result = DirOpenResult.E_ACCESSDENIED;
                root.Value.TotalWeight = 0;
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    _mainForm.LogFolder(_LocRM.GetString("LOG_AccessDenied")+": "+path); // runs on UI thread
                });
            }
            int i = 0;
            if (dirs != null)
            {
                foreach (string dir in dirs)
                {
                    //                TreeNode node = start.Nodes.Add(dir);
                    if (_bw.CancellationPending) { break; }
                    i++;
                    _modalForm.Invoke((MethodInvoker)delegate
                    {
                        _modalForm.UpdateDirectory(path); // runs on UI thread
                    });
                    wfNode<DirInfo> item = new wfNode<DirInfo>(root);
                    Application.DoEvents();
                    item.Value.Name = dir;
                    string full_path = path + "\\" + dir;
                    root.Items.Add(item);
                    PopulateDirectoryBranch(false, full_path, item);
                    if (is_top)
                    {
                        _bw.ReportProgress(i * 100 / dirs.Count());
                    }
                    root.Value.SubWeight += item.Value.TotalWeight;
                }
                root.Value.TotalWeight = root.Value.OwnWeight + root.Value.SubWeight;
            }
        }



        private string FullPath(wfNode<DirInfo> node)
        {
            string path = "";

            while (node != null)
            {
                if (path.Length > 0)
                {
                    path = node.Value.Name + @"\" + path;
                }
                else
                {
                    path = node.Value.Name;
                }
                node = node.Parent;
            }
            if (path.Length == 2) { path += @"\"; }
            return path;
        }

        private string ValidatedFolder(string path)
        {
            string res = path;
            if (path.Length == 2 && path[1] == ':')
            {
                res = res + @"\";
            }
            return res;
        }

        private string GetFolderName(string full_name)
        {
            string[] components = full_name.Split('\\');
            return components[components.Length - 1];
        }

        private string GetParentPath(string full_name)
        {
            string[] components = full_name.Split('\\');
            components = components.Take(components.Length - 1).ToArray();
            return ValidatedFolder(String.Join(@"\", components));
        }
    }
        
}
