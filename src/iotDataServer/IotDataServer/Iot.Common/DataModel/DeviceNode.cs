using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Iot.Common.Util;
using Newtonsoft.Json.Linq;

namespace Iot.Common.DataModel
{
    public class DeviceNode
    {
        public string Id { get; set; }
        public PinObject Pin { get; }
        public Dictionary<string, string> Attributes { get; }
        public Dictionary<string, Dictionary<string, string>> Items { get; }
        public List<DeviceNode> ChildNodes { get; }

        public DeviceNode(string id = "", PinObject pin = null, Dictionary<string, string> attributes = null, Dictionary<string, string> items = null, List<DeviceNode> childNodes = null)
        {
            Id = id;
            Pin = pin?.Clone();

            Attributes = new Dictionary<string, string>();
            if (attributes != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in attributes)
                {
                    Attributes[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            Items = new Dictionary<string, Dictionary<string, string>>();
            if (items != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in items)
                {
                    if (Items.TryGetValue(keyValuePair.Key, out var Item))
                    {
                        Item["name"] = keyValuePair.Key;
                        Item["value"] = keyValuePair.Value;

                    }
                }
            }

            ChildNodes = new List<DeviceNode>();
            if (childNodes != null)
            {
                ChildNodes.AddRange(childNodes);
            }
        }
        public DeviceNode(string id, string group, string name = null, PinObject pin = null, Dictionary<string, string> attributes = null, Dictionary<string, string> items = null, List<DeviceNode> childNodes = null) : this (id, pin, attributes, items, childNodes)
        {
            Attributes["group"] = group;
            Attributes["name"] = name ?? id;
            Attributes["group"] = group;
        }

        public string ToXmlString()
        {
            string xmlString = "";
            try
            {
                using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
                {
                    using (var xmlWriter = XmlWriter.Create(sw, XmlUtils.XmlWriterSettings()))
                    {
                        WriteXml(xmlWriter);
                    }
                    xmlString = sw.ToString();
                }
            }
            catch (Exception)
            {
                xmlString = "";
            }
            return xmlString;
        }

        public virtual void WriteXml(XmlWriter xmlWriter)
        {
            WriteHeader(xmlWriter);

            Pin?.WriteXml(xmlWriter);
            WriteBodyXml(xmlWriter);
            WriteChildNodesXml(xmlWriter);

            WriteFooter(xmlWriter);
        }

        private void WriteHeader(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("DeviceNode");
            xmlWriter.WriteAttributeString("id", Id);

            foreach (KeyValuePair<string, string> keyValuePair in Attributes)
            {
                xmlWriter.WriteAttributeString(keyValuePair.Key, keyValuePair.Value);
            }
        }

        private void WriteBodyXml(XmlWriter xmlWriter)
        {
            if (Items.Count == 0)
            {
                return;
            }
            xmlWriter.WriteStartElement("Items");
            foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in Items)
            {
                Dictionary<string, string> itemAttributeDictionary = keyValuePair.Value;

                xmlWriter.WriteStartElement("Item");
                foreach (KeyValuePair<string, string> valuePair in itemAttributeDictionary)
                {
                    xmlWriter.WriteAttributeString(valuePair.Key, valuePair.Value);

                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        private void WriteChildNodesXml(XmlWriter xmlWriter)
        {
            if (ChildNodes.Count == 0)
            {
                return;
            }

            xmlWriter.WriteStartElement("ChildNodes");
            foreach (var childNode in ChildNodes)
            {
                childNode.WriteXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        private void WriteFooter(XmlWriter xmlWriter)
        {
            xmlWriter.WriteEndElement();
        }

        public virtual JObject ToJObject()
        {
            JObject nodeObject = new JObject();
            nodeObject["id"] = Id;

            foreach (KeyValuePair<string, string> keyValuePair in Attributes)
            {
                nodeObject[keyValuePair.Key] = keyValuePair.Value;
            }

            if (Pin != null)
            {
                nodeObject["pin"] = Pin.ToJObject();
            }

            JArray itemObjectArray = new JArray();
            foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in Items)
            {
                JObject itemObject = new JObject();
                Dictionary<string, string> itemAttributeDictionary = keyValuePair.Value;
                foreach (KeyValuePair<string, string> valuePair in itemAttributeDictionary)
                {
                    itemObject[valuePair.Key] = valuePair.Value;

                }
                itemObjectArray.Add(itemObject);
            }

            if (itemObjectArray.Count > 0)
            {
                nodeObject["items"] = itemObjectArray;
            }

            JArray childObjectArray = new JArray();
            foreach (var childNode in ChildNodes)
            {
                childObjectArray.Add(childNode.ToJObject());
            }

            if (childObjectArray.Count > 0)
            {
                nodeObject["childNodes"] = childObjectArray;
            }

            return nodeObject;
        }

        public void SetAttribute(string name, string value)
        {
            if (name == "Id")
            {
                Id = value;
                return;
            }
            Attributes[name] = value;
        }

        public void SetItem(string name, string value, string status = "")
        {
            if (Items.TryGetValue(name, out var item))
            {
                item["value"] = value;
                if (!string.IsNullOrWhiteSpace(status))
                {
                    item["status"] = status;
                }
            }
            else
            {
                Dictionary<string, string> itemValues = new Dictionary<string, string>();
                itemValues["name"] = name;
                itemValues["value"] = value;
                if (!string.IsNullOrWhiteSpace(status))
                {
                    itemValues["status"] = status;
                }

                Items[name] = itemValues;
            }
        }

        public void SetItem(string name, Dictionary<string, string> itemValueDictionary)
        {
            if (Items.TryGetValue(name, out var item))
            {
                foreach (KeyValuePair<string, string> keyValuePair in itemValueDictionary)
                {
                    item[keyValuePair.Key] = keyValuePair.Value;
                }
            }
            else
            {
                Dictionary<string, string> itemValues = new Dictionary<string, string>();
                itemValues["name"] = name;
                foreach (KeyValuePair<string, string> keyValuePair in itemValueDictionary)
                {
                    if (keyValuePair.Key == "name")
                    {
                        continue;
                    }
                    itemValues[keyValuePair.Key] = keyValuePair.Value;
                }
                Items[name] = itemValues;
            }
        }
    }
}
