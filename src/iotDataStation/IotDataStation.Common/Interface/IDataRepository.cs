using System.Collections;
using IotDataStation.Common.DataModel;

namespace IotDataStation.Common.Interface
{
    public interface IDataRepository
    {
        INode[] GetNodes(string path, bool recursive = false);
        INode GetNode(string path, string id);
        bool SetNode(string path, INode node);

        IFolder GetFolder(string path, int depth, bool includeNodes);
        INodeStatusSummary[] NodeStatusSummaries { get; }
    }
}
