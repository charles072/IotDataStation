using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IotDataServer.Common.Interface;
using NLog;

namespace IotDataServer
{
    internal class ExtensionManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Type DataGetterInterfaceType = typeof(IDataGetter);
        private static readonly Type DataListenerInterfaceType = typeof(IDataListener);

        private DataGetterSetting[] _dataGetterSettings = null;
        private DataListenerSetting[] _dataListenerSettings = null;

        Dictionary<string, Type> _dataGetterTypeDictionary = new Dictionary<string, Type>();
        Dictionary<string, Type> _dataListenerTypeDictionary = new Dictionary<string, Type>();

        private Dictionary<string, IDataGetter> _dataGetterDictionary = new Dictionary<string, IDataGetter>();
        private Dictionary<string, IDataListener> _dataListenerDictionary = new Dictionary<string, IDataListener>();

        public IDataGetter[] DataGetters => _dataGetterDictionary.Values.ToArray();
        public IDataListener[] DataListeners => _dataListenerDictionary.Values.ToArray();

        private FileSystemWatcher _configFolderWatcher;
        private readonly IDataManager _dataManager;
        private readonly string _extensionsPath;

        public ExtensionManager(DataManager dataManager, string extensionModuleFolder, DataGetterSetting[] dataGetterSettings = null, DataListenerSetting[] dataListenerSettings = null, Assembly[] assemblies = null)
        {
            _dataManager = dataManager;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _extensionsPath = Path.Combine(baseDirectory, extensionModuleFolder);
            if (!Directory.Exists(_extensionsPath))
            {
                Directory.CreateDirectory(_extensionsPath);
            }

            _dataGetterSettings = dataGetterSettings ?? new DataGetterSetting[0];
            _dataListenerSettings = dataListenerSettings ?? new DataListenerSetting[0];
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
                if (assemblies != null)
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
                                Assembly assembly = Assembly.LoadFile(dllFilename);
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
                foreach (var type in assembly.GetTypes())
                {
                    if (IsDataGetter(type))
                    {
                        if (_dataGetterTypeDictionary.TryGetValue(type.Name, out var foundType))
                        {
                            Logger.Error($"'{type.Name}' as a data getter is already registered by '{type.FullName}'.");
                        }
                        else
                        {
                            _dataGetterTypeDictionary[type.Name] = type;
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

        private bool IsDataGetter(Type type)
        {
            if (!type.IsClass) return false;
            if (type.IsAbstract) return false;
            return DataGetterInterfaceType.IsAssignableFrom(type);
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
                foreach (var extensionSetting in _dataListenerSettings)
                {
                    if (_dataListenerTypeDictionary.TryGetValue(extensionSetting.Name, out Type extensionType))
                    {
                        try
                        {
                            if (Activator.CreateInstance(extensionType) is IDataListener extension)
                            {
                                _dataListenerDictionary[extensionType.Name] = extension;

                                string configFilepath = Path.Combine(_extensionsPath, extensionSetting.ConfigFile);
                                extension?.Initialize(configFilepath, extensionSetting.IsTestMode, extensionSetting.Settings, _dataManager);
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
                    else
                    {
                        Logger.Error($"Cannot find IDataListener '{extensionSetting.Name}'.");
                    }
                }
                foreach (var extensionSetting in _dataGetterSettings)
                {
                    if (_dataGetterTypeDictionary.TryGetValue(extensionSetting.Name, out Type extensionType))
                    {
                        try
                        {
                            if (Activator.CreateInstance(extensionType) is IDataGetter extension)
                            {
                                _dataGetterDictionary[extensionType.Name] = extension;
                                string configFilepath = Path.Combine(_extensionsPath, extensionSetting.ConfigFile);
                                extension?.Initialize(configFilepath, extensionSetting.IsTestMode, extensionSetting.Settings, _dataManager);
                            }
                            else
                            {
                                Logger.Error($"Cannot create instance of '{extensionType.FullName}' as a IDataGetter.");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, $"Cannot initialize '{extensionSetting.Name}' as a IDataGetter.");
                        }
                        
                    }
                    else
                    {
                        Logger.Error($"Cannot find IDataGetter '{extensionSetting.Name}'.");
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
            foreach (IDataGetter dataGetter in _dataGetterDictionary.Values)
            {
                try
                {
                    dataGetter?.Start();
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Cannot Start '{dataGetter?.GetType().FullName}'.");
                }
            }
        }

        public void Stop()
        {
            foreach (IDataGetter dataGetter in _dataGetterDictionary.Values)
            {
                try
                {
                    dataGetter?.Stop();
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Cannot Stop '{dataGetter?.GetType().FullName}'.");
                }
            }
        }
        
        private void ConfigFolderWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            string fileName = e.Name;
            string filepath = e.FullPath;
            foreach (DataGetterSetting getterSetting in _dataGetterSettings)
            {
                if (getterSetting.ConfigFile == fileName)
                {
                    if (_dataGetterDictionary.TryGetValue(getterSetting.Name, out IDataGetter dataGetter))
                    {
                        Task.Factory.StartNew(() => { dataGetter.UpdatedConfig(fileName); });

                    }
                }
            }

            foreach (DataListenerSetting dataListenerSetting in _dataListenerSettings)
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
            foreach (IDataGetter dataGetter in _dataGetterDictionary.Values)
            {
                dataGetter.Dispose();
            }
            _dataGetterDictionary.Clear();

            _dataGetterSettings = null;
            _dataListenerSettings = null;

            _dataListenerTypeDictionary = null;
            _dataGetterTypeDictionary = null;
        }
    }
}
