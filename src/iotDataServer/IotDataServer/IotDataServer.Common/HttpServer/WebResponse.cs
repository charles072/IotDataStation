using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using Iot.Common.ClassLogger;
using Iot.Common.DataModel;
using Iot.Common.Util;
using Newtonsoft.Json.Linq;
using ContentType = Grapevine.Shared.ContentType;

namespace IotDataServer.Common.HttpServer
{
    public class WebResponse
    {
        private static readonly ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        public static bool SendNodeListResponseAsJson(IHttpContext context, Node[] nodes)
        {
            bool res = false;
            try
            {
                JArray nodeObjectArray = new JArray();

                foreach (var node in nodes)
                {
                    nodeObjectArray.Add(node.ToJObject());
                }

                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                WebServiceUtils.AddNoCacheHeader(context);
                WebServiceUtils.SetJsonHeader(context);
                context.Response.SendResponse(HttpStatusCode.Ok, nodeObjectArray.ToString(), Encoding.UTF8, ContentType.JSON);
                res = true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendNodeListResponseAsJson!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendNodeResponse(IHttpContext context, Node node)
        {
            bool res = false;
            try
            {
                using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
                {
                    using (var xmlWriter = XmlWriter.Create(sw, XmlUtils.XmlWriterSettings()))
                    {
                        node?.WriteXml(xmlWriter);
                    }
                    var xmlString = sw.ToString();

                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    WebServiceUtils.AddNoCacheHeader(context);
                    context.Response.SendResponse(HttpStatusCode.Ok, xmlString, Encoding.UTF8, ContentType.XML);
                    res = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendNodeListResponse!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendNodeListResponse(IHttpContext context, Node[] nodes, Dictionary<string, string> attributeDictionary = null)
        {
            bool res = false;
            try
            {
                using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
                {
                    using (var xmlWriter = XmlWriter.Create(sw, XmlUtils.XmlWriterSettings()))
                    {
                        xmlWriter.WriteStartElement("Nodes");
                        if (attributeDictionary != null)
                        {
                            foreach (KeyValuePair<string, string> keyValuePair in attributeDictionary)
                            {
                                xmlWriter.WriteAttributeString(keyValuePair.Key, keyValuePair.Value);
                            }
                        }

                        if (nodes != null)
                        {
                            foreach (var node in nodes)
                            {
                                node?.WriteXml(xmlWriter);
                            }
                        }


                        xmlWriter.WriteEndElement();
                    }
                    var xmlString = sw.ToString();

                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    WebServiceUtils.AddNoCacheHeader(context);
                    context.Response.SendResponse(HttpStatusCode.Ok, xmlString, Encoding.UTF8, ContentType.XML);
                    res = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendNodeListResponse!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendChartDataSetResponse(IHttpContext context, ChartDataSet chartDataSet)
        {
            bool res = false;
            try
            {
                using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
                {
                    using (var xmlWriter = XmlWriter.Create(sw, XmlUtils.XmlWriterSettings()))
                    {
                        chartDataSet?.WriteXml(xmlWriter);
                    }
                    var xmlString = sw.ToString();

                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    WebServiceUtils.AddNoCacheHeader(context);
                    context.Response.SendResponse(HttpStatusCode.Ok, xmlString, Encoding.UTF8, ContentType.XML);
                    res = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendNodeListResponse!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }
    }
}
