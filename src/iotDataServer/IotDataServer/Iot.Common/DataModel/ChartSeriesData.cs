using System;
using System.Xml;
using Iot.Common.ClassLogger;
using Newtonsoft.Json.Linq;

namespace Iot.Common.DataModel
{
    public class ChartSeriesData
    {
        private static Iot.Common.ClassLogger.ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        private string[] _axisLabels;

        public ChartSeriesData(string label, string[] axisLabels, double[] values, string tag="")
        {
            Label = label;
            AxisLabels = axisLabels;
            Values = values;
            Tag = tag;
        }
        public ChartSeriesData(string label, double[] values, string tag = "")
        {
            Label = label;
            AxisLabels = null;
            Values = values;
            Tag = tag;
        }

        public string Label { get; }
        public string Tag { get; set; }

        public string[] AxisLabels
        {
            get => _axisLabels ?? new string[0];
            internal set => _axisLabels = value ?? new string[0];
        }

        public double[] Values { get; set; }

        public double this[int index] => GetValue(index);
        public double this[string name] => GetValue(name);


        public double GetValue(int index)
        {
            if (index < 0 || Values.Length <= index)
            {
                Logger.Error($"GetValue : No data in [{index}]");
                return double.MinValue;
            }
            return Values[index];
        }

        public double GetValue(string name)
        {
            int index = GetIndex(name);
            if (index < 0 || Values.Length <= index)
            {
                Logger.Error($"GetValue : No data in [{name}]");
                return double.MinValue;
            }
            return Values[index];
        }

        public string GetName(int index)
        {
            if (index >= 0 && index < AxisLabels.Length)
            {
                return AxisLabels[index];
            }
            return "";
        }

        public int GetIndex(string name)
        {
            for (int index = 0; index < AxisLabels.Length; index++)
            {
                if (String.Compare(AxisLabels[index], name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return index;
                }
            }
            return -1;
        }

        public JToken GetJsonObject()
        {
            JObject jObject = new JObject();
            jObject["Label"] = Label;
            jObject["Tag"] = Tag;
            JArray values = new JArray();
            foreach (var value in Values)
            {
                //values.Add(value.ToString("0.##"));
                values.Add(value);
            }
            jObject["Values"] = values;
            return jObject;
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Series");
            xmlWriter.WriteAttributeString("label", Label);
            xmlWriter.WriteAttributeString("tag", Tag);

            foreach (var value in Values)
            {
                xmlWriter.WriteStartElement("Set");
                xmlWriter.WriteAttributeString("value", value.ToString());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}