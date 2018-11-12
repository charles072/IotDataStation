using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using IotDataStation.Common.Util;
using NLog;

namespace IotDataStation.Common.DataModel
{
    public class SimpleSettings : IEnumerable<KeyValuePair<string, string>>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, string> _settingDictionary = new Dictionary<string, string>();

        public SimpleSettings(string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename))
                {
                    return;
                }
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string settingsPath = filename;
                if (!File.Exists(settingsPath))
                {
                    settingsPath = Path.Combine(baseDirectory, "DataGetter", filename);
                    if (!File.Exists(settingsPath))
                    {
                        settingsPath = Path.Combine(baseDirectory, filename);
                    }

                }

                if (!File.Exists(settingsPath))
                {
                    Logger.Error($"Cannot find setting file[{filename}].");
                    return;
                }
                XmlDocument settingXml = new XmlDocument();

                settingXml.Load(settingsPath);
                LoadSettings(settingXml);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot load setting file[{filename}].");
            }
        }

        public SimpleSettings()
        {
        }

        private void LoadSettings(XmlDocument settingXml)
        {
            var settingList = settingXml.SelectNodes("//Setting");

            if (settingList == null)
            {
                return;
            }

            foreach (XmlNode settingNode in settingList)
            {
                string key = XmlUtils.GetXmlAttributeValue(settingNode, "key").Trim();
                string value = XmlUtils.GetXmlAttributeValue(settingNode, "value").Trim();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetStringValue(key, value);
                }
            }
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            string foundData = GetStringValue(key);

            try
            {
                if (string.IsNullOrWhiteSpace(foundData))
                {
                    return defaultValue;
                }
                var converter = TypeDescriptor.GetConverter(typeof(T));

                return (T)converter.ConvertFromString(foundData);
            }
            catch
            {
                return defaultValue;
            }
        }

        public string GetStringValue(string key)
        {
            return _settingDictionary.ContainsKey(key) ? _settingDictionary[key] : "";
        }

        public bool Exist(string key)
        {
            return _settingDictionary.ContainsKey(key);
        }

        public void SetStringValue(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }
            _settingDictionary[key] = value;
        }

        public void Set(string name, string value)
        {
            _settingDictionary[name] = value;
        }

        public string this[string name]
        {
            get => GetStringValue(name);

            set => _settingDictionary[name] = value;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _settingDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
