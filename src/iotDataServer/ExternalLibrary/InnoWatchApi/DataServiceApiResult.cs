using System;
using System.Xml;

namespace InnoWatchApi
{
    public class DataServiceApiResult
    {
        public DataServiceApiRequest Request { get; }
        public DataServiceApiResultCode ResultCode { get; }
        public Exception Error { get; }
        public string ResponseString { get; }
        public XmlElement ResponseXml { get; }

        public string ResultMessage => Error?.Message ?? GetDefaultResultMessage(ResultCode);

        public DataServiceApiResult(DataServiceApiRequest request, DataServiceApiResultCode resultCode, Exception error, string responseString, XmlElement responseXml)
        {
            Request = request;
            ResultCode = resultCode;
            Error = error;
            ResponseString = responseString ?? "";
            ResponseXml = responseXml;
        }

        private string GetDefaultResultMessage(DataServiceApiResultCode resultCode)
        {
            return resultCode.ToString();
        }
    }
}