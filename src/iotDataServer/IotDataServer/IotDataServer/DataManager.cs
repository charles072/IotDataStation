using System;
using System.Collections.Generic;
using System.Linq;
using InnoWatchApi;
using InnoWatchApi.DataModel;
using Iot.Common.ClassLogger;
using Iot.Common.DataModel;
using IotDataServer.Common;

namespace IotDataServer
{
    public class DataManager
    {
        private static ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();

        #region SingleTone
        private static readonly Lazy<DataManager> Lazy = new Lazy<DataManager>(() => new DataManager());

        public static DataManager Instance => Lazy.Value;
        #endregion

        private bool _isTestMode = false;
        private EventServiceClient _eventServiceClient = null;
        private readonly List<AlarmEvent> _alarmEventList = new List<AlarmEvent>();
        public AlarmEvent[] RecentAlarmEvents => _alarmEventList.ToArray();

        public void Initialize(DataServerSetting serverSetting)
        {
            _isTestMode = serverSetting.IsTestMode;

            string aiEventServiceBaseUrl = serverSetting.GetValue("aiEventServiceBaseUrl", "http://127.0.0.1:27000/rest/event");
            if (!string.IsNullOrWhiteSpace(aiEventServiceBaseUrl))
            {
                _eventServiceClient = new EventServiceClient(aiEventServiceBaseUrl);
            }

            if (_isTestMode)
            {
                AlarmEvent alarmEvent = new AlarmEvent("A1234", DeviceStatus.Alarm, "온도이상", "1공장 SMT3003T_1");
                _alarmEventList.Insert(0, alarmEvent);
            }
        }

    }
}
