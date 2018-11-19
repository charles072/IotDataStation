using System.Collections.Generic;
using System.Xml;
using IotDataStation.Common.DataModel;
using Newtonsoft.Json.Linq;

namespace IotDataStation.Common.Interface
{
    public interface IFolder
    {
        string Path { get; }
        string Name { get; }
        KeyValuePair<string, int>[] StatusSummaries { get; }
        IFolder[] ChildFolders { get; }
        INode[] ChildNodes { get; }

        void AddChildFolder(IFolder folder);

        string ToXmlString(bool includeSummary = true);
        void WriteXml(XmlWriter xmlWriter, bool includeSummary = true);
        JObject ToJObject();
    }
}
