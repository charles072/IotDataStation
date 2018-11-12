using System;
using Grapevine.Server;
using NLog;

namespace IotDataStation.HttpServer
{
    public class WebServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly RestServer _restServer;

        public int Port { get; }
        public bool IsListening => _restServer.IsListening;

        public WebServer(int port = 8900, string webRoot = "WebRoot")
        {
            Port = port;
            _restServer = new RestServer
            {
                Host = "*",
                Port = port.ToString()
            };
            _restServer.PublicFolders.Add(new PublicFolder(webRoot, ""));
            _restServer.PublicFolders.Add(new PublicFolder("logs", "logs")); ;
        }

        public bool Start()
        {
            bool res = false;

            try
            {
                _restServer.Start();
                res = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Cannot start!");
            }

            Logger.Info($"WebServer on {Port}");
            return res;
        }

        public void Stop()
        {
            _restServer.Stop();
        }
    }
}