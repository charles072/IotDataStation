using IotDataServer.Common.DataModel;

namespace IotDataServer.Common.Interface
{
    public interface IDataManager
    {
        INode[] GetNodes(string path);
        INode GetNode(string path, string id);
        bool SetNode(string path, INode node);

        IFolder GetFolder(string path, int depth, bool includeNodes);
    }
}
