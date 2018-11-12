using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using IotDataStation.Common.Util;
using IotDataStation.Common.Util;
using Newtonsoft.Json.Linq;
using NLog;

namespace IotDataStation.Common.DataModel
{
    public class NodeItem : IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, string> _attributeDictionary = new Dictionary<string, string>();

        public string Name
        {
            get => GetValue("name");
            set => _attributeDictionary["name"] = value;
        }

        public string Value
        {
            get => GetValue("value");
            set => _attributeDictionary["value"] = value;
        }

        public NodeItem(string name, string value, string status = "")
        {
            Name = name;
            Value = value;

            if (!string.IsNullOrWhiteSpace(status))
            {
                _attributeDictionary["status"] = status;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _attributeDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string this[string name]
        {
            get => GetValue(name);
            set => _attributeDictionary[name] = value;
        }

        public string GetValue(string name)
        {
            TryGetValue(name, out var value);
            return value;
        }
        public bool TryGetValue(string name, out string value)
        {
            if (_attributeDictionary.TryGetValue(name, out var savedValue))
            {
                value = savedValue;
                return true;
            }
            value = "";
            return false;
        }

        public int Count => _attributeDictionary.Count;

        public static NodeItem CreateFrom(JObject nodeItemJObject)
        {
            if (nodeItemJObject == null)
            {
                return null;
            }

            NodeItem nodeItem = null;
            try
            {
                string name = JsonUtils.GetStringValue(nodeItemJObject, "name");
                string value = JsonUtils.GetStringValue(nodeItemJObject, "value");

                if (string.IsNullOrWhiteSpace(name))
                {
                    Logger.Error("Cannot create NodeItem, name is empty.");
                    return null;
                }

                nodeItem = new NodeItem(name, value);
                foreach (var property in nodeItemJObject.Properties())
                {
                    switch (property.Name)
                    {
                        case "name":
                        case "value":
                            break;
                        default:
                            nodeItem[property.Name] = property.Value.Value<string>();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "NodeItem CreateFrom(JObject nodeItemJObject):");
                nodeItem = null;
            }
            return nodeItem;
        }
    }
}