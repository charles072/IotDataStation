using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using IotDataStation.Common.DataModel;
using IotDataStation.Common.Interface;
using IotDataStation.Common.Util;
using IotDataStation.Util;
using Newtonsoft.Json.Linq;

namespace IotDataStation.DataModel
{
    public class Folder : NodeStatusSummary, IFolder
    {
        private readonly List<IFolder> _childFolders = new List<IFolder>();
        private INode[] _childNodes;

        public IFolder[] ChildFolders => _childFolders.ToArray();

        public INode[] ChildNodes
        {
            get => _childNodes ?? new INode[0];
            set => _childNodes = value;
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

            WriteStatusSummaryXml(xmlWriter);
            WriteChildFoldersXml(xmlWriter);
            WriteChildNodesXml(xmlWriter);
            WriteFooter(xmlWriter);
        }

        private void WriteHeader(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Folder");

            xmlWriter.WriteAttributeString("path", Path);
            xmlWriter.WriteAttributeString("name", Name);
        }

        private void WriteStatusSummaryXml(XmlWriter xmlWriter)
        {
            var statusSummaries = StatusSummaries;
            if (statusSummaries.Length == 0)
            {
                return;
            }

            xmlWriter.WriteStartElement("StatusSummary");
            foreach (var statusSummary in statusSummaries)
            {
                xmlWriter.WriteStartElement("Status");

                xmlWriter.WriteAttributeString("name", statusSummary.Key);
                xmlWriter.WriteAttributeString("count", statusSummary.Value.ToString());

                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        private void WriteChildFoldersXml(XmlWriter xmlWriter)
        {
            var childFolders = ChildFolders;
            if (childFolders.Length == 0)
            {
                return;
            }

            xmlWriter.WriteStartElement("Folders");
            foreach (var childFolder in childFolders)
            {
                childFolder.WriteXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }
        private void WriteChildNodesXml(XmlWriter xmlWriter)
        {
            var childNodes = ChildNodes;
            if (childNodes.Length == 0)
            {
                return;
            }

            xmlWriter.WriteStartElement("Nodes");
            foreach (var childNode in childNodes)
            {
                childNode.WriteXml(xmlWriter);
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
            nodeObject["id"] = Path;
            nodeObject["name"] = Name;
            nodeObject["class"] = "Folder";
            nodeObject["path"] = Path;

            var statusSummaries = StatusSummaries;
            if (statusSummaries.Length > 0)
            {
                JArray statusSummariesObject = new JArray();
                foreach (var statusSummary in statusSummaries)
                {
                    JObject statusSummaryObject = new JObject();
                    statusSummaryObject["name"] = statusSummary.Key;
                    statusSummaryObject["value"] = statusSummary.Value;
                    statusSummariesObject.Add(statusSummaryObject);
                }

                nodeObject["statusSummaries"] = statusSummariesObject;

            }

            var childFolders = ChildFolders;
            if (childFolders.Length > 0)
            {
                JArray childFoldersObject = new JArray();
                foreach (var childFolder in childFolders)
                {
                    childFoldersObject.Add(childFolder.ToJObject());
                }
                nodeObject["childFolders"] = childFoldersObject;
            }

            var childNodes = ChildNodes;
            if (childNodes.Length > 0)
            {
                JArray childNodesObject = new JArray();
                foreach (var childNode in childNodes)
                {
                    childNodesObject.Add(childNode.ToJObject());
                }
                nodeObject["childNodes"] = childNodesObject;
            }

            return nodeObject;
        }

        public Folder(string path, KeyValuePair<string, int>[] statusSummaries = null) : base(path, statusSummaries)
        {
        }

        public void AddChildFolder(IFolder folder)
        {
            _childFolders.Add(folder);
        }

        
    }
}
