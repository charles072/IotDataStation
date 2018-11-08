using System;
using Iot.Common.DataModel;
using IotDataServer.Common.DataModel;

namespace IotDataServer.Common.Getter
{
    public interface IDataGetter : IDisposable
    {
        void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataManager dataManager);
        void UpdatedConfig();
        bool Start();
        void Stop();
    }
}
