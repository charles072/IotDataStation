using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using IotDataStation.Common.DataModel;
using IotDataStation.DataModel;
using IotDataStation.HttpServer;
using IotDataStation.Common.Interface;
using IotDataStation.Common.Util;
using IotDataStation.Util;
using Newtonsoft.Json.Linq;
using NLog;


namespace IotDataStation.WebService
{
    [RestResource(BasePath = "/iot/data")]
    public class WebService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/")]
        public IHttpContext Index(IHttpContext context)
        {
            try
            {
                if (WebServiceUtils.CheckAuthentication(context))
                {
                    return context;
                }
                context.Response.SendResponse(HttpStatusCode.Ok, TemplateManager.Rander("PageLinks", GetServicePageList()), Encoding.UTF8);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot GetXmlData!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return context;
        }

        private object GetServicePageList()
        {
            List<PageLinkInfo> linkInfos = new List<PageLinkInfo>();

            IDataRepository dataRepository = DataStation.DataRepository;
            var nodeStatusSummaries = dataRepository.NodeStatusSummaries;

            foreach (var nodeStatusSummary in nodeStatusSummaries)
            {
                linkInfos.Add(new PageLinkInfo($"nodes/{nodeStatusSummary.Path}", $"nodes/{nodeStatusSummary.Path}", "Nodes"));
            }

            foreach (var nodeStatusSummary in nodeStatusSummaries)
            {
                linkInfos.Add(new PageLinkInfo($"folder/{nodeStatusSummary.Path}", $"folder/{nodeStatusSummary.Path}", "Folder"));
            }

            return linkInfos;
        }

        [RestRoute(HttpMethod = HttpMethod.ALL, PathInfo = "/node(?<path>(/[a-zA-Z0-9_]+)+)/(?<nodeId>[a-zA-Z0-9_]+)$")]
        public IHttpContext ServiceNode(IHttpContext context)
        {
            IHttpContext responseContext = null;
            switch (context.Request.HttpMethod)
            {
                case HttpMethod.GET:
                    responseContext = GetNode(context);
                    break;

                case HttpMethod.POST:
                case HttpMethod.PUT:
                    responseContext = SetNode(context);
                    break;
            }

            if (responseContext == null)
            {
                context.Response.SendResponse(HttpStatusCode.BadRequest);
                responseContext = context;
            }
            return responseContext;
        }

        public IHttpContext GetNode(IHttpContext context)
        {
            try
            {
                if (WebServiceUtils.CheckAuthentication(context))
                {
                    return context;
                }

                var webParameter = WebServiceUtils.GetNameValueCollection(context);
                string responseFormat = WebServiceUtils.GetQueryStringValue(webParameter, "format", "json").ToLower();

                string path = "";
                string nodeId = "";
                var match = Regex.Match(context.Request.PathInfo, @"/node(?<path>(/[a-zA-Z0-9_]+)+)/(?<nodeId>[a-zA-Z0-9_]+)$");
                if (match.Success)
                {
                    path = match.Groups["path"].Value;
                    nodeId = match.Groups["nodeId"].Value;
                }

                if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(nodeId))
                {
                    context.Response.SendResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    IDataRepository dataRepository = DataStation.DataRepository;
                    INode node = dataRepository.GetNode(path, nodeId);

                    if (node != null)
                    {
                        if (responseFormat == "xml")
                        {
                            WebResponse.SendNodeResponseAsXml(context, node);
                        }
                        else
                        {
                            WebResponse.SendNodeResponseAsJson(context, node);
                        }
                    }
                    else
                    {
                        context.Response.SendResponse(HttpStatusCode.BadRequest);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot GetXmlData!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return context;
        }
        public IHttpContext SetNode(IHttpContext context)
        {
            try
            {
                if (WebServiceUtils.CheckAuthentication(context))
                {
                    return context;
                }

                var webParameter = WebServiceUtils.GetNameValueCollection(context);
                string responseFormat = WebServiceUtils.GetQueryStringValue(webParameter, "format", "json").ToLower();

                string path = "";
                string nodeId = "";
                var match = Regex.Match(context.Request.PathInfo, @"/node(?<path>(/[a-zA-Z0-9_]+)+)/(?<nodeId>[a-zA-Z0-9_]+)$");
                if (match.Success)
                {
                    path = match.Groups["path"].Value;
                    nodeId = match.Groups["nodeId"].Value;
                }

                string jsonString = context.Request.Payload;
                
                Node node = Node.CreateFrom(JObject.Parse(jsonString));
                if (node == null)
                {
                    context.Response.SendResponse(HttpStatusCode.BadRequest);
                }
                else if (node.Id != nodeId)
                {
                    string message = $"Node ID is mismatched ('{nodeId}' != '{node.Id}').";
                    Logger.Warn(message);
                    context.Response.SendResponse(HttpStatusCode.BadRequest, message);
                }
                else
                {
                    IDataRepository dataRepository = DataStation.DataRepository;
                    if (dataRepository.SetNode(path, node))
                    {
                        WebResponse.SendNodeResponseAsJson(context, node);
                    }
                    else
                    {
                        context.Response.SendResponse(HttpStatusCode.BadRequest);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot SetNode!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return context;
        }


        [RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/nodes(?<path>(/[a-zA-Z0-9_]+)+)$")]
        public IHttpContext GetNodes(IHttpContext context)
        {
            try
            {
                if (WebServiceUtils.CheckAuthentication(context))
                {
                    return context;
                }

                var webParameter = WebServiceUtils.GetNameValueCollection(context);
                string responseFormat = WebServiceUtils.GetQueryStringValue(webParameter, "format", "json").ToLower();

                string path = "";
                var match = Regex.Match(context.Request.PathInfo, @"/nodes(?<path>(/[a-zA-Z0-9_]+)+)$");
                if (match.Success)
                {
                    path = match.Groups["path"].Value;
                }

                IDataRepository dataRepository = DataStation.DataRepository;
                INode[] nodes = dataRepository.GetNodes(path);
                if (responseFormat == "xml")
                {
                    WebResponse.SendNodesResponseAsXml(context, nodes);
                }
                else
                {
                    WebResponse.SendNodesResponseAsJson(context, nodes);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot GetXmlData!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return context;
        }

        [RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/folder(?<path>(/[a-zA-Z0-9_]+)+)$")]
        public IHttpContext GetFolder(IHttpContext context)
        {
            try
            {
                if (WebServiceUtils.CheckAuthentication(context))
                {
                    return context;
                }

                var webParameter = WebServiceUtils.GetNameValueCollection(context);
                string responseFormat = WebServiceUtils.GetQueryStringValue(webParameter, "format", "json").ToLower();
                int depth = StringUtils.GetIntValue(WebServiceUtils.GetQueryStringValue(webParameter, "depth", "1"), 1);
                bool includeNodes = StringUtils.GetValue(WebServiceUtils.GetQueryStringValue(webParameter, "includeNodes", "false"), true);

                string path = "";
                var match = Regex.Match(context.Request.PathInfo, @"/folder(?<path>(/[a-zA-Z0-9_]+)+)$");
                if (match.Success)
                {
                    path = match.Groups["path"].Value;
                }

                IDataRepository dataRepository = DataStation.DataRepository;
                var folder = dataRepository.GetFolder(path, depth, includeNodes);
                if (responseFormat == "xml")
                {
                    WebResponse.SendFolderResponseAsXml(context, folder);
                }
                else
                {
                    WebResponse.SendFolderResponseAsJson(context, folder);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot GetXmlData!");
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
            return context;
        }
    }
}
