using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IotDataServer.Common.DataModel;
using IotDataServer.Common.Interface;
using IotDataServer.HttpServer;
using NLog;

namespace IotDataServer
{
    public class DataServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static object _lockObject = new object();

        public static int WebServicePort { get; private set; } = 20000;
        public static string WebRootFolder => "WebRoot";
        public static string WebTemplateFolder => "WebTemplates";
        public static string ExtensionModuleFolder => "ExtensionModules";

        private static readonly Dictionary<string, DataGetterSetting> DataGetterSettingDictionary = new Dictionary<string, DataGetterSetting>();
        public static DataGetterSetting[] DataGetterSettings => DataGetterSettingDictionary.Values.ToArray();

        private static readonly Dictionary<string, DataListenerSetting> DataListenerSettingDictionary = new Dictionary<string, DataListenerSetting>();
        public static DataListenerSetting[] DataListenerSettings => DataListenerSettingDictionary.Values.ToArray();

        private static readonly List<Assembly> ExtensionAssemblieList = new List<Assembly>();
        public static Assembly[] ExtensionAssemblies => ExtensionAssemblieList.ToArray();

        private static WebServer _webServer;
        private static DataManager _dataManager = new DataManager();
        private static ExtensionManager _extensionManager = null;

        public static IDataManager DataManager => _dataManager;

        static DataServer()
        {
            Init();
        }

        private static void Init()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(Path.Combine(baseDirectory, WebRootFolder));
            Directory.CreateDirectory(Path.Combine(baseDirectory, WebTemplateFolder));
            Directory.CreateDirectory(Path.Combine(baseDirectory, ExtensionModuleFolder));
        }

        public static bool IsStarted { get; private set; } = false;

        public static bool Start(int webServicePort)
        {
            bool res = false;
            lock (_lockObject)
            {
                try
                {
                    if (IsStarted)
                    {
                        Logger.Warn("IodDataServer is already running.");
                        return true;
                    }
                    WebServicePort = webServicePort;

                    _extensionManager = new ExtensionManager(_dataManager, ExtensionModuleFolder, DataGetterSettings, DataListenerSettings, ExtensionAssemblies);
                    _dataManager.SetDataListeners(_extensionManager.DataListeners);
                    TemplateManager.TemplateFolder = WebTemplateFolder;
                    _webServer = new WebServer(WebServicePort, WebRootFolder);

                    res = _webServer.Start();
                    if (res)
                    {
                        _extensionManager.Start();
                    }
                }
                catch (Exception e)
                {
                    res = false;
                    Logger.Error(e, "Start");
                }
                if (!res)
                {
                    _webServer?.Stop();
                    _webServer = null;
                    _extensionManager?.Stop();
                    _extensionManager?.Dispose();
                    _extensionManager = null;

                }

                IsStarted = res;
            }
            return res;
        }

        public static void Stop()
        {
            lock (_lockObject)
            {
                try
                {
                    if (IsStarted)
                    {
                        _webServer?.Stop();
                        _webServer = null;
                        _extensionManager?.Stop();
                        _extensionManager?.Dispose();
                        _extensionManager = null;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Stop:");
                }

                IsStarted = false;
            }
        }

        public static bool SetDataGetter(string name, string configFile = "", bool isTestMode = false, SimpleSettings settings = null)
        {
            lock (_lockObject)
            {
                if (DataGetterSettingDictionary.TryGetValue(name, out var dataGetterSetting))
                {
                    Logger.Error($"'{name}' is alreay regeistered as a data getter.");
                    return false;
                }

                DataGetterSettingDictionary[name] = new DataGetterSetting(name, configFile, isTestMode, settings);
            }

            return true;
        }

        public static bool SetDataListener(string name, string configFile = "", bool isTestMode = false, SimpleSettings settings = null)
        {
            lock (_lockObject)
            {
                if (DataListenerSettingDictionary.TryGetValue(name, out var dataListenerSetting))
                {
                    Logger.Error($"'{name}' is alreay regeistered as a data listener.");
                    return false;
                }

                DataListenerSettingDictionary[name] = new DataListenerSetting(name, configFile, isTestMode, settings);
            }

            return true;
        }

        public static void AddExtensionAssembly(Assembly extensionAssembly)
        {
            if (extensionAssembly != null)
            {
                ExtensionAssemblieList.Add(extensionAssembly);
            }
        }
    }
}
