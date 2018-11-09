using System;
using System.Xml;
using IotDataServer.Common.DataModel;
using Newtonsoft.Json.Linq;

namespace IotDataServer.Common.Interface
{
    public interface INode
    {
        string ClassName { get; }
        string Id { get; }
        string Name { get; }
        NodeStatus Status { get; }
        string GroupName { get; }
        DateTime UpdatedTime { get; set; }
        NodePoint Point { get; }
        NodeAttributes Attributes { get; }
        NodeItems Items { get; }
        string ToXmlString();
        void WriteXml(XmlWriter xmlWriter);
        JObject ToJObject();
    }
}
