using System;
using Iot.Common.DataModel;

namespace IotDataServer.Interface.Getter
{
    public interface IDataGetter : IDisposable
    {
        void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings);
        void UpdatedConfig();
        bool Start();
        void Stop();
    }
}
