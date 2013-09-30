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

        public string ToFileString(string parentPath)
        {
            string str = Name;
            if (parentPath != "") { str = parentPath + @"\" + Name; }
            return String.Format("{0}\t{1}\t{2}", TotalWeight, OwnWeight, str);
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

    public delegate void ReportProgressCallback(int percentProgress);

    public delegate void UpdateDirectoryLabelCallback (string path);

    public class FolderInventory
    {
        private TreeView _treeview;
        private wfNode<DirInfo> _internal_root;
        private StreamWriter _file;
        private List<string> _log;
        //private fMain _mainForm;
        //modalScanProgress _modalForm;
        public List<string> Log { get { return _log; } }
        string _path;
        ResourceManager _LocRM;
        public bool CancellationPending { get; set; }
        public ReportProgressCallback ReportProgressHandler { get; set; }
        public UpdateDirectoryLabelCallback UpdateDirectoryLabelHandler { get; set; }

        public SortOrder current_sort_order { get; set; }
        public wfNode<DirInfo> Root { get { return _internal_root; } }


        public FolderInventory(fMain mainForm, ResourceManager locRM, TreeView treeview) 
        {
            //_mainForm = mainForm;
            _LocRM = locRM;
            _treeview = treeview;
            _log = new List<string>(100);
            CancellationPending = false;
        }

        public void InitializeInventory(string path)
        {
            _path = path;
            _internal_root = new wfNode<DirInfo>();
            _internal_root.Value.Name = _path;
        }


        public void bw_DoWorkEventHandler(object sender, DoWorkEventArgs e)
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
            _file = new StreamWriter(filename, false, Encoding.GetEncoding(1251));
            WriteInventoryPath(_internal_root,"");
            _file.Close();
        }

        /// <summary>
        /// Recursive method to save directory to file
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="file"></param>
        private void WriteInventoryPath(wfNode<DirInfo> root, string parent)
        {
            _file.WriteLine(root.Value.ToFileString(parent));
            foreach (wfNode<DirInfo> node in root.Items)
            {
                WriteInventoryPath(node, (parent == "" ? "" : parent+@"\") +root.Value.Name);
            }
        }

        private void LogFolder(string str)
        {
            _log.Add(str);
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
                LogFolder(_LocRM.GetString("LOG_HardLink") + ": " + path);
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
                LogFolder(_LocRM.GetString("LOG_AccessDenied") + ": " + path); 
           }
            int i = 0;
            if (dirs != null)
            {
                foreach (string dir in dirs)
                {

                    if (CancellationPending) { break; }
                    i++;
                    if (UpdateDirectoryLabelHandler != null)
                    {
                        UpdateDirectoryLabelHandler(path);
                    }
                    wfNode<DirInfo> item = new wfNode<DirInfo>(root);
                    Application.DoEvents();
                    item.Value.Name = dir;
                    string full_path = path + "\\" + dir;
                    root.Items.Add(item);
                    PopulateDirectoryBranch(false, full_path, item);
                    if (is_top)
                    {

                        if (ReportProgressHandler != null)
                        {
                            ReportProgressHandler(i * 100 / dirs.Count());
                        }
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
