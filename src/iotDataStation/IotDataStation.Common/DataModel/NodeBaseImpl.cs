using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using IotDataStation.Common.Util;
using Newtonsoft.Json.Linq;
using NLog;

namespace IotDataStation.Common.DataModel
{
    public abstract class NodeBaseImpl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string _className = "";
        private string _groupName = "";

        public string ClassName
        {
            get => string.IsNullOrWhiteSpace(_className) ? this.GetType().Name: _className;
            set => _className = value;
        }
        public string GroupName
        {
            get => string.IsNullOrWhiteSpace(_groupName) ? ClassName : _groupName;
            set => _groupName = value;
        }

        public string Id { get; set;}
        public string Name { get; set; }
        public NodeStatus Status { get; set; }

        public DateTime UpdatedTime { get; set; }
        public NodePoint Point { get; set; }
        public NodeAttributes Attributes { get; }
        public NodeItems Items { get; }

        protected NodeBaseImpl(string id, string name = "", NodeStatus status = NodeStatus.None, string groupName = "", NodePoint point = null,NodeAttributes attributes = null, NodeItems items = null, DateTime? updatedTime = null)
        {
            Id = id;
            Name = string.IsNullOrWhiteSpace(name) ? id : name;
            Status = status;
            GroupName = groupName;
            UpdatedTime = updatedTime ?? CachedDateTime.Now;

            Point = point?.Clone();

            Attributes = new NodeAttributes();
            if (attributes != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in attributes)
                {
                    SetAttribute(keyValuePair.Key, keyValuePair.Value);
                }
            }

            Items = new NodeItems();
            if (items != null)
            {
                foreach (var nodeItem in items.Values)
                {
                    SetItem(nodeItem);
                }
            }
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

            Point?.WriteXml(xmlWriter);
            WriteBodyXml(xmlWriter);

            WriteFooter(xmlWriter);
        }

        private void WriteHeader(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Node");

            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("name", Name);
            xmlWriter.WriteAttributeString("class", ClassName);
            xmlWriter.WriteAttributeString("status", Status.ToString());
            xmlWriter.WriteAttributeString("group", GroupName);
            xmlWriter.WriteAttributeString("updatedTime", StringUtils.GetDateTimeSecString(UpdatedTime));

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
            foreach (var nodeItem in Items.Values)
            {
                xmlWriter.WriteStartElement("Item");
                foreach (KeyValuePair<string, string> valuePair in nodeItem)
                {
                    xmlWriter.WriteAttributeString(valuePair.Key, valuePair.Value);

                }
                xmlWriter.WriteEndElement();
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
            nodeObject["name"] = Name;
            nodeObject["class"] = ClassName;
            nodeObject["status"] = Status.ToString();
            nodeObject["group"] = GroupName;
            nodeObject["updatedTime"] = StringUtils.GetDateTimeSecString(UpdatedTime);

            foreach (KeyValuePair<string, string> keyValuePair in Attributes)
            {
                nodeObject[keyValuePair.Key] = keyValuePair.Value;
            }

            if (Point != null)
            {
                ////nodeObject["point"] = Point.ToJObject();
                nodeObject["point"] = Point.ToJArray();
            }

            if (Items.Count > 0)
            {
                JArray itemsObject= new JArray();
                foreach (NodeItem nodeItem in Items.Values)
                {
                    JObject itemObject = new JObject();
                    foreach (KeyValuePair<string, string> valuePair in nodeItem)
                    {
                        itemObject[valuePair.Key] = valuePair.Value;
                    }
                    itemsObject.Add(itemObject);
                }

                nodeObject["items"] = itemsObject;

            }

            return nodeObject;
        }

        public void SetAttribute(string name, string value)
        {
            string lowerCaseName = name.ToLower();
            switch (lowerCaseName)
            {
                case "id":
                case "name":
                case "class":
                case "status":
                case "group":
                case "updated":
                    Logger.Error($"'{name}' is a reserved word.");
                    break;
                default:
                    Attributes[name] = value;
                    break;
            }
        }

        public void SetItem(string name, string value, string status = "")
        {
            SetItem(new NodeItem(name, value, status));
        }

        public void SetItem(NodeItem nodeItem)
        {
            Items[nodeItem.Name] = nodeItem;
        }
    }
}
