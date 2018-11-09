using System.Collections.Generic;
using System.Xml;
using IotDataServer.Common.DataModel;
using Newtonsoft.Json.Linq;

namespace IotDataServer.Common.Interface
{
    public interface IFolder
    {
        string Path { get; }
        string Name { get; }
        KeyValuePair<string, int>[] StatusSummaries { get; }
        IFolder[] ChildFolders { get; }
        INode[] ChildNodes { get; }

        void AddChildFolder(IFolder folder);

        string ToXmlString();
        void WriteXml(XmlWriter xmlWriter);
        JObject ToJObject();
    }
}
