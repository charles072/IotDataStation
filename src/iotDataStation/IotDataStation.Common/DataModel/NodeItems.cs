using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using NLog;

namespace IotDataStation.Common.DataModel
{
    public class NodeItems : IEnumerable<KeyValuePair<string, NodeItem>>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        public static NodeItems CreateFrom(JArray nodeItemsJArray)
        {
            if (nodeItemsJArray == null)
            {
                return null;
            }
            NodeItems nodeItems = null;
            try
            {
                nodeItems = new NodeItems();
                foreach (var nodeItemJObject in nodeItemsJArray)
                {
                    NodeItem nodeItem = NodeItem.CreateFrom((JObject)nodeItemJObject);
                    if (nodeItem != null)
                    {
                        nodeItems.SetItem(nodeItem);
                    }
                }
            }
            catch (Exception e)
            {
                nodeItems = null;
                Logger.Error(e, "CreateFrom(JArray nodeItemsJArray):");
            }

            return nodeItems;
        }

        public static NodeItems CreateFrom(XmlNodeList itemNodeList)
        {
            NodeItems nodeItems = null;
            try
            {
                nodeItems = new NodeItems();
                foreach (XmlNode itemNode in itemNodeList)
                {
                    NodeItem nodeItem = NodeItem.CreateFrom(itemNode);
                    if (nodeItem != null)
                    {
                        nodeItems.SetItem(nodeItem);
                    }
                }
            }
            catch (Exception e)
            {
                nodeItems = null;
                Logger.Error(e, "CreateFrom(XmlNodeList itemNodeList):");
            }

            return nodeItems;
        }
    }
}