using System.Xml;
using Newtonsoft.Json.Linq;

namespace Iot.Common.DataModel
{
    public class PinObject
    {
        public PinPoint PinPoint { get; set; }
        public PinSize PinSize { get; set; }
        public PinAlignment PinAlignment { get; set; }
        public PinPoint LeftTopPoint => GetLeftTopPoint();

        public PinObject(PinPoint pinPoint = null, PinSize pinSize = null, PinAlignment pinAlignment = PinAlignment.CenterMiddle)
        {
            PinPoint = pinPoint ?? new PinPoint();
            PinSize = pinSize ?? new PinSize();
            PinAlignment = pinAlignment;
        }
        public PinObject Clone()
        {
            PinObject cloneObject = new PinObject(PinPoint, PinSize, PinAlignment);
            return cloneObject;
        }

        private PinPoint GetLeftTopPoint()
        {
            PinPoint point = new PinPoint(PinPoint.X, PinPoint.Y);

            if (PinSize.IsEmpty)
            {
                return point;
            }

            switch (PinAlignment)
            {
                case PinAlignment.LeftTop:
                    break;
                case PinAlignment.CenterTop:
                    point.X -= PinSize.Width / 2;
                    break;
                case PinAlignment.RightTop:
                    point.X -= PinSize.Width;
                    break;
                case PinAlignment.LeftMiddle:
                    point.Y -= PinSize.Height / 2;
                    break;
                case PinAlignment.CenterMiddle:
                    point.X -= PinSize.Width / 2;
                    point.Y -= PinSize.Height / 2;
                    break;
                case PinAlignment.RightMiddle:
                    point.X -= PinSize.Width;
                    point.Y -= PinSize.Height / 2;
                    break;
                case PinAlignment.LeftBottom:
                    point.Y -= PinSize.Height;
                    break;
                case PinAlignment.CenterBottom:
                    point.X -= PinSize.Width / 2;
                    point.Y -= PinSize.Height;
                    break;
                case PinAlignment.RightBottom:
                    point.X -= PinSize.Width;
                    point.Y -= PinSize.Height;
                    break;
            }

            return point;
        }

        public JObject ToJObject()
        {
            PinPoint leftTop = LeftTopPoint;

            JObject pinObject = new JObject();

            pinObject["x"] = leftTop.X;
            pinObject["y"] = leftTop.Y;
            pinObject["width"] = PinSize.Width;
            pinObject["height"] = PinSize.Height;

            pinObject["pinX"] = PinPoint.X;
            pinObject["pinY"] = PinPoint.Y;
            pinObject["pinAlignment"] = PinAlignment.ToString();

            return pinObject;
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Pin");

            PinPoint leftTop = LeftTopPoint;

            xmlWriter.WriteAttributeString("x", $"{leftTop.X}");
            xmlWriter.WriteAttributeString("y", $"{leftTop.Y}");
            xmlWriter.WriteAttributeString("width", $"{PinSize.Width}");
            xmlWriter.WriteAttributeString("height", $"{PinSize.Height}");

            xmlWriter.WriteAttributeString("pinX", $"{PinPoint.X}");
            xmlWriter.WriteAttributeString("pinY", $"{PinPoint.Y}");
            xmlWriter.WriteAttributeString("pinAlignment", $"{PinAlignment}");

            xmlWriter.WriteEndElement();
        }
    }
}