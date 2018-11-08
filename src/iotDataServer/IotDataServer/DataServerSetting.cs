using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Iot.Common.ClassLogger;
using Iot.Common.DataModel;
using Iot.Common.Log;
using Iot.Common.Util;

namespace IotDataServer
{
    public class DataServerSetting
    {
        private static ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();

        public const string SettingFileName = "DataServerSettings.xml";
        public int WebServicePort { get; set; } = 20000;
        public string WebRootFolder => "WebRoot";
        public string WebTemplateFolder => "Templates";
        public string NodeGetterFolder => "Getters";
        public LogLevel ConsoleLogLevel { get; set; } = LogLevel.Trace;
        public LogLevel FileLogLevel { get; set; } = LogLevel.Info;

        private readonly Dictionary<string, GetterSetting> _getterSettingDictionary = new Dictionary<string, GetterSetting>();

        public GetterSetting[] GetterSettings => _getterSettingDictionary.Values.ToArray();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            sb.Append($"webServicePort : {WebServicePort} \n");
            sb.Append($"webRootFolder : {WebRootFolder} \n");
            sb.Append($"webTemplateFolder : {WebTemplateFolder} \n");
            sb.Append($"nodeGetterFolder : {NodeGetterFolder} \n");
            return sb.ToString();
        }

        public DataServerSetting(int webServicePort, IEnumerable<GetterSetting> getterSettings = null)
        {
            WebServicePort = webServicePort;

            if (getterSettings != null)
            {
                foreach (GetterSetting getterSetting in getterSettings)
                {
                    _getterSettingDictionary[getterSetting.Name] = getterSetting;
                }
            }
        }

        public DataServerSetting(string settingFileName = null)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (string.IsNullOrWhiteSpace(settingFileName))
                {
                    settingFileName = Path.Combine(baseDirectory, SettingFileName);
                }
                else if (!File.Exists(settingFileName))
                {
                    settingFileName = Path.Combine(baseDirectory, SettingFileName);
                }

                if (File.Exists(settingFileName))
                {
                    XmlDocument settingXml = new XmlDocument();

                    settingXml.Load(settingFileName);
                    LoadSettings(settingXml.DocumentElement);
                    LoadGetters(settingXml.DocumentElement);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot load setting file[{settingFileName}].");
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
                            case "webServicePort":
                                WebServicePort = StringUtils.GetValue(value, WebServicePort);
                                break;
                            case "consoleLogLevel":
                                ConsoleLogLevel = StringUtils.GetValue(value, ConsoleLogLevel);
                                break;
                            case "fileLogLevel":
                                FileLogLevel = StringUtils.GetValue(value, FileLogLevel);
                                break;
                            default:
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
                        string dllFilename = XmlUtils.GetXmlAttributeTypeValue(getterNode, "dll", "");

                        SimpleSettings settings = new SimpleSettings();

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
                                    settings[attribute.Name] = attribute.Value;
                                    break;
                            }
                        }
                        _getterSettingDictionary[name] = new GetterSetting(name, config, isTestMode, settings);
                    }
                }
            }
        }

        public void AddGetter(string name, string configFile, bool isTestMode, string dllFileName = "", SimpleSettings settings = null)
        {
            _getterSettingDictionary[name] = new GetterSetting(name, configFile, isTestMode, settings);
        }
    }
}
