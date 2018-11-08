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

            DataServerSetting setting = new DataServerSetting();
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
