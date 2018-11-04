using System;
using System.Collections.Generic;
using System.Xml;
using Iot.Common.ClassLogger;
using Iot.Common.Util;
using Newtonsoft.Json.Linq;

namespace Iot.Common.DataModel
{
    public class ChartDefinition
    {
        private static readonly Iot.Common.ClassLogger.ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();

        public string Id { get; private set; }
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string AxisKind { get; private set; }
        public string AxisUnitName { get; private set; }
        public string YAxisUnitName { get; private set; }
        public string StatsName { get; private set; }
        public List<ChartFieldDefinition> Fields { get; private set; }
        public List<ChartGroupDefinition> Groups { get; set; }

        public ChartDefinition(string id, string type, string name, string axisKind, string axisUnitName, string yAxisUnitName, string statsName, List<ChartFieldDefinition> fields = null, List<ChartGroupDefinition> groups = null)
        {
            Id = id;
            Type = type;
            Name = name;
            AxisKind = axisKind;
            AxisUnitName = axisUnitName;
            YAxisUnitName = yAxisUnitName;
            StatsName = statsName;
            Fields = fields ?? new List<ChartFieldDefinition>();
            Groups = groups ?? new List<ChartGroupDefinition>();
        }

        public static ChartDefinition CreateFrom(XmlNode chartNode)
        {
            try
            {
                string id = XmlUtils.GetXmlAttributeValue(chartNode, "id");
                string type = XmlUtils.GetXmlAttributeValue(chartNode, "type");
                string name = XmlUtils.GetXmlAttributeValue(chartNode, "name", id);
                string axisKind = XmlUtils.GetXmlAttributeValue(chartNode, "axisKind");
                string axisUnitName = XmlUtils.GetXmlAttributeValue(chartNode, "axisUnitName");
                string yAxisUnitName = XmlUtils.GetXmlAttributeValue(chartNode, "yAxisUnitName");
                string statsName = XmlUtils.GetXmlAttributeValue(chartNode, "statsName");
                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.Error("Chart Id is empty.");
                    return null;
                }

                List<ChartFieldDefinition> fields = new List<ChartFieldDefinition>();
                XmlNodeList fieldNodeList = chartNode.SelectNodes("Fields/Field");
                if (fieldNodeList != null)
                {
                    foreach (XmlNode fieldNode in fieldNodeList)
                    {
                        string fieldKey = XmlUtils.GetXmlAttributeValue(fieldNode, "key");
                        string fieldName = XmlUtils.GetXmlAttributeValue(fieldNode, "name", fieldKey);
                        string fieldTag = XmlUtils.GetXmlAttributeValue(fieldNode, "tag");
                        if (string.IsNullOrWhiteSpace(fieldKey))
                        {
                            Logger.Error($"fieldKey of {id} is empty.");
                            continue;
                        }
                        fields.Add(new ChartFieldDefinition(fieldKey, fieldName, fieldTag));

                    }
                }

                return new ChartDefinition(id, type, name, axisKind, axisUnitName, yAxisUnitName, statsName, fields);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot CreateFrom xmlNode.");
                return null;
            }
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("name", Name);
            xmlWriter.WriteAttributeString("axisKind", AxisKind.ToString());
            xmlWriter.WriteAttributeString("statsName", StatsName);

