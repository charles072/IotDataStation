using System.Collections.Generic;
using IotDataStation.Common.DataModel;

namespace IotDataStation
{
    public class DataReporterSetting
    {
        public string Name { get; set; }
        public string ConfigFile { get; set; }
        public bool IsTestMode { get; set; }
        internal bool IsLoaded { get; set; } = false;

        public SimpleSettings Settings { get; }

        public DataReporterSetting(string name, string configFile = "", bool isTestMode = false, SimpleSettings settings = null)
        {
            Name = name;
            ConfigFile = configFile;
            IsTestMode = isTestMode;
            Settings = new SimpleSettings();
            if (settings != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in settings)
                {
                    Settings[keyValuePair.Key] = keyValuePair.Value;
                }
            }
        }
    }
}