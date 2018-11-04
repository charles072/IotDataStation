using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using Iot.Common.ClassLogger;
using Iot.Common.DataModel;
using Iot.Common.Util;
using IotDataServer.Common.DataModel;
using IotDataServer.Common.HttpServer;

namespace IotDataServer.WebService
{
    [RestResource(BasePath = "/iot")]
    public class WebService
    {
        private static readonly ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();

        [RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/")]
        public IHttpContext Index(IHttpContext context)
        {
            try
            {
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

            linkInfos.Add(new PageLinkInfo("modes", "/node/", "test data"));

            return linkInfos;
        }
    }
}