            xmlWriter.WriteStartElement("Fields");
            foreach (ChartFieldDefinition field in Fields)
            {
                xmlWriter.WriteStartElement("Fields");
                xmlWriter.WriteAttributeString("key", field.Key);
                xmlWriter.WriteAttributeString("name", field.Name);
                xmlWriter.WriteAttributeString("tag", field.Tag);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Groups");
            foreach (ChartGroupDefinition group in Groups)
            {
                xmlWriter.WriteStartElement("Group");
                xmlWriter.WriteAttributeString("name", group.Name);
                xmlWriter.WriteAttributeString("tag", group.Tag);
                xmlWriter.WriteAttributeString("isChecked", group.IsChecked.ToString());
                xmlWriter.WriteEndElement();

            }
            xmlWriter.WriteEndElement();
        }
        public static ChartDefinition CreateFrom(JObject chartObject)
        {
            try
            {
                string id = JsonUtils.GetStringValue(chartObject, "Id");
                string type = JsonUtils.GetStringValue(chartObject, "Type");
                string name = JsonUtils.GetStringValue(chartObject, "Name", id);
                string axisKind = JsonUtils.GetStringValue(chartObject, "AxisKind");
                string axisUnitName = JsonUtils.GetStringValue(chartObject, "AxisUnitName");
                string yAxisUnitName = JsonUtils.GetStringValue(chartObject, "YAxisUnitName");
                string statsName = JsonUtils.GetStringValue(chartObject, "StatsName");
                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.Error("Chart Id is empty.");
                    return null;
                }

                List<ChartFieldDefinition> fields = new List<ChartFieldDefinition>();
                JArray fieldJArray = JsonUtils.GetValueUsePath<JArray>(chartObject, "Fields");
                if (fieldJArray != null)
                {
                    foreach (JToken fieldToken in fieldJArray)
                    {
                        JObject fieldObject = fieldToken.Value<JObject>();
                        string fieldKey = JsonUtils.GetStringValue(fieldObject, "Key");
                        string fieldName = JsonUtils.GetStringValue(fieldObject, "Name");
                        string fieldTag = JsonUtils.GetStringValue(fieldObject, "Tag");
                        if (string.IsNullOrWhiteSpace(fieldKey))
                        {
                            Logger.Error($"fieldKey of {id} is empty.");
                            continue;
                        }
                        fields.Add(new ChartFieldDefinition(fieldKey, fieldName, fieldTag));
                    }
                }

                List<ChartGroupDefinition> groups = new List<ChartGroupDefinition>();
                JArray groupJArray = JsonUtils.GetValueUsePath<JArray>(chartObject, "Groups");
                if (groupJArray != null)
                {
                    foreach (JToken groupToken in groupJArray)
                    {
                        JObject groupObject = groupToken.Value<JObject>();
                        string groupName = JsonUtils.GetStringValue(groupObject, "Name");
                        string groupTag = JsonUtils.GetStringValue(groupObject, "Tag");
                        bool groupIsChecked = JsonUtils.GetBoolValue(groupObject, "IsChecked", true);
                        if (string.IsNullOrWhiteSpace(groupName))
                        {
                            Logger.Error($"groupName of {groupName} is empty.");
                            continue;
                        }
                        groups.Add(new ChartGroupDefinition(groupName, groupTag, groupIsChecked));
                    }
                }

                return new ChartDefinition(id, type, name, axisKind, axisUnitName, yAxisUnitName, statsName, fields, groups);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot CreateFrom xmlNode.");
                return null;
            }
        }

        public JObject ToJObject()
        {
            JObject jObject = new JObject();
            try
            {
                jObject["Id"] = Id;
                jObject["Type"] = Type;
                jObject["Name"] = Name;
                jObject["AxisKind"] = AxisKind;
                jObject["AxisUnitName"] = AxisUnitName;
                jObject["YAxisUnitName"] = YAxisUnitName;
                jObject["StatsName"] = StatsName;

                JArray fields = new JArray();
                foreach (ChartFieldDefinition field in Fields)
                {
                    JToken jToken = new JObject();
                    jToken["Name"] = field.Name;
                    jToken["Key"] = field.Key;
                    jToken["Tag"] = field.Tag;
                    fields.Add(jToken);
                }
                jObject["Fields"] = fields;

                JArray groups = new JArray();
                foreach (ChartGroupDefinition group in Groups)
                {
                    JToken jToken = new JObject();
                    jToken["Name"] = group.Name;
                    jToken["Tag"] = group.Tag;
                    jToken["IsChecked"] = group.IsChecked;
                    groups.Add(jToken);
                }
                jObject["Groups"] = groups;
            }
            catch (Exception e)
            {
                Logger.Error(e, "It can't ToJObject");
            }

            return jObject;
        }
    }

    public class ChartFieldDefinition
    {
        public ChartFieldDefinition(string key, string name, string tag= "")
        {
            Key = key;
            Name = name;
            Tag = tag;
        }

        public string Key { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
    }

    public class ChartGroupDefinition
    {
        private static readonly Iot.Common.ClassLogger.ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        public bool IsChecked { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }

        public ChartGroupDefinition(string name, string tag="", bool isChecked = true)
        {
            Name = name;
            Tag = tag;
            IsChecked = isChecked;
        }
    }
}