using System;
using Iot.Common.Util;

namespace IotDataServer.Common.Notification
{
    public class NotificationMessage
    {
        public string Sender { get; set; }
        public string Target { get; set; }
        public string MessageId { get; set; }
        public string TimeStamp { get; set; }
        public string MessageType { get; set; }
        public string MessageFunction { get; set; }
        public string Message { get; set; }


        public NotificationMessage(string sender, string target, string messageType, string messageFunction, string message)
        {
            Sender = sender;
            Target = target;
            MessageId = Guid.NewGuid().ToString("N");
            TimeStamp = StringUtils.GetDateTimeMillisecString();
            MessageType = messageType;
            MessageFunction = messageFunction;
            Message = message;
        }


    }
}
