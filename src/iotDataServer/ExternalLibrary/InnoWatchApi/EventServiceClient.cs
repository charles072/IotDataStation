using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using InnoWatchApi.DataModel;
using Iot.Common.ClassLogger;
using Iot.Common.Util;


namespace InnoWatchApi
{
    public class EventServiceClient
    {
        private static readonly ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        public string HostUrl { get; private set; }
        public bool WithGis { get; set; } = false;
        public EventServiceClient(string hostUrl)
        {
            HostUrl = hostUrl;
            if (!HostUrl.EndsWith("/"))
            {
                HostUrl += "/";
            }
        }

        public void FireEvent(AlarmEvent alarmEvent)
        {
            if (string.IsNullOrWhiteSpace(alarmEvent.AlarmIds))
            {
                Logger.Warn("eventIds is empty!");
            }
            else
            {
                FireEvent(alarmEvent.AlarmIds, alarmEvent);
            }
        }

        public void FireEvent(string alarmIds, AlarmEvent alarmEvent)
        {
            if (string.IsNullOrWhiteSpace(alarmIds))
            {
                Logger.Warn("eventIds is empty!");
            }
            else
            {
                string[] splitEventIds = alarmEvent.AlarmIds.Split(',');
                foreach (string alarmid in splitEventIds)
                {
                    SendAlarmEvent(alarmid, alarmEvent);
                }
            }
        }

        private void SendAlarmEvent(string alarmId, AlarmEvent alarmEvent)
        {
            Task.Factory.StartNew(() => DoSendAlarmEvent(alarmId, alarmEvent));
        }

        private void DoSendAlarmEvent(string alarmId, AlarmEvent alarmEvent)
        {
            WebResponse response = null;
            try
            {
                string requestUri = $"{HostUrl}/NotifyEvent";
                if (WithGis)
                {
                    requestUri = $"{HostUrl}/NotifyEventWithLocation";
                }

                WebRequest request = WebRequest.Create(requestUri);
                request.Timeout = 2000;
                request.Method = "POST";

                string postData = GetEventNotifyData(alarmId, alarmEvent);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = $"application/xml; charset={Encoding.UTF8.WebName}";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                response = request.GetResponse();
                Logger.Trace($"Sent new alarm notify for '{alarmId}/{alarmEvent.Message}'!");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Cannot send alarm event for '{alarmId}'.");
            }
            finally
            {
                response?.Close();
            }
        }

        private string GetEventNotifyData(string alarmId, AlarmEvent alarmEvent)
        {
            string postData = "";

            if (alarmEvent != null)
            {
                using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
                {
                    using (var xw = XmlWriter.Create(sw, XmlUtils.XmlWriterSettings()))
                    {
                        alarmEvent.WriteInnoWatchAlarmXml(xw, alarmId);
                    }
                    postData = sw.ToString();
                }
            }
            else
            {
                Logger.Warn("alarmEvent is null.");
            }
            return postData;
        }

    }
}
