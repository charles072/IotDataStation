using IotDataServer.Common.DataModel;
using IotDataServer.Common.Interface;
using Nest;
using System;
using System.IO;
using System.Xml.Linq;
using NLog;

namespace ElasticSearchListener
{
    public class ElasticSearchListener : IDataListener
    {
        private ElasticClient _elasticClient;
        public void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings, IDataManager dataManager)
        {
            try
            {
                if (!File.Exists(configFilepath))
                    throw new FileNotFoundException($"{configFilepath} was not found");

                XDocument xDocument = XDocument.Load(configFilepath);
                var elkurl = xDocument.Element("ELKUrl")?.Value;
                _elasticClient = new ElasticClient(new Uri(elkurl));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdatedConfig(string configFilepath)
        {
            throw new NotImplementedException();
        }

        public void UpdatedNode(string path, INode newNode, INode oldNode = null)
        {
            try
            {
                var checkIndex = _elasticClient.IndexExists(new IndexExistsRequest(path));
                if (checkIndex.Exists)
                    /// TODO : api 테스트 필요
                    _elasticClient.Index(newNode, i => i.Index("path").Type("all"));
                else
                    /// TODO : api 테스트 필요
                    _elasticClient.CreateIndex(path, c => c.Mappings(ms => ms.Map<INode>(m => m.AutoMap())));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            LogManager.GetCurrentClassLogger().Info("EalsticSearchListener is disposed");
        }
    }
}
