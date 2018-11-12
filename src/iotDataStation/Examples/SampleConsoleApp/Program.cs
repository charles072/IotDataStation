using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotDataStation;
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
            DataServer.SetDataListener("ElasticSearchListener");

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
