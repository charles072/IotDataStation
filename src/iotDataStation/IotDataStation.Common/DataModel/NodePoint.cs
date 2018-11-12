using System;
using System.Xml;
using IotDataStation.Common.Util;
using IotDataStation.Common.Util;
using Newtonsoft.Json.Linq;
using NLog;

namespace IotDataStation.Common.DataModel
{
    public class NodePoint
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public double X { get; set; }
        public double Y { get; set; }

        public NodePoint(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        public JObject ToJObject()
        {
            JObject pointObject = new JObject();

            pointObject["x"] = X;
            pointObject["y"] = Y;
            return pointObject;
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Point");

            xmlWriter.WriteAttributeString("x", $"{X}");
            xmlWriter.WriteAttributeString("y", $"{Y}");

            xmlWriter.WriteEndElement();
        }

        public static NodePoint CreateFrom(JObject pointJObject)
        {
            NodePoint pointObject = null;
            if (pointJObject == null)
            {
                return null;
            }

            try
            {
                double x = JsonUtils.GetDoubleValue(pointJObject, "x");
                double y = JsonUtils.GetDoubleValue(pointJObject, "y");
                pointObject = new NodePoint(x, y);
            }
            catch (Exception e)
            {
                pointObject = null;
                Logger.Error(e, "CreateFrom(pointObject):");
            }
            return pointObject;
        }

        public NodePoint Clone()
        {
            return new NodePoint(X, Y);
        }
    }
}
