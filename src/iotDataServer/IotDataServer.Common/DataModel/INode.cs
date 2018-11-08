using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Iot.Common.DataModel;
using Newtonsoft.Json.Linq;

namespace IotDataServer.Common.DataModel
{
    public interface INode
    {
        string ClassName { get; }
        string Id { get; }
        string Name { get; }
        NodeStatus Status { get; }
        string GroupName { get; }
        DateTime UpdatedTime { get; set; }
        PinObject Pin { get; }
        NodeAttributes Attributes { get; }
        NodeItems Items { get; }
        string ToXmlString();
        void WriteXml(XmlWriter xmlWriter);
        JObject ToJObject();
    }
}
