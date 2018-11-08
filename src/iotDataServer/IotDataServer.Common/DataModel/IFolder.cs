using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace IotDataServer.Common.DataModel
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
