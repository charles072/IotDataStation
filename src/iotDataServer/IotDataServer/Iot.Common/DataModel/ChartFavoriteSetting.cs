using System;
using System.Collections.Generic;
using System.Xml;
using Iot.Common.ClassLogger;
using Iot.Common.Util;
using Newtonsoft.Json.Linq;

namespace Iot.Common.DataModel
{
    public class ChartFavoriteSetting
    {
        private static readonly Iot.Common.ClassLogger.ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string AxisKind { get; private set; }
        public string[] AvailableAxisKinds { get; private set; }
        public List<ChartGroupDefinition> Groups { get; private set; }

        public ChartFavoriteSetting(string id, string name, string axisKind, string[] availableAxisKinds, List<ChartGroupDefinition> groups)
        {
            Id = id;
            Name = name;
            AxisKind = axisKind;
            AvailableAxisKinds = availableAxisKinds;
            Groups = groups;
        }

        public static ChartFavoriteSetting CreateFrom(XmlNode xmlNode)
        {
            try
            {
                string id = XmlUtils.GetXmlAttributeValue(xmlNode, "id");
                string name = XmlUtils.GetXmlAttributeValue(xmlNode, "name");
                string axisKind = XmlUtils.GetXmlAttributeValue(xmlNode, "axisKind");

                List<string> availableAxisKinds = new List<string>();
                string availableAxisKindsString = XmlUtils.GetXmlAttributeValue(xmlNode, "availableAxisKinds");
                string[] aixKinds = availableAxisKindsString.Split(',');
                foreach (var aixKind in aixKinds)
                {
                    availableAxisKinds.Add(aixKind);
                }

                List<ChartGroupDefinition> groups = new List<ChartGroupDefinition>();
                XmlNodeList groupNodes = xmlNode.SelectNodes("Groups/Group");
                if (groupNodes != null)
                {
                    foreach (XmlNode groupNode in groupNodes)
                    {
                        string groupName = XmlUtils.GetXmlAttributeValue(groupNode, "name");
                        string groupTag = XmlUtils.GetXmlAttributeValue(groupNode, "tag");
                        bool groupIsChecked = XmlUtils.GetXmlAttributeTypeValue(groupNode, "isChecked", true);
                        if (string.IsNullOrWhiteSpace(groupName))
                        {
                            Logger.Error($"groupName of {id} is empty.");
                            continue;
                        }
                        groups.Add(new ChartGroupDefinition(groupName, groupTag, groupIsChecked));
                    }
                }

                return new ChartFavoriteSetting(id, name, axisKind, availableAxisKinds.ToArray(), groups);
            }
            catch (Exception e)
            {
                Logger.Error(e, "It can't CreateFrom");
                return null;
            }
        }

        public static ChartFavoriteSetting CreateFrom(JObject chartSettingJobject)
        {
            try
            {
                string id = JsonUtils.GetStringValue(chartSettingJobject, "Id");
                string name = JsonUtils.GetStringValue(chartSettingJobject, "Name", id);
                string axisKind = JsonUtils.GetStringValue(chartSettingJobject, "AxisKind");

                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.Error("Chart Id is empty.");
                    return null;
                }

                List<string> availableAxisKinds = new List<string>();
                JArray availableAxisKindJArray = (JArray)chartSettingJobject["AvailableAxisKinds"];
                foreach (JToken jToken in availableAxisKindJArray)
                {
                    string availableAxisKind = jToken.ToString();
                    availableAxisKinds.Add(availableAxisKind);
                }

                List<ChartGroupDefinition> groups = new List<ChartGroupDefinition>();
                JArray groupJArray = JsonUtils.GetValueUsePath<JArray>(chartSettingJobject, "Groups");
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

                return new ChartFavoriteSetting(id, name, axisKind, availableAxisKinds.ToArray(), groups);
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

            List<string> availableAxisKindList = new List<string>();
            foreach (string availableAxisKind in AvailableAxisKinds)
            {
                availableAxisKindList.Add(availableAxisKind);
            }
            string availableAxisKinds = string.Join(",", availableAxisKindList.ToArray());
            xmlWriter.WriteAttributeString("availableAxisKinds", availableAxisKinds);
            
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

        public JObject ToJObject()
        {
            JObject jObject = new JObject();
            try
            {
                jObject["Id"] = Id;
                jObject["Name"] = Name;
                jObject["AxisKind"] = AxisKind.ToString();

                JArray axisKinds = new JArray();
                foreach (var axisKind in AvailableAxisKinds)
                {
                    axisKinds.Add(axisKind.ToString());
                }
                jObject["AvailableAxisKinds"] = axisKinds;
                
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
}