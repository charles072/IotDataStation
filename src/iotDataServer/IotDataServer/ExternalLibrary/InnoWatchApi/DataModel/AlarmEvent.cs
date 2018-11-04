using System;
using System.Xml;
using Iot.Common.DataModel;
using Iot.Common.Util;

namespace InnoWatchApi.DataModel
{
    public class AlarmEvent
    {
        public string AlarmIds { get; set; }
        public DateTime EventTime { get; set; }
        public string Location { get; set; }
        public string AlarmStatus { get; set; }
        public string ColorCode { get; set; }
        public DeviceStatus DeviceStatus { get; set; }
        public string DisplayEventType => DeviceStatus.ToString();
        public string Message { get; set; }
        public string IconUrl { get; set; }

        public AlarmEvent()
        {
            AlarmIds = "";
            EventTime = DateTime.Now;
            Location = "";
            AlarmStatus = "Start";
            ColorCode = "#FC0C0C";
            DeviceStatus = DeviceStatus.None;
            Message = "";
            IconUrl = "";
        }

        public AlarmEvent(string alarmIds, DeviceStatus deviceStatus, string message, string location) : this()
        {
            AlarmIds = alarmIds;
            DeviceStatus = deviceStatus;
            Message = message;
            Location = location;
        }

        public void WriteInnoWatchAlarmXml(XmlWriter xmlWriter, string alarmId = "")
        {
            string alarmName = string.IsNullOrEmpty(alarmId) ? AlarmIds : alarmId;
            xmlWriter.WriteStartElement("AlarmNotifyInfo");
            xmlWriter.WriteElementString("AlarmId", alarmName);
            //xmlWriter.WriteElementString("TimeZone", "UTC");  //InnoWatch EventService에서 TimeZone을 UTC 시간에서 차이값으로 인식해서 0으로 변경함.
            xmlWriter.WriteElementString("TimeZone", "0");
            xmlWriter.WriteElementString("EventTime", StringUtils.GetDateTimeSecString(EventTime.ToUniversalTime()));
            //xmlWriter.WriteElementString("EventTimeUtc", StringUtils.GetDateTimeSecString(EventTime.ToUniversalTime()));
            xmlWriter.WriteElementString("LocalTime", StringUtils.GetDateTimeSecString(EventTime));

            xmlWriter.WriteElementString("AlarmStatus", DisplayEventType);

            xmlWriter.WriteStartElement("AlarmParameter");
            {
                xmlWriter.WriteElementString("ColorCode", ColorCode);
                xmlWriter.WriteStartElement("DisplayDataXml");
                {
                    xmlWriter.WriteStartElement("DisplayData");
                    {
                        xmlWriter.WriteStartElement("Data");
                        {
                            xmlWriter.WriteElementString("Title", "Type");
                            xmlWriter.WriteElementString("Value", DisplayEventType);
                        }
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Data");
                        {
                            xmlWriter.WriteElementString("Title", "Location");
                            xmlWriter.WriteElementString("Value", Location);
                        }
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Data");
                        {
                            xmlWriter.WriteElementString("Title", "Occurred");
                            xmlWriter.WriteElementString("Value", StringUtils.GetDateTimeSecString(EventTime));
                        }
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteElementString("IconUrl", IconUrl);
                xmlWriter.WriteElementString("Name", alarmName);
                xmlWriter.WriteElementString("Message", Message);

                xmlWriter.WriteElementString("PriorityLevel", "0");
                xmlWriter.WriteStartElement("PriorityLevelInfoList");
                {
                    //                            xmlWriter.WriteStartElement("PriorityLevelInfo");
                    //                            {
                    //                                xmlWriter.WriteElementString("ColorCode", "#FC0C0C");
                    //                                xmlWriter.WriteElementString("Description", newCdpAlarm.Message);
                    //                                xmlWriter.WriteElementString("Level", "0");
                    //                                xmlWriter.WriteElementString("Name", newCdpAlarm.Name);
                    //                                xmlWriter.WriteElementString("Value", newCdpAlarm.Name);
                    //                            }
                    //                            xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }
    }
}
