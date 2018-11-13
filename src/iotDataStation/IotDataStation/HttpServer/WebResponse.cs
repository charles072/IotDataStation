using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using NLog;
using IotDataStation.Common.Interface;
using IotDataStation.Common.Util;
using IotDataStation.Util;
using IotDataStation.WebService;
using Newtonsoft.Json.Linq;
using ContentType = Grapevine.Shared.ContentType;

namespace IotDataStation.HttpServer
{
    public class WebResponse
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool SendFolderResponseAsJson(IHttpContext context, IFolder folder)
        {
            bool res = false;
            try
            {
                if (folder == null)
                {
                    context.Response.SendResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    WebServiceUtils.AddNoCacheHeader(context);
                    WebServiceUtils.SetJsonHeader(context);
                    context.Response.SendResponse(HttpStatusCode.Ok, folder.ToJObject().ToString(), Encoding.UTF8, ContentType.JSON);
                    res = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendNodeListResponseAsJson:");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendNodeResponseAsJson(IHttpContext context, INode node)
        {
            bool res = false;
            try
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                WebServiceUtils.AddNoCacheHeader(context);
                WebServiceUtils.SetJsonHeader(context);
                context.Response.SendResponse(HttpStatusCode.Ok, node.ToJObject().ToString(), Encoding.UTF8, ContentType.JSON);
                res = true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendNodeResponseAsJson:");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendNodesResponseAsJson(IHttpContext context, INode[] nodes)
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
                Logger.Error(e, "SendNodeListResponseAsJson:");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendFolderResponseAsXml(IHttpContext context, IFolder folder)
        {
            bool res = false;
            try
            {
                using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
                {
                    using (var xmlWriter = XmlWriter.Create(sw, XmlUtils.XmlWriterSettings()))
                    {
                        folder?.WriteXml(xmlWriter);
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
                Logger.Error(e, "SendFolderResponseAsXml:");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendNodeResponseAsXml(IHttpContext context, INode node)
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
                Logger.Error(e, "SendNodeResponseAsXml:");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }

        public static bool SendNodesResponseAsXml(IHttpContext context, INode[] nodes, Dictionary<string, string> attributeDictionary = null)
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
                Logger.Error(e, "SendNodeListResponseAsXml:");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return res;
        }
/*
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
        }*/
    }
}
