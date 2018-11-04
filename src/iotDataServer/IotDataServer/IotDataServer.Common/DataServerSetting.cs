using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using Iot.Common.ClassLogger;
using Iot.Common.DataModel;
using Iot.Common.Log;
using Iot.Common.Util;

namespace IotDataServer.Common
{
    public class DataServerSetting
    {
        private static ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();

        public const string SettingFileName = "DataServerSettings.xml";
        public bool IsTestMode { get; set; } = true;
        public int ServicePort { get; set; } = 20000;
        public LogLevel ConsoleLogLevel { get; set; } = LogLevel.Trace;
        public LogLevel FileLogLevel { get; set; } = LogLevel.Info;
        private readonly Dictionary<string, string> _settingDictionary = new Dictionary<string, string>();
        private readonly List<GetterSetting> _getterSettings = new List<GetterSetting>();

        public GetterSetting[] GetterSettings => _getterSettings.ToArray();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n\n");
            sb.Append($"IsTestMode : {IsTestMode} \n");
            sb.Append($"ServicePort : {ServicePort} \n");
            return sb.ToString();
        }
        public DataServerSetting()
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string settingsPath = Path.Combine(baseDirectory, SettingFileName);
                if (string.IsNullOrWhiteSpace(settingsPath))
                {
                    return;
                }
                XmlDocument settingXml = new XmlDocument();

                settingXml.Load(settingsPath);
                LoadSettings(settingXml.DocumentElement);
                LoadGetters(settingXml.DocumentElement);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot load setting file[{filename}].");
            }
        }

        private void LoadSettings(XmlNode rootNode)
        {
            var settingNodes = rootNode.SelectNodes("Settings/Setting");
            if (settingNodes != null)
            {
                foreach (XmlNode settingNode in settingNodes)
                {
                    string key = XmlUtils.GetXmlAttributeValue(settingNode, "key").Trim();
                    string value = XmlUtils.GetXmlAttributeValue(settingNode, "value").Trim();
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        switch (key)
                        {
                            case "isTestMode":
                                IsTestMode = StringUtils.GetValue(value, IsTestMode);
                                break;
                            case "servicePort":
                                ServicePort = StringUtils.GetValue(value, ServicePort);
                                break;
                            default:
                                SetStringValue(key, value);
                                break;
                        }
                    }
                }
            }
        }

        private void LoadGetters(XmlNode rootNode)
        {
            var getterNodes = rootNode.SelectNodes("Getters/Getter");
            if (getterNodes != null)
            {
                foreach (XmlNode getterNode in getterNodes)
                {
                    if (getterNode.Attributes != null)
                    {
                        string name = XmlUtils.GetXmlAttributeValue(getterNode, "name");
                        string config = XmlUtils.GetXmlAttributeValue(getterNode, "config", "");
                        if (string.IsNullOrWhiteSpace(config))
                        {
                            config = $"{name}.xml";
                        }
                        bool isTestMode = XmlUtils.GetXmlAttributeTypeValue(getterNode, "isTestMode", false);
                        Dictionary<string, string> settingDictionary = new Dictionary<string, string>();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Logger.Error("'name attribute of getter is empty!");
                            continue;
                        }

                        foreach (XmlAttribute attribute in getterNode.Attributes)
                        {
                            switch (attribute.Name)
                            {
                                case "name":
                                case "config":
                                case "isTestMode":
                                    break;
                                default:
                                    settingDictionary[attribute.Name] = attribute.Value;
                                    break;
                            }
                        }
                        _getterSettings.Add(new GetterSetting(name, config, isTestMode, settingDictionary));
                    }
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
    }

    public class GetterSetting
    {
        public string Name { get; set; }
        public string ConfigFile { get; set; }
        public bool IsTestMode { get; set; }

        public SimpleSettings Settings => _settings;

        private readonly SimpleSettings _settings = new SimpleSettings();

        public GetterSetting(string name, string configFile, bool isTestMode, Dictionary<string, string> settingDictionary = null)
        {
            Name = name;
            ConfigFile = configFile;
            IsTestMode = isTestMode;

            if (settingDictionary != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in settingDictionary)
                {
                    _settings[keyValuePair.Key] = keyValuePair.Value;
                }
            }
        }
    }
}
