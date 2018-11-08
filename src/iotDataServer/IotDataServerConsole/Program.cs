using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iot.Common.ClassLogger;
using IotDataServer;

namespace IotDataServerConsole
{
    class Program
    {
        private static ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            // Create setting in code
            //DataServerSetting setting = new DataServerSetting(3000);
            //Load setting form file.
            DataServerSetting setting = new DataServerSetting("DataServerSettings.sample.xml");
            DataServer server = new DataServer(setting);

            try
            {
                Logger.Info("Start ====================== ");
                Logger.Info(setting);
                server.Start();
                Console.Read();
                server.Stop();
                Logger.Info("========================= End");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Main:");
                server.Stop();
            }

            Console.Read();
        }
    }
}
