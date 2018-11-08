using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Iot.Common.ClassLogger;
using Iot.Common.IO;
using Iot.Common.Util;
using Newtonsoft.Json.Linq;

namespace Iot.Common.DataModel
{
    public class ChartDataSet
    {
        private static Iot.Common.ClassLogger.ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string Name { get; private set; }
        public string AxisKind { get; set; } = "";
        public string AxisUnitName { get; set; } = "";
        public string YAxisUnitName { get; set; } = "";
        public string[] AxisLabels { get; private set; } = new string[0];
        public List<ChartSeriesData> Series { get; set; } = new List<ChartSeriesData>();

        public ChartDataSet(string name, string filename)
        {
            Init(name);
            try
            {
                using (TextFieldReader textFieldReader = new TextFieldReader(filename))
                {
                    bool isFirst = true;

                    string[] fields;
                    while ((fields = textFieldReader.ReadFields()) != null)
                    {
                        if (isFirst)
                        {
                            if (fields.Length >= 2)
                            {
                                AxisLabels = fields.SubArray(2, fields.Length - 2);
                            }
                            isFirst = false;
                        }
                        else
                        {
                            try
                            {
                                string label = fields[0];
                                string tag = fields[1];
                                List<double> values = new List<double>();
                                for (int i = 2; i < fields.Length; i++)
                                {
                                    if (double.TryParse(fields[i], out double value))
                                    {
                                        values.Add(value);
                                    }
                                    else
                                    {
                                        throw new ArgumentException($"'{fields[i]}' is not double.");
                                    }
                                }
                                Series.Add(new ChartSeriesData(label, AxisLabels, values.ToArray(), tag));
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e, "Read fields Exception:");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "StatsDataSet:");
            }
        }

        public ChartDataSet(string name, string[] axisLabels)
        {
            Init(name);
            if (axisLabels != null)
            {
                AxisLabels = axisLabels;
            }
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

        private void Init(string name)
        {
            Name = name;
            Id = "";
            AxisKind = "";
            AxisLabels = new string[0];
        }

        public bool Write(string filename)
        {
            bool res = false;
            try
            {
                string folder = Path.GetDirectoryName(Path.GetFullPath(filename));
                Directory.CreateDirectory(folder);
                using (TextFieldWriter writer = new TextFieldWriter(filename))
                {
                    List<string> headers = new List<string>();
                    headers.Add("Label");
                    headers.Add("Tag");
                    headers.AddRange(AxisLabels);
                    writer.WriteFields(headers.ToArray());

                    foreach (ChartSeriesData row in Series)
                    {
                        List<string> fields = new List<string>();
                        fields.Add(row.Label);
                        fields.Add(row.Tag);
                        foreach (double value in row.Values)
                        {
                            fields.Add(value.ToString("0.##"));
                        }

                        writer.WriteFields(fields.ToArray());
                    }
                    res = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Write:");
                res = false;
            }
            return res;
        }

        public JObject GetJsonObject()
        {
            JObject jObject = new JObject();
            jObject["ChartName"] = Name;
            jObject["ChartId"] = Id;
            jObject["ChartType"] = Type;
            jObject["AxisKind"] = AxisKind;
            jObject["AxisUnitName"] = AxisUnitName;
            jObject["YAxisUnitName"] = YAxisUnitName;
            JArray axisLabels = new JArray();
            foreach (string axisLabel in AxisLabels)
            {
                axisLabels.Add(axisLabel);
            }
            jObject["AxisLabels"] = axisLabels;

            JArray series = new JArray();
            foreach (ChartSeriesData seriesData in Series)
            {
                series.Add(seriesData.GetJsonObject());
            }
            jObject["Series"] = series;
            return jObject;
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Chart");
            xmlWriter.WriteAttributeString("chartName", Name);
            xmlWriter.WriteAttributeString("chartId", Id);
            xmlWriter.WriteAttributeString("chartType", Type);
            xmlWriter.WriteAttributeString("axisKind", AxisKind);
            xmlWriter.WriteAttributeString("axisUnitName", AxisUnitName);
            xmlWriter.WriteAttributeString("yAxisUnitName", YAxisUnitName);

            xmlWriter.WriteStartElement("AxisLabels");
            foreach (string axisLabel in AxisLabels)
            {
                xmlWriter.WriteStartElement("Label");
                xmlWriter.WriteAttributeString("value", axisLabel);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("SeriesList");
            foreach (ChartSeriesData seriesData in Series)
            {
                seriesData.WriteXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        public void Add(ChartSeriesData chartSeries)
        {
            chartSeries.AxisLabels = AxisLabels;


            Series.Add(chartSeries);
        }


        public bool TryGetSeries(string label, out ChartSeriesData chartSeries)
        {
            chartSeries = null;
            foreach (ChartSeriesData chartSeriesData in Series)
            {
                if (chartSeriesData.Label == label)
                {
                    chartSeries = chartSeriesData;
                    return true;
                }
            }
            return false;
        }

        
    }
}
