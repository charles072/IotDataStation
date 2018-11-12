using System;
using IotDataStation.Common.DataModel;

namespace IotDataStation.Common.Interface
{
    public interface IDataReporter : IDisposable
    {
        void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataRepository dataRepository);
        void UpdatedConfig(string configFilepath);
        bool Start();
        void Stop();
    }
}
