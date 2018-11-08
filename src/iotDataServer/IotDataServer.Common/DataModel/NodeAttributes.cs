using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IotDataServer.Common.DataModel
{
    public class NodeAttributes: IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
        readonly Dictionary<string, string> _attributeDictionary = new Dictionary<string, string>();

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

        public string[] Names => _attributeDictionary.Keys.ToArray();
        public string[] Values => _attributeDictionary.Values.ToArray();

        public int Count => _attributeDictionary.Count;
    }
}