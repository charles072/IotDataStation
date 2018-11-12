using System;
using System.Threading.Tasks;
using IotDataStation.Common.DataModel;
using IotDataStation.Common.Interface;
using NLog;

namespace IotDataStation.Common.DataGetter
{
    public abstract class DataReporterCore : IDataReporter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected volatile bool IsStarted = false;
        protected volatile bool ShouldStop = false;
        protected volatile bool IsTestMode = false;
        protected volatile bool IsConfigUpdated = false;
        protected string ConfigFilepath = "";
        protected SimpleSettings Settings = new SimpleSettings();
        protected IDataRepository DataRepository = null;


        public virtual void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataRepository dataRepository)
        {
            IsTestMode = isTestMode;
            ConfigFilepath = configFilepath;
            Settings = settings ?? new SimpleSettings();
            DataRepository = dataRepository;
            IsConfigUpdated = true;
        }

        public virtual void UpdatedConfig(string configFilepath)
        {
            IsConfigUpdated = true;
        }

        public virtual bool Start()
        {
            if (IsStarted)
            {
                return true;
            }
            IsStarted = true;

            Task.Factory.StartNew(DoWork);

            return true;
        }

        public virtual void Stop()
        {
            ShouldStop = true;
        }

        protected virtual void DoWork()
        {
            try
            {
                DoRun();
            }
            catch (Exception e)
            {
                Logger.Error(e, "DoRun:");
            }
            try
            {
                DoDone();
            }
            catch (Exception e)
            {
                Logger.Error(e, "DoDone:");
            }
            IsStarted = false;
            ShouldStop = false;
        }

        protected abstract void DoRun();

        protected virtual void DoDone()
        {
        }

        public virtual void Dispose()
        {
            Stop();
        }
    }
}