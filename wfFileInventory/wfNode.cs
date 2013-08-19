using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wfFileInventory
{
    public class wfNode<T>
    {
        public T Value;
        private List<wfNode<T>> items;
        private wfNode<T> parent;
        public wfNode() { items = new List<wfNode<T>>(); parent = null;}
        public wfNode(wfNode<T> _parent) : this () { parent = _parent; }
        public List<wfNode<T>> Items { get { return items; } }
    }

    public struct DirInfo 
    { 
        public string Name;
        public long OwnWeight;
        public long SubWeight;
        public long TotalWeight;
        public Int32 FileCount;
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
