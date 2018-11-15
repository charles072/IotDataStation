using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;

namespace IotDataStation.WebService
{
    public class WebServiceUtils
    {
        public static bool CheckAuthentication(IHttpContext context)
        {
            bool res = DataStation.Authentication.ValidateUser(context);
            if (!res)
            {
                context.Response.SendResponse(HttpStatusCode.Unauthorized);
            }

            return res;
        }

        public static NameValueCollection GetNameValueCollection(IHttpContext context)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            var getQueryString = context.Request.QueryString;
            if (getQueryString != null)
            {
                foreach (string key in getQueryString.Keys)
                {
                    try
                    {
                        nameValueCollection[key] = getQueryString.Get(key).Trim();
                    }
                    catch
                    {
                        //Ignore;
                    }
                }
            }
            if (context.Request.HttpMethod == HttpMethod.POST)
            {
                if (context.Request.Headers["Content-Type"].Contains("application/x-www-form-urlencoded"))
                {
                    var queryString = HttpUtility.ParseQueryString(context.Request.Payload);
                    foreach (string key in queryString.Keys)
                    {
                        try
                        {
                            nameValueCollection[key] = queryString.Get(key).Trim();
                        }
                        catch
                        {
                            //Ignore;
                        }
                    }
                }
            }
            return nameValueCollection;
        }

        public static string GetQueryStringValue(NameValueCollection qscoll, string name, string defaultValue = "")
        {
            if (qscoll == null)
            {
                return defaultValue;
            }
            return qscoll.GetValues(name)?[0] ?? defaultValue;
        }


        public static void SetJsonHeader(IHttpContext context)
        {
            AddNoCacheHeader(context);
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.ContentType = ContentType.JSON;
        }

        public static void AddNoCacheHeader(IHttpContext context)
        {
            context.Response.Headers["Cache-Control"] = "cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no - cache";
            context.Response.Headers["Expires"] = "0";
        }
        
        public static void SendFile(IHttpContext context, string filepath)
        {
            if (!File.Exists(filepath))
            {
                filepath = Path.Combine(context.Server.PublicFolder.FolderPath, filepath);
                if (!File.Exists(filepath))
                {
                    foreach (var serverPublicFolder in context.Server.PublicFolders)
                    {
                        filepath = Path.Combine(serverPublicFolder.FolderPath, filepath);
                        if (File.Exists(filepath))
                        {
                            break;
                        }
                    };
                }
            }
            if (File.Exists(filepath))
            {
                var lastModified = File.GetLastWriteTimeUtc(filepath).ToString("R");
                context.Response.AddHeader("Last-Modified", lastModified);

                if (context.Request.Headers.AllKeys.Contains("If-Modified-Since"))
                {
                    if (context.Request.Headers["If-Modified-Since"].Equals(lastModified))
                    {
                        context.Response.SendResponse(HttpStatusCode.NotModified);
                        return;
                    }
                }

                ContentType contentType = ContentType.TEXT;
                contentType.FromExtension(filepath);

                context.Response.SendResponse(new FileStream(filepath, FileMode.Open), contentType.FromExtension(filepath));
            }
            else
            {
                context.Response.SendResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
