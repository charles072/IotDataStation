using System;
using System.IO;
using IotDataServer.HttpServer;
using NLog;

namespace IotDataServer
{
    public class DataServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DataServerSetting _setting;
        private WebServer _webServer;
        private DataGetterManager _dataGetter = null;
        private DataManager _dataManager = null;

        public DataServer(DataServerSetting setting)
        {
            _setting = setting;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            CreateDirectory(baseDirectory, _setting.WebRootFolder);
            CreateDirectory(baseDirectory, _setting.WebTemplateFolder);
            CreateDirectory(baseDirectory, _setting.NodeGetterFolder);

            TemplateManager.TemplateFolder = _setting.WebTemplateFolder;

            Init();
        }
        private string CreateDirectory(string baseDirectory, string directoryName)
        {
            string directory = Path.Combine(baseDirectory, directoryName);
            Directory.CreateDirectory(directory);

            return directory;
        }

        public void Init()
        {
            _dataManager = DataManager.Instance;
            _dataManager.Initialize(_setting);
            _webServer = new WebServer(_setting.WebServicePort, _setting.WebRootFolder);
        }



        public bool Start()
        {
            bool res = false;
            _dataGetter = new DataGetterManager(_setting.GetterSettings, _setting.NodeGetterFolder, _dataManager);
            try
            {
                _dataGetter.Start();
                _webServer.Start();
                res = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Cannot start!");
            }

            return res;
        }

        public void Stop()
        {
            _webServer.Stop();
            _dataGetter.Stop();
        }
    }
}
