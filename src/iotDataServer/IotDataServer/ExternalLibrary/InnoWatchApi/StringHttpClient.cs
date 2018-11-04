using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InnoWatchApi
{
    public class StringHttpClient
    {
        public delegate void StringHttpClientCompletedHandler(object sender, object userToken, StringHttpResult result);
        public event StringHttpClientCompletedHandler RequestCompleted;

        public StringHttpResult Get(string url, Dictionary<string, string> dictParam)
        {
            return Get(UrlFactory.GetUri(url, dictParam));
        }

        public StringHttpResult Get(string baseUrl, string function, Dictionary<string, string> dictParam)
        {
            return Get(UrlFactory.GetUri(baseUrl, function, dictParam));
        }

        public StringHttpResult Get(Uri uri)
        {

            using (var client = new WebClient())
            {
                // Assign all the important stuff
                //client.Proxy = this.Proxy;
                //client.Headers[HttpRequestHeader.UserAgent] = this.UserAgent;

                // Run DownloadString() as a task.

                StringHttpResult result = null;
                try
                {
                    string resString = client.DownloadString(uri);
                    result = new StringHttpResult(StringHttpResultCode.Success, resString);
                }
                catch (Exception e)
                {
                    result = new StringHttpResult(StringHttpResultCode.Error, e);
                }
                return result;
            }
        }


        public void GetAsync(string url, Dictionary<string, string> dictParam)
        {
            GetUriAsync(UrlFactory.GetUri(url, dictParam));
        }

        public void GetUriAsync(string baseUrl, string function, Dictionary<string, string> dictParam)
        {
            GetUriAsync(UrlFactory.GetUri(baseUrl, function, dictParam));
        }

        public void GetUriAsync(Uri uri, object userToken = null)
        {

            using (var client = new WebClient())
            {
                string resString = string.Empty;
                // Assign all the important stuff
                //client.Proxy = this.Proxy;
                //client.Headers[HttpRequestHeader.UserAgent] = this.UserAgent;

                // Run DownloadString() as a task.

                StringHttpResult result = null;
                try
                {
                    client.DownloadStringCompleted += ClientOnDownloadStringCompleted;
                    client.DownloadStringAsync(uri, userToken);
                }
                catch (Exception e)
                {
                    result = new StringHttpResult(StringHttpResultCode.Error, e);
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
                OnRequestCompleted(downloadStringCompletedEventArgs.UserState, new StringHttpResult(StringHttpResultCode.Canceled, downloadStringCompletedEventArgs.Error, downloadStringCompletedEventArgs.Result));
            }
            else if (downloadStringCompletedEventArgs.Error == null)
            {
                OnRequestCompleted(downloadStringCompletedEventArgs.UserState,
                    new StringHttpResult(StringHttpResultCode.Success, downloadStringCompletedEventArgs.Result));
            }
            else
            {
                OnRequestCompleted(downloadStringCompletedEventArgs.UserState,
                    new StringHttpResult(StringHttpResultCode.Error, downloadStringCompletedEventArgs.Error));
            }
        }

        protected virtual void OnRequestCompleted(object userToken, StringHttpResult result)
        {
            RequestCompleted?.Invoke(this, userToken, result);
        }
    }

    public enum StringHttpResultCode
    {
        Success,
        Error,
        Canceled
    }

    public class StringHttpResult
    {
        public StringHttpResultCode ResultCode { get; }
        public bool IsSuccess => (ResultCode == StringHttpResultCode.Success);
        public bool IsCancelled => (ResultCode == StringHttpResultCode.Canceled);
        public Exception Error { get; }
        public string Response { get; }

        public StringHttpResult(StringHttpResultCode resultCode, Exception error=null, string response = "")
        {
            ResultCode = resultCode;
            Error = error;
            Response = response ?? "";
        }

        public StringHttpResult(StringHttpResultCode resultCode, string response)
            :this(resultCode, null, response)
        {
            
        }

    }

}
