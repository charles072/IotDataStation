using System.Collections.Generic;
using Iot.Common.DataModel;

namespace IotDataServer
{
    public class GetterSetting
    {
        public string Name { get; set; }
        public string ConfigFile { get; set; }
        public bool IsTestMode { get; set; }

        public SimpleSettings Settings => _settings;

        private readonly SimpleSettings _settings = new SimpleSettings();

        public GetterSetting(string name, string configFile, bool isTestMode, SimpleSettings settings = null)
        {
            Name = name;
            ConfigFile = configFile;
            IsTestMode = isTestMode;

            if (settings != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in settings)
                {
                    _settings[keyValuePair.Key] = keyValuePair.Value;
                }
            }
        }
    }
}