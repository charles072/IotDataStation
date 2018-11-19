using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using IotDataStation.Common.Interface;
using IotDataStation.Common.Util;
using Newtonsoft.Json.Linq;
using NLog;

namespace IotDataStation.Common.DataModel
{
    public class Node : NodeBaseImpl, INode
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool IsNullNode => string.IsNullOrWhiteSpace(Id);

        public Node(string id = "", string name = "", NodeStatus status = NodeStatus.None, string groupName = "", NodeAttributes attributes = null, NodeItems items = null, DateTime? updatedTime = null, string className = "") : base(id, name, status, groupName, attributes, items, updatedTime)
        {
            if (!string.IsNullOrWhiteSpace(className))
            {
                ClassName = className;
            }
        }

        public static Node CreateFrom(INode iNode)
        {
            if (iNode == null)
            {
                return null;
            }
            return new Node(iNode.Id, iNode.Name, iNode.Status, iNode.GroupName, iNode.Attributes, iNode.Items, iNode.UpdatedTime, iNode.ClassName);
        }

        public static Node CreateFrom(JObject nodeObject)
        {
            Node node = null;
            if (nodeObject == null)
            {
                return node;
            }

            try
            {
                string path = "";
                string id = "";
                string name = "";
                string className = "";
                NodeStatus status = NodeStatus.None;
                string groupName = "";
                DateTime updatedTime = CachedDateTime.Now;
                NodeAttributes attributes = null;
                NodeItems items = null;


                foreach (var property in nodeObject.Properties())
                {
                    switch (property.Name)
                    {
                        case "path":
                            path = property.Value.Value<string>();
                            break;
                        case "id":
                            id = property.Value.Value<string>();
                            break;
                        case "name":
                            name = property.Value.Value<string>();
                            break;
                        case "class":
                            className = property.Value.Value<string>();
                            break;
                        case "status":
                            status = StringUtils.ToEnum(property.Value.Value<string>(), NodeStatus.None);
                            break;
                        case "group":
                            groupName = property.Value.Value<string>();
                            break;
                        case "updatedTime":
                            updatedTime = DateTime.ParseExact(property.Value.Value<string>(), "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        case "items":
                            items = NodeItems.CreateFrom((JArray)nodeObject["items"]);
                            break;
                        default:
                            if (attributes == null)
                            {
                                attributes = new NodeAttributes();
                            }
                            attributes[property.Name] = property.Value.Value<string>();
                            break;
                    }
                }
                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.Error("Cannot create Node, id is empty.");
                    return null;
                }

                node = new Node(id, name, status, groupName, attributes, items, updatedTime);
                node.Path = path;
                if (!string.IsNullOrWhiteSpace(className))
                {
                    node.ClassName = className.Trim();
                }
            }
            catch (Exception e)
            {
                node = null;
                Logger.Error(e, "CreateFrom(nodeObject):");
            }

            return node;
        }

        public static Node CreateFrom(XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                return null;
            }

            Node node = null;
            try
            {
                string path = "";
                string id = "";
                string name = "";
                string className = "";
                NodeStatus status = NodeStatus.None;
                string groupName = "";
                DateTime updatedTime = CachedDateTime.Now;
                NodeAttributes attributes = null;
                NodeItems items = null;

                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    switch (attribute.Name)
                    {
                        case "path":
                            path = attribute.Value;
                            break;
                        case "id":
                            id = attribute.Value;
                            break;
                        case "name":
                            name = attribute.Value;
                            break;
                        case "class":
                            className = attribute.Value;
                            break;
                        case "status":
                            status = StringUtils.ToEnum(attribute.Value, NodeStatus.None);
                            break;
                        case "group":
                            groupName = attribute.Value;
                            break;
                        case "updatedTime":
                            updatedTime = DateTime.ParseExact(attribute.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        default:
                            if (attributes == null)
                            {
                                attributes = new NodeAttributes();
                            }
                            attributes[attribute.Name] = attribute.Value;
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.Error("Cannot create Node, id is empty.");
                    return null;
                }

                XmlNodeList itemNodeList = xmlNode.SelectNodes("Items/Item");
                if (itemNodeList?.Count > 0)
                {
                    items = NodeItems.CreateFrom(itemNodeList);
                }

                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.Error("Cannot create Node, id is empty.");
                    return null;
                }

                node = new Node(id, name, status, groupName, attributes, items, updatedTime);
                node.Path = path;
                if (!string.IsNullOrWhiteSpace(className))
                {
                    node.ClassName = className.Trim();
                }
            }
            catch (Exception e)
            {
                node = null;
                Logger.Error(e, "CreateFrom(XmlNode xmlNode):");
            }
            return node;
        }
    }
}
