using Nest;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Elasticsearch.Net;
using IotDataStation.Common.DataModel;
using IotDataStation.Common.Interface;
using IotDataStation.Common.Util;
using Newtonsoft.Json.Linq;

namespace ElasticSearchListener
{
    public class ElasticSearchListener : IDataListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ElasticClient _elasticClient;
        private ElasticLowLevelClient _elasticLowLevelClient;

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
                _elasticLowLevelClient =
                    new ElasticLowLevelClient(
                        new ConnectionConfiguration(new SniffingConnectionPool(new List<Uri>() {new Uri(elkurl)})));
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
            try
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
                                INode node = addedDocument[1] as INode;
                                var jObject = node.ToJObject();
                                //_elasticClient.Index(jObject.ToString(Newtonsoft.Json.Formatting.None),
                                //    i => i.Index((string) addedDocument[0]).Id(Guid.NewGuid().ToString()));
                                _elasticLowLevelClient.Index<BytesResponse>(path, node.GroupName,
                                    PostData.String(jObject.ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

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
                path = path.Replace("/", ".");
                //////var st = newNode.ToJObject().ToString();
                // path 별로 IndexExist 를 확인하여 
                // 없으면 추가하고 
                // 있으면 큐에 담기
                if (!_elasticClient.IndexExists(path).Exists && !_indexPath.Contains(path))
                {
                    //_elasticClient.CreateIndex(path, c => c);
                    //_elasticClient.CreateIndex(path, c => c.Mappings(ms => ms.Map<JObject>(m => m.AutoMap())));
                    var response = _elasticLowLevelClient.Index<BytesResponse>(path, newNode.GroupName, PostData.String(newNode.ToJObject().ToString()));
                    _indexPath.Add(path);
                }
                else
                {
                    if (path.Equals("sensor.basic"))
                        _addDocumentQueue.Add(new object[] {path + 3, newNode});
                }
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