using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Iot.Common.ClassLogger;
using IotDataServer.Common;
using IotDataServer.Interface.Getter;

namespace IotDataServer
{
    public class DataGetterManager
    {
        private static ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        private static readonly Type DataGetterInterfaceType = typeof(IDataGetter);

        private GetterSetting[] _getterSettings = null;
        private readonly Dictionary<string, IDataGetter> _dataGetterNameDictionary = new Dictionary<string, IDataGetter>();

        private FileSystemWatcher _configFolderWatcher;
        private readonly string _configFolderPath;

        public DataGetterManager(GetterSetting[] settings, Assembly getterAssembly, string configFolderPath = null)
        {
            _configFolderPath = configFolderPath;
            if (string.IsNullOrWhiteSpace(_configFolderPath))
            {
                var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
                _configFolderPath = Path.Combine(baseFolder, "GetterSettings");
            }
            _getterSettings = settings ?? new GetterSetting[0];
            LoadGetters(getterAssembly);
            InitializeGetters();
            if (!Directory.Exists(_configFolderPath))
            {
                Directory.CreateDirectory(_configFolderPath);
            }
            _configFolderWatcher = new FileSystemWatcher();
            _configFolderWatcher.Path = _configFolderPath;
            _configFolderWatcher.Changed += ConfigFolderWatcherOnChanged;
            _configFolderWatcher.EnableRaisingEvents = true;
        }


        private void InitializeGetters()
        {
            try
            {
                foreach (GetterSetting setting in _getterSettings)
                {
                    if (_dataGetterNameDictionary.TryGetValue(setting.Name, out IDataGetter dataGetter))
                    {
                        string configFilepath = Path.Combine(_configFolderPath, setting.ConfigFile);
                        dataGetter?.Initialize(configFilepath, setting.IsTestMode, setting.Settings);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "InitializeGetters:");
            }
        }
        public void Start()
        {
            foreach (IDataGetter dataGetter in _dataGetterNameDictionary.Values)
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
            foreach (IDataGetter dataGetter in _dataGetterNameDictionary.Values)
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

        private void LoadGetters(Assembly assembly)
        {
            //var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                return;
            }
            try
            {
                Dictionary<string, IDataGetter> dataGetterDictionary = new Dictionary<string, IDataGetter>();
                foreach (var getterSetting in _getterSettings)
                {
                    _dataGetterNameDictionary[getterSetting.Name] = null;
                }

                foreach (var type in assembly.GetTypes())
                {
                    if (IsDataGetter(type))
                    {
                        GetterSetting getterSetting = Find(type);
                        if (getterSetting != null)
                        {
                            if (Activator.CreateInstance(type) is IDataGetter dataGetter)
                            {
                                _dataGetterNameDictionary[type.Name] = dataGetter;
                            }
                            else
                            {
                                Logger.Error($"Cannot create instance of '{type.FullName}'.");
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, IDataGetter> keyValuePair in _dataGetterNameDictionary)
                {
                    if (keyValuePair.Value == null)
                    {
                        Logger.Error($"Cannot find DataGetter name of '{keyValuePair.Key}'.");
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error(e, $"Cannot LoadGetters.");
                throw;
            }
        }

        private GetterSetting Find(Type type)
        {
            string name = type.Name;
            foreach (var getterSetting in _getterSettings)
            {
                if (getterSetting.Name == name)
                {
                    return getterSetting;
                }
            }

            return null;
        }

        private bool IsDataGetter(Type type)
        {
            if (!type.IsClass) return false;
            if (type.IsAbstract) return false;
            return DataGetterInterfaceType.IsAssignableFrom(type);
        }

        private void ConfigFolderWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            string fileName = e.Name;
            string filepath = e.FullPath;
            foreach (GetterSetting getterSetting in _getterSettings)
            {
                if (getterSetting.ConfigFile == fileName)
                {
                    if (_dataGetterNameDictionary.TryGetValue(getterSetting.Name, out IDataGetter dataGetter))
                    {
                        Task.Factory.StartNew(() => { dataGetter.UpdatedConfig(); });

                    }
                }
            }
        }

    }
}
