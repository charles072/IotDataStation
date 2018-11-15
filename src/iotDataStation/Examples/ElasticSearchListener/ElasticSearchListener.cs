using Nest;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using IotDataStation.Common.DataModel;
using IotDataStation.Common.Interface;
using IotDataStation.Common.Util;

namespace ElasticSearchListener
{
    public class ElasticSearchListener : IDataListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ElasticClient _elasticClient;

        private volatile List<string> _indexPath;

        //private ConcurrentList<object[]>  _addDocumentQueue;
        private ConcurrentBag<object[]> _addDocumentQueue;

        public void Initialize(string configFilepath, bool isTestMode, SimpleSettings settings,
            IDataRepository dataManager)
        {
            try
            {
                if (!File.Exists(configFilepath))
                    throw new FileNotFoundException($"{configFilepath} was not found");

                XDocument xDocument = XDocument.Load(configFilepath);
                var elkurl = xDocument.Element("ELKUrl")?.Value;
                _elasticClient = new ElasticClient(new Uri(elkurl));
                _indexPath = new List<string>();
                _addDocumentQueue = new ConcurrentBag<object[]>();
                Task.Factory.StartNew(new Action(UpdateNodeRun));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Initialize");
            }
        }

        private void UpdateNodeRun()
        {
            while (true)
            {
                while (!_addDocumentQueue.IsEmpty)
                {
                    object[] addedDocument;
                    if (_addDocumentQueue.TryPeek(out addedDocument))
                    {
                        var path = (string) addedDocument[0];
                        if (_elasticClient.IndexExists(path).Exists)
                        {
                            _addDocumentQueue.TryTake(out addedDocument);
                            _elasticClient.Index((INode) addedDocument[1],
                                i => i.Index((string) addedDocument[0]).Id(Guid.NewGuid().ToString()));
                        }
                    }
                }
            }
        }

        public void UpdatedConfig(string configFilepath)
        {
            //throw new NotImplementedException();
        }

        public void UpdatedNode(string path, INode newNode, INode oldNode = null)
        {
            try
            {
                path = path.Replace("/", "_");
                // path 별로 IndexExist 를 확인하여 
                // 없으면 추가하고 
                // 있으면 큐에 담기
                if (!_indexPath.Contains(path))
                {
                    _elasticClient.CreateIndex(path, c => c.Mappings(ms => ms.Map<INode>(m => m.AutoMap())));
                    _indexPath.Add(path);
                }
                else
                {
                    _addDocumentQueue.Add(new object[] {path, newNode});
                }

                //////if (_elasticClient.IndexExists(path).Exists)
                //////    /// TODO : api 테스트 필요
                //////    _elasticClient.Index(newNode, i => i.Index("path"));
                //////else
                //////    /// TODO : api 테스트 필요
                //////    _elasticClient.CreateIndex(path, c => c.Mappings(ms => ms.Map<INode>(m => m.AutoMap())));
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