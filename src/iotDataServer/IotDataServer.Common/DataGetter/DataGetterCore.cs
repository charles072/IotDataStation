using System;
using System.Threading.Tasks;
using IotDataServer.Common.DataModel;
using IotDataServer.Common.Interface;
using NLog;

namespace IotDataServer.Common.DataGetter
{
    public abstract class DataGetterCore : IDataGetter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected volatile bool IsStarted = false;
        protected volatile bool ShouldStop = false;
        protected volatile bool IsTestMode = false;
        protected volatile bool IsConfigUpdated = false;
        protected string ConfigFilepath = "";
        protected SimpleSettings Settings = new SimpleSettings();
        protected IDataManager DataManager = null;


        public virtual void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataManager dataManager)
        {
            IsTestMode = isTestMode;
            ConfigFilepath = configFilepath;
            Settings = settings ?? new SimpleSettings();
            DataManager = dataManager;
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