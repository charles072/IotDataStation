using System;
using System.Collections.Generic;
using NLog;
using IotDataServer;
using IotDataServer = IotDataServer.DataServer;

namespace IotDataServerConsole
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            DataServer.SetDataGetter("TestGetter");
            

            try
            {
                Logger.Info("Start ====================== ");
                DataServer.Start(31000);
                Console.Read();
                DataServer.Stop();
                Logger.Info("========================= End");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Main:");
                DataServer.Stop();
            }

            Console.Read();
        }
    }
}
