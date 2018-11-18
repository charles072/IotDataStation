using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IotDataStation.Common.Interface;
using IotDataStation.Util;
using NLog;

namespace IotDataStation
{
    internal class ExtensionManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Type DataReporterInterfaceType = typeof(IDataReporter);
        private static readonly Type DataListenerInterfaceType = typeof(IDataListener);

        private Dictionary<string, DataReporterSetting> _dataReporterSettingDictionary = new Dictionary<string, DataReporterSetting>();
        private Dictionary<string, DataListenerSetting> _dataListenerSettingDictionary = new Dictionary<string, DataListenerSetting>();

        Dictionary<string, Type> _dataReporterTypeDictionary = new Dictionary<string, Type>();
        Dictionary<string, Type> _dataListenerTypeDictionary = new Dictionary<string, Type>();

        private Dictionary<string, IDataReporter> _dataReporterDictionary = new Dictionary<string, IDataReporter>();
        private Dictionary<string, IDataListener> _dataListenerDictionary = new Dictionary<string, IDataListener>();

        public IDataReporter[] DataReporters => _dataReporterDictionary.Values.ToArray();
        public IDataListener[] DataListeners => _dataListenerDictionary.Values.ToArray();

        private FileSystemWatcher _configFolderWatcher;
        private readonly IDataRepository _dataRepository;
        private readonly string _extensionsPath;

