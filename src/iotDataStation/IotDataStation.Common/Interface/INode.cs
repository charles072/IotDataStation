using System;
using System.Xml;
using IotDataStation.Common.DataModel;
using Newtonsoft.Json.Linq;

namespace IotDataStation.Common.Interface
{
    public interface INode
    {
        string ClassName { get; }
        string Path { get; set; }
        string Id { get; }
        string Name { get; }
        NodeStatus Status { get; }
        string GroupName { get; }
        DateTime UpdatedTime { get; set; }
        NodeAttributes Attributes { get; }
        NodeItems Items { get; }
        string ToXmlString();
        void WriteXml(XmlWriter xmlWriter);
        JObject ToJObject();
    }
}
