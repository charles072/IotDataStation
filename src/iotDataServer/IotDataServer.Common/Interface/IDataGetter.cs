using System;
using IotDataServer.Common.DataModel;

namespace IotDataServer.Common.Interface
{
    public interface IDataGetter : IDisposable
    {
        void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataManager dataManager);
        void UpdatedConfig();
        bool Start();
        void Stop();
    }
}
