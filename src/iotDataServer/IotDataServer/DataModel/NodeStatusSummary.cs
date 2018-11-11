using System;
using System.Collections.Generic;
using System.Linq;
using IotDataServer.Common.DataModel;
using IotDataServer.Common.Interface;
using IotDataServer.Common.Util;
using NLog;

namespace IotDataServer.DataModel
{
    public class NodeStatusSummary : INodeStatusSummary
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string Path { get; }
        public int PathDepth { get; }
        public string Name { get; }
        public int TotalCount { get; private set; }
        public KeyValuePair<string, int>[] StatusSummaries => _nodeStatusSummaryDictionary.ToArray();

        private readonly Dictionary<string, int> _nodeStatusSummaryDictionary = new Dictionary<string, int>();

        public NodeStatusSummary(string path, KeyValuePair<string, int>[] statusSummaries = null)
        {
            Path = path;
            if (!string.IsNullOrWhiteSpace(path))
            {
                var splitPath = StringUtils.Split(path, '/');
                PathDepth = splitPath.Length;
                Name = splitPath.Last();
            }
            else
            {
                PathDepth = 0;
                Name = "_ROOT_";
            }

            if (statusSummaries == null)
            {
                var nodeStatusNames = Enum.GetNames(typeof(NodeStatus));
                foreach (string nodeStatusName in nodeStatusNames)
                {
                    _nodeStatusSummaryDictionary[nodeStatusName] = 0;
                }
            }
            else
            {
                foreach (KeyValuePair<string, int> statusSummary in statusSummaries)
                {
                    _nodeStatusSummaryDictionary[statusSummary.Key] = statusSummary.Value;
                }
            }

            TotalCount = 0;
        }

        public int ChangeStatusSummaryCount(NodeStatus nodeStatus, int count)
        {
            return ChangeStatusSummaryCount(nodeStatus.ToString(), count);
        }

        private int ChangeStatusSummaryCount(string name, int count)
        {
            TotalCount += count;
            int newCount = 0;
            if (_nodeStatusSummaryDictionary.TryGetValue(name, out int currentCount))
            {
                newCount = currentCount + count;
            }
            else
            {
                newCount = count;
            }
            _nodeStatusSummaryDictionary[name] = newCount;

            if (newCount < 0)
            {
                Logger.Error($"count of '{name}' status on '{Path}' is below zero('{newCount}').");
            }

            return newCount;
        }

        public NodeStatusSummary Clone()
        {
            NodeStatusSummary nodeStatusSummary = new NodeStatusSummary(Path, StatusSummaries);

            return nodeStatusSummary;
        }
    }
}
