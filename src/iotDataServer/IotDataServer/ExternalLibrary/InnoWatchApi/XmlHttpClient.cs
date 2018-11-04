using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace InnoWatchApi
{
    public class XmlHttpClient
    {
        public delegate void XmlHttpClientCompletedHandler(object sender, object userToken, XmlHttpResult result);
        public event XmlHttpClientCompletedHandler RequestCompleted;

        public XmlHttpResult Get(string url, Dictionary<string, string> dictParam)
        {
            return Get(UrlFactory.GetUri(url, dictParam));
        }

        public XmlHttpResult Get(string baseUrl, string function, Dictionary<string, string> dictParam)
        {
            return Get(UrlFactory.GetUri(baseUrl, function, dictParam));
        }

        public XmlHttpResult Get(Uri uri)
        {

            using (var client = new WebClient())
            {
                // Assign all the important stuff
                //client.Proxy = this.Proxy;
                //client.Headers[HttpRequestHeader.UserAgent] = this.UserAgent;

                // Run DownloadString() as a task.

                XmlHttpResult result = null;
                try
                {
                    client.Encoding = Encoding.UTF8;
                    string resString = client.DownloadString(uri);
                    result = new XmlHttpResult(XmlHttpResultCode.Success, resString);
                }
                catch (Exception e)
                {
                    result = new XmlHttpResult(XmlHttpResultCode.Error, e);
                }

                return result;
            }
        }


        public void GetAsync(string url, Dictionary<string, string> dictParam, object userToken = null)
        {
            GetAsync(UrlFactory.GetUri(url, dictParam), userToken);
        }

        public void GetAsync(string baseUrl, string function, Dictionary<string, string> dictParam, object userToken = null)
        {
            GetAsync(UrlFactory.GetUri(baseUrl, function, dictParam), userToken);
        }

        public void GetAsync(Uri uri, object userToken = null)
        {

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string resString = string.Empty;
                // Assign all the important stuff
                //client.Proxy = this.Proxy;
                client.Headers[HttpRequestHeader.CacheControl] = "no-store,no-cache";
                client.Headers[HttpRequestHeader.Pragma] = "no-cache";

                // Run DownloadString() as a task.

                XmlHttpResult result = null;
                try
                {
                    client.DownloadStringCompleted += ClientOnDownloadStringCompleted;
                    client.DownloadStringAsync(uri, userToken);
                }
                catch (Exception e)
                {
                    result = new XmlHttpResult(XmlHttpResultCode.Error, e);
                }

                if (result != null)
                {
                    OnRequestCompleted(userToken, result);
                }
            }
        }

        private void ClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Cancelled)
            {
                OnRequestCompleted(downloadStringCompletedEventArgs.UserState, new XmlHttpResult(XmlHttpResultCode.Canceled, downloadStringCompletedEventArgs.Error, downloadStringCompletedEventArgs.Result));
            }
            else if (downloadStringCompletedEventArgs.Error == null)
            {
                OnRequestCompleted(downloadStringCompletedEventArgs.UserState,
                    new XmlHttpResult(XmlHttpResultCode.Success, downloadStringCompletedEventArgs.Result));
            }
            else
            {
                OnRequestCompleted(downloadStringCompletedEventArgs.UserState,
                    new XmlHttpResult(XmlHttpResultCode.Error, downloadStringCompletedEventArgs.Error));
            }
        }

        protected virtual void OnRequestCompleted(object userToken, XmlHttpResult result)
        {
            RequestCompleted?.Invoke(this, userToken, result);
        }

        public void SetHearder(HttpRequestHeader httpRequestHeader, string value)
        {
            throw new NotImplementedException();
        }
    }

    public enum XmlHttpResultCode
    {
        Success,
        Error,
        Canceled,
        ErrorXmlParsing
    }

    public class XmlHttpResult
    {
        public XmlHttpResultCode ResultCode { get; private set; }
        public bool IsSuccess => (ResultCode == XmlHttpResultCode.Success);
        public bool IsCancelled => (ResultCode == XmlHttpResultCode.Canceled);
        public Exception Error { get; private set; }
        public string ResponseString { get; private set; }
        public XmlElement ResponseXmlRoot { get; private set; }
        
        public XmlHttpResult(XmlHttpResultCode resultCode, Exception error = null, string response = "")
        {
            Set(resultCode, error, response);
        }

        public XmlHttpResult(XmlHttpResultCode resultCode, string response)
            : this(resultCode, null, response)
        {

        }

        public void Set(XmlHttpResultCode resultCode, Exception error = null, string response = "")
        {
            ResultCode = resultCode;
            Error = error;
            ResponseString = response ?? "";

            var xmlDoc = new XmlDocument();

            if (resultCode == XmlHttpResultCode.Success && response != "")
            {
                try
                {
                    xmlDoc.LoadXml(ResponseString);
                    ResponseXmlRoot = xmlDoc.DocumentElement;
                }
                catch(Exception)
                {
                    ResultCode = XmlHttpResultCode.ErrorXmlParsing;
                    ResponseXmlRoot = xmlDoc.CreateElement("ROOT");
                }
            }
            else
            {
                ResponseXmlRoot = xmlDoc.CreateElement("ROOT");
            }
        }
    }

}