        public static readonly List<Assembly> Assemblies;
        static ExtensionManager()
        {
            Assemblies = new List<Assembly>();
            foreach (
                var assembly in
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.GlobalAssemblyCache && a.GetName().Name != "IotDataStation" && !a.GlobalAssemblyCache && a.GetName().Name != "IotDataStation.Common" && !a.GlobalAssemblyCache && a.GetName().Name != "Grapevine" && !a.GetName().Name.StartsWith("vshost"))
                    .OrderBy(a => a.FullName))
            { Assemblies.Add(assembly); }
        }

        public ExtensionManager(DataRepository dataRepository, string extensionModuleFolder, DataReporterSetting[] dataReporterSettings = null, DataListenerSetting[] dataListenerSettings = null, Assembly[] assemblies = null)
        {
            _dataRepository = dataRepository;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _extensionsPath = Path.Combine(baseDirectory, extensionModuleFolder);
            if (!Directory.Exists(_extensionsPath))
            {
                Directory.CreateDirectory(_extensionsPath);
            }

            if (dataReporterSettings != null && dataReporterSettings.Length > 0)
            {
                foreach (var setting in dataReporterSettings)
                {
                    _dataReporterSettingDictionary[setting.Name] = setting;
                }
            }
            else
            {
                _dataReporterSettingDictionary["*"] = new DataReporterSetting("*")
                {
                    IsLoaded = true
                };
            }
            if (dataListenerSettings != null && dataListenerSettings.Length > 0)
            {
                foreach (var setting in dataListenerSettings)
                {
                    _dataListenerSettingDictionary[setting.Name] = setting;
                }
            }
            else
            {
                _dataListenerSettingDictionary["*"] = new DataListenerSetting("*")
                {
                    IsLoaded = true
                };
            }
            LoadExtensions(_extensionsPath, assemblies);

            InitializeExtensions();

            _configFolderWatcher = new FileSystemWatcher();
            _configFolderWatcher.Path = _extensionsPath;
            _configFolderWatcher.Changed += ConfigFolderWatcherOnChanged;
            _configFolderWatcher.EnableRaisingEvents = true;
        }

        private void LoadExtensions(string extensionsPath, Assembly[] assemblies = null)
        {
            try
            {
                if (assemblies == null || assemblies.Length == 0)
                {
                    foreach (Assembly assembly in Assemblies)
                    {
                        LoadExtensions(assembly);
                    }
                }
                else
                {
                    foreach (Assembly assembly in assemblies)
                    {
                        LoadExtensions(assembly);
                    }
                }

                if (Directory.Exists(extensionsPath))
                {
                    foreach (var file in new DirectoryInfo(extensionsPath).GetFiles())
                    {
                        string fileExtension = file.Extension.ToLower();
                        if (String.CompareOrdinal(fileExtension, ".dll") == 0)
                        {
                            try
                            {
                                string dllFilename = file.FullName;
                                Assembly assembly = Assembly.LoadFrom(dllFilename);
                                LoadExtensions(assembly);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e, $"cannot load assembly for '{file.Name}'.");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"LoadExtensions({extensionsPath}):");
            }
        }

        private void LoadExtensions(Assembly assembly)
        {
            try
            {
                foreach (var type in assembly.GetLoadableTypes())
                {
                    if (IsDataReporter(type))
                    {
                        if (_dataReporterTypeDictionary.TryGetValue(type.Name, out var foundType))
                        {
                            Logger.Error($"'{type.Name}' as a data getter is already registered by '{type.FullName}'.");
                        }
                        else
                        {
                            _dataReporterTypeDictionary[type.Name] = type;
                        }
                    }
                    if (IsDataListener(type))
                    {
                        if (_dataListenerTypeDictionary.TryGetValue(type.Name, out var foundType))
                        {
                            Logger.Error($"'{type.Name}' as a data listener is already registered '{type.FullName}'.");
                        }
                        else
                        {
                            _dataListenerTypeDictionary[type.Name] = type;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"LoadExtensions({assembly.FullName}):");
            }
        }

        private bool IsDataReporter(Type type)
        {
            if (!type.IsClass) return false;
            if (type.IsAbstract) return false;
            return DataReporterInterfaceType.IsAssignableFrom(type);
        }

        private bool IsDataListener(Type type)
        {
            if (!type.IsClass) return false;
            if (type.IsAbstract) return false;
            return DataListenerInterfaceType.IsAssignableFrom(type);
        }

        private void InitializeExtensions()
        {
            try
            {
                foreach (Type extensionType in _dataListenerTypeDictionary.Values)
                {
                    DataListenerSetting extensionSetting = null;
                    if (!_dataListenerSettingDictionary.TryGetValue(extensionType.Name, out extensionSetting))
                    {
                        if (!_dataListenerSettingDictionary.TryGetValue("*", out extensionSetting))
                        {
                            extensionSetting = null;
                        }
                    }

                    if (extensionSetting != null) { 
                        try
                        {
                            if (Activator.CreateInstance(extensionType) is IDataListener extension)
                            {
                                string configFilepath = Path.Combine(_extensionsPath, extensionSetting.ConfigFile);
                                if (string.IsNullOrWhiteSpace(extensionSetting.ConfigFile))
                                {
                                    configFilepath = Path.Combine(_extensionsPath, $"{extensionSetting.Name}.xml");
                                }
                                extension?.Initialize(configFilepath, extensionSetting.IsTestMode, extensionSetting.Settings, _dataRepository);
                                extensionSetting.IsLoaded = true;

                                _dataListenerDictionary[extensionType.Name] = extension;
                            }
                            else
                            {
                                Logger.Error($"Cannot create instance of '{extensionType.FullName}' as a IDataListener.");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, $"Cannot initialize '{extensionSetting.Name}' as a IDataListener.");
                        }
                    }
                }

                foreach (Type extensionType in _dataReporterTypeDictionary.Values)
                {
                    DataReporterSetting extensionSetting = null;
                    if (!_dataReporterSettingDictionary.TryGetValue(extensionType.Name, out extensionSetting))
                    {
                        if (!_dataReporterSettingDictionary.TryGetValue("*", out extensionSetting))
                        {
                            extensionSetting = null;
                        }
                    }

                    if (extensionSetting != null)
                    {
                        try
                        {
                            if (Activator.CreateInstance(extensionType) is IDataReporter extension)
                            {
                                _dataReporterDictionary[extensionType.Name] = extension;

                                string configFilepath = Path.Combine(_extensionsPath, extensionSetting.ConfigFile);
                                if (string.IsNullOrWhiteSpace(extensionSetting.ConfigFile))
                                {
                                    configFilepath = Path.Combine(_extensionsPath, $"{extensionType.Name}.xml");
                                }
                                extension?.Initialize(configFilepath, extensionSetting.IsTestMode, extensionSetting.Settings, _dataRepository);
                                extensionSetting.IsLoaded = true;
                            }
                            else
                            {
                                Logger.Error($"Cannot create instance of '{extensionType.FullName}' as a IDataReporter.");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, $"Cannot initialize '{extensionSetting.Name}' as a IDataReporter.");
                        }
                    }
                }
                foreach (var extensionSetting in _dataListenerSettingDictionary.Values)
                {
                    if (!extensionSetting.IsLoaded)
                    {
                        Logger.Error($"Cannot find IDataListener '{extensionSetting.Name}'.");
                    }
                }
                foreach (var extensionSetting in _dataReporterSettingDictionary.Values)
                {
                    if (!extensionSetting.IsLoaded)
                    {
                        Logger.Error($"Cannot find IDataReporter '{extensionSetting.Name}'.");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "InitializeExtensions:");
            }
        }

        public void Start()
        {
            foreach (IDataReporter dataReporter in _dataReporterDictionary.Values)
            {
                try
                {
                    dataReporter?.Start();
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Cannot Start '{dataReporter?.GetType().FullName}'.");
                }
            }
        }

        public void Stop()
        {
            foreach (IDataReporter dataReporter in _dataReporterDictionary.Values)
            {
                try
                {
                    dataReporter?.Stop();
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Cannot Stop '{dataReporter?.GetType().FullName}'.");
                }
            }
        }
        
        private void ConfigFolderWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            string fileName = e.Name;
            string filepath = e.FullPath;
            foreach (DataReporterSetting getterSetting in _dataReporterSettingDictionary.Values)
            {
                if (getterSetting.ConfigFile == fileName)
                {
                    if (_dataReporterDictionary.TryGetValue(getterSetting.Name, out IDataReporter dataReporter))
                    {
                        Task.Factory.StartNew(() => { dataReporter.UpdatedConfig(fileName); });

                    }
                }
            }

            foreach (DataListenerSetting dataListenerSetting in _dataListenerSettingDictionary.Values)
            {
                if (dataListenerSetting.ConfigFile == fileName)
                {
                    if (_dataListenerDictionary.TryGetValue(dataListenerSetting.Name, out IDataListener dataListener))
                    {
                        Task.Factory.StartNew(() => { dataListener.UpdatedConfig(fileName); });

                    }
                }
            }
        }

        public void Dispose()
        {
            _configFolderWatcher?.Dispose();
            _configFolderWatcher = null;
            foreach (IDataListener dataListener in _dataListenerDictionary.Values)
            {
                dataListener.Dispose();
            }
            _dataListenerDictionary.Clear();
            foreach (IDataReporter dataReporter in _dataReporterDictionary.Values)
            {
                dataReporter.Dispose();
            }
            _dataReporterDictionary.Clear();

            _dataReporterSettingDictionary.Clear();
            _dataListenerSettingDictionary.Clear();

            _dataListenerTypeDictionary = null;
            _dataReporterTypeDictionary = null;
        }
    }
}
