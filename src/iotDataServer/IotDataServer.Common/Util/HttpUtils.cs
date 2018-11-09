using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using NLog;

namespace IotDataServer.Common.Util
{
    public class HttpUtils
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public static int HttpRequest(string requestUri, bool isPost, string postData, string contentType, Encoding encoding, out string resultString)
        {
            HttpStatusCode responseStatusCode = HttpStatusCode.BadRequest;
            resultString = "";
            HttpWebResponse response = null;
            try
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUri);
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7";
                request.Timeout = 2000;
                request.ReadWriteTimeout = 2000;
                request.Accept = "*/*";
                request.Referer = "http://maps.google.com/";
                if (isPost)
                {
                    request.Method = "POST";

                    byte[] byteArray = encoding.GetBytes(postData);
                    contentType = string.IsNullOrWhiteSpace(contentType) ? "application/x-www-form-urlencoded" : contentType;
                    request.ContentType = $"{contentType}; charset={encoding.WebName}";

                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                else
                {
                    contentType = string.IsNullOrWhiteSpace(contentType) ? "text/plain" : contentType;
                    request.ContentType = $"{contentType}; charset={encoding.WebName}";
                }

                response = (HttpWebResponse) request.GetResponse();
                responseStatusCode = response.StatusCode;
                using (Stream stReadData = response.GetResponseStream())
                {
                    if (stReadData != null)
                    {
                        using (StreamReader srReadData = new StreamReader(stReadData, encoding ?? Encoding.UTF8))
                        {
                            resultString = srReadData.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Cannot http request for '{requestUri}'.");
                resultString = "";
                responseStatusCode = HttpStatusCode.InternalServerError;
            }
            finally
            {
                response?.Close();
            }

            return (int)responseStatusCode;
        }

        public static string HttpRequest(string requestUri, bool isPost = false, string postData = "", string contentType = "", Encoding encoding = null)
        {
            string resultString = "";
            int responseStatusCode = HttpRequest(requestUri, isPost, postData, contentType, encoding, out resultString);
            
            return resultString;
        }

        public static string HttpGetRequest(string requestUri, Dictionary<string, string> queryData = null)
        {
            string queryString = GetHttpNameValuePairString(queryData);
            queryString = string.IsNullOrWhiteSpace(queryString) ? "" : "?" + queryString;

            return HttpRequest($"{requestUri}{queryString}");
        }

        public static string HttpPostRequest(string requestUri, Dictionary<string, string> postData = null)
        {
            string postString = GetHttpNameValuePairString(postData);

            return HttpRequest(requestUri, true, postString);
        }

        public static string HttpPostRequest(string requestUri, string body)
        {
            return HttpRequest(requestUri, true, body);
        }

        public static string HttpRequestEucKr(string requestUri)
        {
            return HttpRequest(requestUri, false, "", "", Encoding.GetEncoding("euc-kr"));
        }

        public static string GetHttpNameValuePairString(Dictionary<string, string> queryData)
        {
            if (queryData == null)
            {
                return "";
            }

            List<string> queryDataList = new List<string>();
            foreach (KeyValuePair<string, string> keyValuePair in queryData)
            {
                string key = "";
                string value = "";

                // todo : 한시적으로 servicekey 일때만, URL 인코딩 사용 안함.
                switch (keyValuePair.Key)
                {
                    case "key":
                    case "serviceKey":
                        key = keyValuePair.Key;
                        value = keyValuePair.Value;
                        break;
                    default:
                        key = HttpUtility.UrlEncode(keyValuePair.Key);
                        value = HttpUtility.UrlEncode(keyValuePair.Value);
                        break;
                }

                queryDataList.Add($"{key}={value}");

            }

            return string.Join("&", queryDataList);
        }

        public static bool RemoteUrlFileExist(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
