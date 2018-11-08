using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Iot.Common.ClassLogger;
using IotDataServer.Common.DataModel;
using IotDataServer.Common.Getter;

namespace IotDataServer
{
    public class DataGetterManager
    {
        private static ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        private static readonly Type DataGetterInterfaceType = typeof(IDataGetter);

        private GetterSetting[] _getterSettings = null;
        private readonly Dictionary<string, IDataGetter> _dataGetterNameDictionary = new Dictionary<string, IDataGetter>();

        private FileSystemWatcher _configFolderWatcher;
        private readonly IDataManager _dataManager;
        private readonly string _nodeGetterFolder;

        public DataGetterManager(GetterSetting[] settings, string nodeGetterFolder, IDataManager dataManager)
        {
            _dataManager = dataManager;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _nodeGetterFolder = Path.Combine(baseDirectory, nodeGetterFolder);

            _getterSettings = settings ?? new GetterSetting[0];
            LoadGetters();
            InitializeGetters();
            if (!Directory.Exists(_nodeGetterFolder))
            {
                Directory.CreateDirectory(_nodeGetterFolder);
            }
            _configFolderWatcher = new FileSystemWatcher();
            _configFolderWatcher.Path = _nodeGetterFolder;
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
                        string configFilepath = Path.Combine(_nodeGetterFolder, setting.ConfigFile);
                        dataGetter?.Initialize(configFilepath, setting.IsTestMode, setting.Settings, _dataManager);
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

        private void LoadGetters()
        {
            try
            {
                if (!Directory.Exists(_nodeGetterFolder))
                {
                    Directory.CreateDirectory(_nodeGetterFolder);
                }

                Dictionary<string, Type> getterTypeDictionary = new Dictionary<string, Type>();
                foreach (var file in new DirectoryInfo(_nodeGetterFolder).GetFiles())
                {
                    string fileExtension = file.Extension.ToLower();
                    if (String.CompareOrdinal(fileExtension, ".dll") == 0)
                    {
                        try
                        {
                            string dllFilename = file.FullName;
                            Assembly assembly = Assembly.LoadFile(dllFilename);
                            foreach (var type in assembly.GetTypes())
                            {
                                if (IsDataGetter(type))
                                {
                                    getterTypeDictionary[type.Name] = type;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, $"cannot load assembly for '{file.Name}'.");
                        }
                    }
                }

                foreach (var getterSetting in _getterSettings)
                {
                    if (getterTypeDictionary.TryGetValue(getterSetting.Name, out Type getterType))
                    {
                        if (Activator.CreateInstance(getterType) is IDataGetter dataGetter)
                        {
                            _dataGetterNameDictionary[getterType.Name] = dataGetter;
                        }
                        else
                        {
                            Logger.Error($"Cannot create instance of '{getterType.FullName}'.");
                        }
                    }
                    else
                    {
                        Logger.Error($"Cannot find DataGetter '{getterSetting.Name}'.");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Cannot LoadFactories.");
                throw;
            }
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
