using System;
using IotDataStation.Common.DataModel;

namespace IotDataStation.Common.Interface
{
    public interface IDataListener : IDisposable
    {
        void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataRepository dataRepository);
        void UpdatedConfig(string configFilepath);
        void UpdatedNode(string path, INode newNode, INode oldNode = null);
    }
}
