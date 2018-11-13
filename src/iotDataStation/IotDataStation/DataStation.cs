using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grapevine.Interfaces.Server;
using IotDataStation.Common.DataModel;
using IotDataStation.Common.Interface;
using IotDataStation.HttpServer;
using IotDataStation.Interface;
using NLog;

namespace IotDataStation
{
    public class DataStation
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static object _lockObject = new object();

        public static int WebServicePort { get; protected set; } = 20000;
        public static string WebRootFolder => "WebRoot";
        public static string WebTemplateFolder => "WebTemplates";
        public static string ExtensionModuleFolder => "ExtensionModules";

        private static readonly Dictionary<string, DataReporterSetting> DataReporterSettingDictionary = new Dictionary<string, DataReporterSetting>();
        public static DataReporterSetting[] DataReporterSettings => DataReporterSettingDictionary.Values.ToArray();

        private static readonly Dictionary<string, DataListenerSetting> DataListenerSettingDictionary = new Dictionary<string, DataListenerSetting>();
        public static DataListenerSetting[] DataListenerSettings => DataListenerSettingDictionary.Values.ToArray();

        private static readonly List<Assembly> ExtensionAssemblyList = new List<Assembly>();
        public static Assembly[] ExtensionAssemblies => ExtensionAssemblyList.ToArray();
        public static IAuthentication Authentication { get; protected set; } = new NoneAuthentication();

        private static WebServer _webServer;
        private static DataRepository _dataRepository = new DataRepository();
        private static ExtensionManager _extensionManager = null;

        public static IDataRepository DataRepository => _dataRepository;

        static DataStation()
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

                    _extensionManager = new ExtensionManager(_dataRepository, ExtensionModuleFolder, DataReporterSettings, DataListenerSettings, ExtensionAssemblies);
                    _dataRepository.SetDataListeners(_extensionManager.DataListeners);
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

        public static bool SetDataReporter(string name, string configFile = "", bool isTestMode = false, SimpleSettings settings = null)
        {
            lock (_lockObject)
            {
                if (DataReporterSettingDictionary.TryGetValue(name, out var dataReporterSetting))
                {
                    Logger.Error($"'{name}' is alreay regeistered as a data getter.");
                    return false;
                }

                DataReporterSettingDictionary[name] = new DataReporterSetting(name, configFile, isTestMode, settings);
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
                ExtensionAssemblyList.Add(extensionAssembly);
            }
        }

        public bool SetAuthentication(IAuthentication authentication)
        {
            if (authentication == null)
            {
                return false;
            }

            Authentication = authentication;
            return true;
        }
    }

    public class NoneAuthentication : IAuthentication
    {
        public bool ValidateUser(IHttpContext context)
        {
            return true;
        }

        public string GetLoginPageUri(IHttpContext context)
        {
            return "/iot/";
        }
    }
}
