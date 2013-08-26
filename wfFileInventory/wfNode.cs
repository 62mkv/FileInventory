using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wfFileInventory
{
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
    }

    public class MyTreeNode : TreeNode
    {
        public wfNode<DirInfo> virtualNode;
        public MyTreeNode(string str) : base(str) { 
            virtualNode = null;   
        }
    }

    public enum SortOrder
    {
        Alpha = 1, Weight = 2
    }
}
