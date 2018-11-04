// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataServiceHandler.cs" company="Innotive Inc. Korea">
//   Copyright (c) Innotive Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The create type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Xml;
using Iot.Common.ClassLogger;

namespace InnoWatchApi.DataService
{
    /// <summary>
    /// The data service handler class.
    /// </summary>
    public static class DataServiceHandler
    {
        private static readonly ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();

        public static string RequestToStringResponse(RequestParameter parameter, bool isPost = false)
        {
            var url = parameter.Url;

            var encodingOption = GetEncodingOption(parameter.EncodingOption);

            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var responseString = string.Empty;

            try
            {
                // 로컬 파일 읽기
                if (url.Contains("http://") == false)
                {
                    var reader = new StreamReader(url, encodingOption);
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }

                var httpWebRequest = WebRequestOnly(url);

                if (httpWebRequest == null)
                    return string.Empty;

                if (isPost)
                {
                    httpWebRequest.Method = "POST";

                    var postData = parameter.PostMessage;
                    var byteArray = encodingOption.GetBytes(postData);

                    // Set the ContentType property of the WebRequest.
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    httpWebRequest.ContentLength = byteArray.Length;

                    var dataStream = httpWebRequest.GetRequestStream();

                    dataStream.Write(byteArray, 0, byteArray.Length);

                    dataStream.Close();
                }

                using (var streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
                {
                    responseString = streamReader.ReadToEnd();
                }

                //using (var webResponse = httpWebRequest.GetResponse())
                //{
                //    using (var streamReader = new StreamReader(webResponse.GetResponseStream(), encodingOption))
                //    {
                //        responseString = streamReader.ReadToEnd();
                //    }
                //}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Web Request Error] ", ex.ToString());
                Logger.Error(ex.ToString());
                //Logger.Write(Logger.Error, string.Format("[Web request Error] : {0}", ex.ToString()));
            }

            return responseString;
        }

        public static string RequestToFileTransfer(RequestParameter parameter)
        {
            var url = parameter.Url;

            var encodingOption = GetEncodingOption(parameter.EncodingOption);

            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var responseString = string.Empty;

            try
            {
                // 로컬 파일 읽기
                if (url.Contains("http://") == false)
                {
                    var reader = new StreamReader(url, encodingOption);
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }

                var httpWebRequest = WebRequestOnly(url);

                if (httpWebRequest == null)
                    return string.Empty;

                httpWebRequest.Method = "POST";

                // Set the ContentType property of the WebRequest.
                httpWebRequest.ContentType = "multi-part/form-data";
                httpWebRequest.ContentLength = parameter.PostByteData.Length;

                using(var dataStream = httpWebRequest.GetRequestStream())
                {
                    dataStream.Write(parameter.PostByteData, 0, parameter.PostByteData.Length);    
                }

                using (var webResponse = httpWebRequest.GetResponse())
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(stream, encodingOption))
                        {
                            responseString = streamReader.ReadToEnd();
                        }    
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Web Request Error");
            }

            return responseString;
        }
        
        /// <summary>
        /// REST 요청으로 응답온 Stream을 DataSet으로 반환한다.
        /// </summary>
        /// <param name="parameter">The PollingParameter.</param>
        /// <param name="isPost">The isPost.</param>
        /// <returns>The DataSet.</returns>
        public static DataSet RequestToDataSetResponseByRest(RequestParameter parameter, bool isPost = false)
        {
            DataSet dataSet = null;

            try
            {
                using (var webResponse = GetResponseStream(parameter, isPost))
                {
                    if (webResponse == null)
                    {
                        return null;
                    }
                        
                    using (var stream = webResponse.GetResponseStream())
                    {
                        if (stream == null)
                        {
                            return null;
                        }

                        dataSet = new DataSet();
                        dataSet.ReadXml(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "RequestToDataSetResponseByRest:");
                dataSet = null;
            }

            return dataSet;
        }

        public static DataTable RequestToDataTableResponseByRest(RequestParameter parameter, bool isPost = false)
        {
            try
            {
                using (var webResponse = GetResponseStream(parameter, isPost))
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        if (stream == null)
                            return null;

                        var dataTable = new DataTable();
                        dataTable.ReadXml(stream);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "RequestToDataTableResponseByRest:");
                return null;
            }
        }

        public static DataTable RequestToFirstDataTableResponseByRest(RequestParameter parameter, bool isPost = false)
        {
            var dataSet = RequestToDataSetResponseByRest(parameter, isPost);
            return dataSet == null ? null : dataSet.Tables[0];
        }

        public static bool RequestToBoolResponse(RequestParameter parameter, bool isPost = false)
        {
            try
            {
                string message;
                return RequestToBoolResponse(parameter, out message, isPost);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "RequestToBoolResponse:");
                return false;
            }
        }

