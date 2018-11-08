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
            //Load setting form file.
            //DataServerSetting setting = new DataServerSetting("DataServerSettings.sample.xml");
            // Create setting in code
            List<GetterSetting> getterSettings = new List<GetterSetting>();
            getterSettings.Add(new GetterSetting("TestGetter"));
            DataServerSetting setting = new DataServerSetting(31000, getterSettings);
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
