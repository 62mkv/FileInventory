using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public Int32 OwnWeight;
        public Int32 SubWeight;
        public Int32 FileCount;
    }

}
