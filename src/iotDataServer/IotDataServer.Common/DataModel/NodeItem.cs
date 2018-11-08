using System.Collections;
using System.Collections.Generic;

namespace IotDataServer.Common.DataModel
{
    public class NodeItem : IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
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
    }
}