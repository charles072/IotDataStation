using IotDataStation;
using System;
using NLog;

namespace SampleConsoleApp
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            DataStation.SetDataReporter("TestReporter");
            //DataStation.SetDataListener("TestGetter");
            DataStation.SetDataListener("ElasticSearchListener");

            try
            {
                Logger.Info("Start ====================== ");
                DataStation.Start(31000);
                Console.Read();
                DataStation.Stop();
                Logger.Info("========================= End");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Main:");
                DataStation.Stop();
            }

            Console.Read();
        }
    }
}
