using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IotDataStation.Common.DataModel;
using IotDataStation.Common.Interface;
using IotDataStation.Common.Util;
using IotDataStation.DataModel;
using IotDataStation.Util;
using NLog;

namespace IotDataStation
{
    public class DataRepository : IDataRepository
    {
        private IDataListener[] _dataListeners = new IDataListener[0];
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Dictionary<string, INode>> _nodesDictionary = new Dictionary<string, Dictionary<string, INode>>();
        private readonly Dictionary<string, INodeStatusSummary> _nodeStatusSummaryDictionary = new Dictionary<string, INodeStatusSummary>();

        public DataRepository()
        {
        }

        public INodeStatusSummary[] NodeStatusSummaries => _nodeStatusSummaryDictionary.Values.ToArray();

        public INode[] GetNodes(string path)
        {
            path = path.Trim().Trim('/');
            if (_nodesDictionary.TryGetValue(path, out var nodeDictionary))
            {
                return nodeDictionary.Values.ToArray();
            }
            return new INode[0];
        }

        public INode GetNode(string path, string id)
        {
            path = path.Trim().Trim('/');
            if (_nodesDictionary.TryGetValue(path, out var nodeDictionary))
            {
                if (nodeDictionary.TryGetValue(id, out var node))
                {
                    return node;
                }
            }
            return Node.CreateNullNode();
        }

        public bool SetNode(string path, INode newNode)
        {
            path = path.Trim().Trim('/');
            bool res = true;

            if (string.IsNullOrWhiteSpace(newNode?.Id))
            {
                Logger.Debug("node is empty.");
                return false;
            }
            try
            {
                Node node = Node.CreateFrom(newNode);
                if (_nodesDictionary.TryGetValue(path, out var nodeDictionary))
                {
                    if (nodeDictionary.TryGetValue(node.Id, out var foundNode))
                    {
                        nodeDictionary[node.Id] = node;
                        if (node.Status != foundNode.Status)
                        {
                            ChangedNodeStatus(path, foundNode.Status, node.Status);
                        }
                        NotifyUpdatedNode(path, node, foundNode);
                    }
                    else
                    {
                        nodeDictionary[node.Id] = node;
                        AddNewNodeStatus(path, node.Status);
                        NotifyUpdatedNode(path, node, null);
                    }
                }
                else
                {
                    nodeDictionary = new Dictionary<string, INode> {[node.Id] = node};
                    _nodesDictionary[path] = nodeDictionary;
                    AddNewNodeStatus(path, node.Status);
                    NotifyUpdatedNode(path, node, null);
                }
            }
            catch (Exception e)
            {
                res = false;
                Logger.Error(e, "SetNode");
            }

            return res;
        }

        private void AddNewNodeStatus(string path, NodeStatus newNodeStatus)
        {
            UpdateNodeStatusSummaries(path, newNodeStatus);
        }

        private void ChangedNodeStatus(string path, NodeStatus fromNodeStatus, NodeStatus toNodeStatus)
        {
            UpdateNodeStatusSummaries(path, toNodeStatus, 1, fromNodeStatus, 1);
        }

        private void UpdateNodeStatusSummaries(string path, NodeStatus newNodeStatus, int incCount = 1, NodeStatus oldNodeStatus = NodeStatus.None, int decCount = 0)
        {
            string[] splitPath = StringUtils.Split(path, '/');

            string currentPath = null;
            foreach (string pathName in splitPath)
            {
                currentPath = (currentPath == null) ? pathName : $"{currentPath}/{pathName}";

                NodeStatusSummary nodeStatusSummary = null;
                if (_nodeStatusSummaryDictionary.TryGetValue(currentPath, out var foundNodeStatusSummary))
                {
                    nodeStatusSummary = ((NodeStatusSummary)foundNodeStatusSummary).Clone();
                }

                nodeStatusSummary = nodeStatusSummary ?? new NodeStatusSummary(currentPath);

                if (decCount != 0)
                {
                    nodeStatusSummary.ChangeStatusSummaryCount(oldNodeStatus, -decCount);
                }
                nodeStatusSummary.ChangeStatusSummaryCount(newNodeStatus, incCount);

                _nodeStatusSummaryDictionary[currentPath] = nodeStatusSummary;
            }
        }

        public IFolder GetFolder(string path, int depth, bool includeNodes)
        {
            path = path.Trim().Trim('/');
            string[] splitPath = StringUtils.Split(path, '/');
            int pathDepth = splitPath.Length;

            List<INodeStatusSummary> nodeStatusSummaryList = new List<INodeStatusSummary>();
            nodeStatusSummaryList.AddRange(NodeStatusSummaries);
            nodeStatusSummaryList.Sort((x,y) =>
            {
                int res = x.PathDepth.CompareTo(y.PathDepth);
                if (res == 0)
                {
                    return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
                }

                return res;
            });

            bool isMatched = false;
            Dictionary<string, IFolder> folderDictionary = new Dictionary<string, IFolder>();

            if (pathDepth == 0) //for _ROOT_
            {
                Folder newFolder = new Folder("");
                folderDictionary[""] = newFolder;
                isMatched = true;
            }
            foreach (var nodeStatusSummary in nodeStatusSummaryList)
            {
                if (nodeStatusSummary.PathDepth == pathDepth)
                {
                    if (nodeStatusSummary.Path == path)
                    {
                        Folder newFolder = new Folder(nodeStatusSummary.Path, nodeStatusSummary.StatusSummaries);
                        if (includeNodes)
                        {
                            newFolder.ChildNodes = GetNodes(newFolder.Path);
                        }
                        folderDictionary[nodeStatusSummary.Path] = newFolder;
                        isMatched = true;
                    }
                }
                else if (nodeStatusSummary.PathDepth > pathDepth)
                {
                    if (!isMatched)
                    {
                        break;
                    }
                    if (depth > 0 && (nodeStatusSummary.PathDepth > (pathDepth + depth)))
                    {
                        break;
                    }

                    string parentPath = GetParentPath(nodeStatusSummary.Path);
                    if (folderDictionary.TryGetValue(parentPath, out var parentFolder))
                    {
                        Folder newFolder = new Folder(nodeStatusSummary.Path, nodeStatusSummary.StatusSummaries);
                        if (includeNodes && (depth < 0 || nodeStatusSummary.PathDepth < (pathDepth + depth)))
                        {
                            newFolder.ChildNodes = GetNodes(newFolder.Path);
                        }
                        parentFolder.AddChildFolder(newFolder);
                        folderDictionary[newFolder.Path] = newFolder;
                    }
                }
            }

            if (!folderDictionary.TryGetValue(path, out var folder))
            {
                folder = new Folder(path);
            }

            return folder;
        }

        private static string GetParentPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                int foundOffset = path.LastIndexOf('/');
                if (foundOffset > 0)
                {
                    return path.Substring(0, foundOffset);
                }
            }
            return "";
        }

        public void SetDataListeners(IDataListener[] extensionManagerDataListeners)
        {
            _dataListeners = extensionManagerDataListeners;
        }

        private void NotifyUpdatedNode(string path, INode newNode, INode oldNode)
        {
            foreach (IDataListener dataListener in _dataListeners)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Node fromNode = Node.CreateFrom(oldNode);
                        Node toNode = Node.CreateFrom(newNode);
                        dataListener.UpdatedNode(path, toNode, fromNode);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "NotifyUpdatedNode:");
                    }

                });
            }
        }
    }
}
