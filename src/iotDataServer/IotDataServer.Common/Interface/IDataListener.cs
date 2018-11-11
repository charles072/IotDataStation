using System;
using IotDataServer.Common.DataModel;

namespace IotDataServer.Common.Interface
{
    public interface IDataListener : IDisposable
    {
        void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataManager dataManager);
        void UpdatedConfig(string configFilepath);
        void UpdatedNode(string path, INode newNode, INode oldNode = null);
    }
}
