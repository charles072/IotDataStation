using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IotDataServer.Common.DataModel
{
    public class NodeItems : IEnumerable<KeyValuePair<string, NodeItem>>
    {
        readonly Dictionary<string, NodeItem> _itemDictionary = new Dictionary<string, NodeItem>();

        public IEnumerator<KeyValuePair<string, NodeItem>> GetEnumerator()
        {
            return _itemDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public NodeItem this[string name]
        {
            get => GetNodeItem(name);
            set => _itemDictionary[name] = value;
        }

        public NodeItem GetNodeItem(string name)
        {
            TryGetNodeItem(name, out var nodeItem);
            return nodeItem;
        }
        public bool TryGetNodeItem(string name, out NodeItem nodeItem)
        {
            if (_itemDictionary.TryGetValue(name, out var savedValue))
            {
                nodeItem = savedValue;
                return true;
            }
            nodeItem = null;
            return false;
        }

        public bool SetItem(string name, string value, string status = "")
        {
            return SetItem(new NodeItem(name, value, status));
        }

        public bool SetItem(NodeItem nodeItem)
        {
            _itemDictionary[nodeItem.Name] = nodeItem;
            return true;
        }
        public string[] Names => _itemDictionary.Keys.ToArray();
        public NodeItem[] Values => _itemDictionary.Values.ToArray();
        public int Count => _itemDictionary.Count;
    }
}