        public static bool RequestToBoolResponse(RequestParameter parameter, out string message, bool isPost = false)
        {
            try
            {
                message = string.Empty;
                using (var webResponse = GetResponseStream(parameter, isPost))
                {
                    using (var response = webResponse.GetResponseStream())
                    {
                        if (response == null)
                        {
                            return false;
                        }

                        message = CheckResultXml(response);
                    }
                }

                return string.IsNullOrWhiteSpace(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "RequestToBoolResponse:");
                message = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// WebResponse 를 이용해 파일을 다운로드 한다.
        /// WebResponse 사용의 일관성을 두기 위해 여기에 메서드를 생성함.
        /// </summary>
        /// <returns>The result to boolean.</returns>
        public static bool RequestFileTransferProcess(RequestParameter parameter, string path, out string message, bool isPost = false)
        {
            try
            {
                using (var webResponse = GetResponseStream(parameter, isPost))
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        if (stream == null)
                        {
                            message = "Response Stream is null.";
                            return false;
                        }

                        using (var writeStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                        {
                            var bufferSize = 2048;
                            var buffer = new byte[bufferSize];
                            int readLength;

                            while ((readLength = stream.Read(buffer, 0, bufferSize)) != 0)
                            {
                                writeStream.Write(buffer, 0, readLength);
                            }
                        }
                    }
                }

                message = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "RequestFileTransferProcess:");
                message = ex.Message;
                return false;
            }
        }

        private static WebResponse GetResponseStream(RequestParameter parameter, bool isPost = false)
        {
            WebResponse response = null;
            var url = parameter.Url;
            var encodingOption = GetEncodingOption(parameter.EncodingOption);

            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                var req = WebRequestOnly(url);

                if (req == null)
                    return null;

                req.Proxy = null;

                if (isPost)
                {
                    req.Method = "POST";

                    var postData = parameter.PostMessage;
                    var byteArray = encodingOption.GetBytes(postData);

                    // Set the ContentType property of the WebRequest.
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = byteArray.Length;

                    using(var dataStream = req.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);    
                    }
                }

                response = req.GetResponse();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "GetResponseStream:");
            }

            return response;
        }

        private static HttpWebRequest WebRequestOnly(string url)
        {
            try
            {
                var req = (WebRequest.Create(url) as HttpWebRequest);

                if (req != null)
                {
                    //req.Timeout = 5000;
                    req.Timeout = 20000; //default
                    req.ContentType = "multipart/form-data";
                    req.Method = "GET";
                    req.SendChunked = false;
                    req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                    return req;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "GetResponseStream:");
                return null;
            }
        }

        private static Encoding GetEncodingOption(string encondingOption)
        {
            Encoding encodingOption;

            // Contents에 지정된 EncodingOption을 가져와서 설정한다.
            switch (encondingOption)
            {
                case "ASCII":
                    encodingOption = Encoding.ASCII;
                    break;
                case "BigEndianUnicode":
                    encodingOption = Encoding.BigEndianUnicode;
                    break;
                case "Unicode":
                    encodingOption = Encoding.Unicode;
                    break;
                case "UTF32":
                    encodingOption = Encoding.UTF32;
                    break;
                case "UTF7":
                    encodingOption = Encoding.UTF7;
                    break;
                case "UTF8":
                    encodingOption = Encoding.UTF8;
                    break;
                case "Default":
                    encodingOption = Encoding.Default;
                    break;
                default:
                    encodingOption = Encoding.UTF8;
                    break;
            }

            return encodingOption;
        }
        public static string CheckResultXml(Stream response)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(response);

                if (doc.DocumentElement == null)
                {
                    Logger.Error(String.Format("\r\nMessage : {0}", "Response Xml Parsing Failed!"));
                    //Logger.Write(Logger.Error, Environment.StackTrace, String.Format("\r\nMessage : {0}", "Response Xml Parsing Failed!"));
                    return "Response Xml Parsing Failed!";
                }

                var result = doc.DocumentElement.SelectSingleNode("Result");

                if (result == null)
                {
                    Logger.Error(String.Format("\r\nMessage : {0}", "Result node is invalid."));
                    //Logger.Write(Logger.Error, Environment.StackTrace, String.Format("\r\nMessage : {0}", "Result node is invalid."));
                    return "Result node is invalid.";
                }

                if (result.Attributes == null)
                {
                    Logger.Error(String.Format("\r\nMessage : {0}", "Value node is invalid."));
                    //Logger.Write(Logger.Error, Environment.StackTrace, String.Format("\r\nMessage : {0}", "Value node is invalid."));
                    return "Value node is invalid.";
                }
                var attr = result.Attributes.GetNamedItem("value");

                if (String.Compare(attr.Value.ToLower(), "success") != 0)
                {
                    var messageAttr = result.Attributes.GetNamedItem("message");
                    Logger.Error(String.Format("\r\nMessage : {0}", messageAttr.Value));
                    //Logger.Write(Logger.Error, Environment.StackTrace, String.Format("\r\nMessage : {0}", messageAttr.Value));

                    //ShowPopupWindow(Resource_Popup.RequestErrorTitle, messageAttr.Value);

                    return messageAttr.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                //Logger.Write(Logger.Error, GetMethodInfoStrings.GetMethodName(), ex.ToString());
                //ShowPopupWindow(Resource_Popup.RequestErrorTitle, ex.ToString());

                return ex.Message;
            }

            return String.Empty;
        }
    }
}
