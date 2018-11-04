using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Xml.Serialization;
using Iot.Common.Util;

namespace InnoWatchApi.DataModel
{
    /// <summary>
    /// 지정된 카메라 해상도 정보 모음 클래스.
    /// </summary>
    [XmlRoot("CameraInfo")]
    public class CameraInfo
    {
        public string CameraID { get; set; }
        public string CameraDescription { get; set; }
        public string DefaultProjectCameraID { get; set; }
        public string DefaultProjectCameraName { get; set; }
        public string CameraIP { get; set; }
        public int CameraPort { get; set; }
        public string ConnectMode { get; set; }
        public string MediaServerID { get; set; }
        public string MediaServerExtraParams { get; set; }
        public string RtspID { get; set; }
        public string RtspPassword { get; set; }
        public string ControlType { get; set; }
        public string CameraControlSettingID { get; set; }
        public string PtzConnectionInfoID { get; set; }
        public string ImageClippingArea { get; set; }
       
        public string RecordCameraNumber { get; set; }
        public bool Enable { get; set; }
        public bool ShowAudioLevel { get; set; }
        public string AnalogCameraMemo { get; set; }
        public string MediaServerPosition { get; set; }
        public string Vendor { get; set; }
        public string RdsHost { get; set; }
        public int RdsPort { get; set; }
        public string MultiCameraControllerId { get; set; }
        public string ResizingCodec { get; set; }
        public string Channel { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DemolitionDate { get; set; }
        public string ProjectCameraID { get; set; }
        public string ProjectCameraName { get; set; }
        public static CameraInfo CreateFrom(XmlNode cameraNode)
        {
            CameraInfo camera = null;
            if (cameraNode == null)
            {
                return null;
            }
            try
            {
                camera = new CameraInfo();
                camera.CameraID = XmlUtils.GetXmlNodeInnerText(cameraNode, "CameraID");
                camera.CameraDescription = XmlUtils.GetXmlNodeInnerText(cameraNode, "CameraDescription");
                camera.ProjectCameraID = XmlUtils.GetXmlNodeInnerText(cameraNode, "ProjectCameraID");
                camera.ProjectCameraName = XmlUtils.GetXmlNodeInnerText(cameraNode, "ProjectCameraName");

                camera.DefaultProjectCameraID = StringUtils.GetStringValue(XmlUtils.GetXmlNodeInnerText(cameraNode, "DefaultProjectCameraID"), camera.ProjectCameraID);
                camera.DefaultProjectCameraName = StringUtils.GetStringValue(XmlUtils.GetXmlNodeInnerText(cameraNode, "DefaultProjectCameraName"), camera.ProjectCameraName);
                camera.CameraIP = XmlUtils.GetXmlNodeInnerText(cameraNode, "CameraIP");
                camera.CameraPort = StringUtils.GetIntValue(XmlUtils.GetXmlNodeInnerText(cameraNode, "CameraPort"));
                camera.ConnectMode = XmlUtils.GetXmlNodeInnerText(cameraNode, "ConnectMode");
                camera.MediaServerID = XmlUtils.GetXmlNodeInnerText(cameraNode, "MediaServerID");
                camera.MediaServerExtraParams = XmlUtils.GetXmlNodeInnerText(cameraNode, "MediaServerExtraParams");
                camera.RtspID = XmlUtils.GetXmlNodeInnerText(cameraNode, "RtspID");
                camera.RtspPassword = XmlUtils.GetXmlNodeInnerText(cameraNode, "RtspPassword");
                camera.ControlType = XmlUtils.GetXmlNodeInnerText(cameraNode, "ControlType");
                camera.CameraControlSettingID = XmlUtils.GetXmlNodeInnerText(cameraNode, "CameraControlSettingID");
                camera.PtzConnectionInfoID = XmlUtils.GetXmlNodeInnerText(cameraNode, "PtzConnectionInfoID");
                camera.ImageClippingArea = XmlUtils.GetXmlNodeInnerText(cameraNode, "ImageClippingArea");
                camera.RecordCameraNumber = XmlUtils.GetXmlNodeInnerText(cameraNode, "RecordCameraNumber");
                camera.Enable = StringUtils.GetValue<bool>(XmlUtils.GetXmlNodeInnerText(cameraNode, "Enable"));
                camera.ShowAudioLevel = StringUtils.GetValue<bool>(XmlUtils.GetXmlNodeInnerText(cameraNode, "ShowAudioLevel"));
                camera.AnalogCameraMemo = XmlUtils.GetXmlNodeInnerText(cameraNode, "AnalogCameraMemo");
                camera.MediaServerPosition = XmlUtils.GetXmlNodeInnerText(cameraNode, "MediaServerPosition");
                camera.Vendor = XmlUtils.GetXmlNodeInnerText(cameraNode, "Vendor");
                camera.RdsHost = XmlUtils.GetXmlNodeInnerText(cameraNode, "RdsHost");
                camera.RdsPort = StringUtils.GetIntValue(XmlUtils.GetXmlNodeInnerText(cameraNode, "RdsPort"));
                camera.MultiCameraControllerId = XmlUtils.GetXmlNodeInnerText(cameraNode, "MultiCameraControllerId");
                camera.ResizingCodec = XmlUtils.GetXmlNodeInnerText(cameraNode, "ResizingCodec");
                camera.Channel = XmlUtils.GetXmlNodeInnerText(cameraNode, "Channel");
                camera.Latitude = XmlUtils.GetXmlNodeInnerText(cameraNode, "Latitude");
                camera.Longitude = XmlUtils.GetXmlNodeInnerText(cameraNode, "Longitude");
                camera.DemolitionDate = XmlUtils.GetXmlNodeInnerText(cameraNode, "DemolitionDate");
            }
            catch (Exception)
            {
                camera = null;
            }
            
            return camera;
        }

        public DataTable ToDataTable()
        {
            var dataTable = new DataTable("Camera");
            var columnCameraId = new DataColumn("CameraID", typeof(string));
            var columnCameraDescription = new DataColumn("CameraDescription", typeof(string));
            var columnDefaultProjectCameraID = new DataColumn("DefaultProjectCameraID", typeof(string));
            var columnDefaultProjectCameraName = new DataColumn("DefaultProjectCameraName", typeof(string));
            var columnCameraIp = new DataColumn("CameraIP", typeof(string));
            var columnCameraPort = new DataColumn("CameraPort", typeof(int));
            var columnConnectMode = new DataColumn("ConnectMode", typeof(string));
            var columnMediaServerId = new DataColumn("MediaServerID", typeof(string));
            var columnMediaServerExtraParams = new DataColumn("MediaServerExtraParams", typeof(string));
            var columnRtspId = new DataColumn("RtspID", typeof(string));
            var columnRtspPassword = new DataColumn("RtspPassword", typeof(string));
            var columnControlType = new DataColumn("ControlType", typeof(string));
            var columnCameraControlSettingId = new DataColumn("CameraControlSettingID", typeof(string));
            var columnPtzConnectionInfoId = new DataColumn("PtzConnectionInfoID", typeof(string));
            var columnImageClippingArea = new DataColumn("ImageClippingArea", typeof(string));
            var columnEnable = new DataColumn("Enable", typeof(bool));
            var columnShowAudioLevel = new DataColumn("ShowAudioLevel", typeof(bool));
            var columnAnalogCameraMemo = new DataColumn("AnalogCameraMemo", typeof(string));
            var columnMediaServerPosition = new DataColumn("MediaServerPosition", typeof(int));
            var columnVendor = new DataColumn("Vendor", typeof(int));
            var columnRecordCameraNumber = new DataColumn("RecordCameraNumber", typeof(int));
            var columnRdsHost = new DataColumn("RdsHost", typeof(string));
            var columnRdsPort = new DataColumn("RdsPort", typeof(int));
            var columnMultiCameraControllerId = new DataColumn("MultiCameraControllerId", typeof(string));
            var columnResizingCodec = new DataColumn("ResizingCodec", typeof(string));
            var columnCameraChannel = new DataColumn("Channel", typeof(string));
            var columnLatitude = new DataColumn("Latitude", typeof(string));
            var columnLongitude = new DataColumn("Longitude", typeof(string));
            var columnDemolitionDate = new DataColumn("DemolitionDate", typeof(string));

            dataTable.Columns.Add(columnCameraId);
            dataTable.Columns.Add(columnCameraDescription);
            dataTable.Columns.Add(columnDefaultProjectCameraID);
            dataTable.Columns.Add(columnDefaultProjectCameraName);
            dataTable.Columns.Add(columnCameraIp);
            dataTable.Columns.Add(columnCameraPort);
            dataTable.Columns.Add(columnConnectMode);
            dataTable.Columns.Add(columnMediaServerId);
            dataTable.Columns.Add(columnMediaServerExtraParams);
            dataTable.Columns.Add(columnRtspId);
            dataTable.Columns.Add(columnRtspPassword);
            dataTable.Columns.Add(columnControlType);
            dataTable.Columns.Add(columnCameraControlSettingId);
            dataTable.Columns.Add(columnPtzConnectionInfoId);
            dataTable.Columns.Add(columnImageClippingArea);
            dataTable.Columns.Add(columnEnable);
            dataTable.Columns.Add(columnShowAudioLevel);
            dataTable.Columns.Add(columnAnalogCameraMemo);
            dataTable.Columns.Add(columnMediaServerPosition);
            dataTable.Columns.Add(columnVendor);
            dataTable.Columns.Add(columnRecordCameraNumber);
            dataTable.Columns.Add(columnRdsHost);
            dataTable.Columns.Add(columnRdsPort);
            dataTable.Columns.Add(columnMultiCameraControllerId);
            dataTable.Columns.Add(columnResizingCodec);
            dataTable.Columns.Add(columnCameraChannel);
            dataTable.Columns.Add(columnLatitude);
            dataTable.Columns.Add(columnLongitude);
            dataTable.Columns.Add(columnDemolitionDate);

            var newRow = dataTable.NewRow();
            newRow["CameraID"] = StringUtils.ConvertToDbTypeString(CameraID);
            newRow["CameraDescription"] = StringUtils.ConvertToDbTypeString(CameraDescription);
            newRow["DefaultProjectCameraID"] = StringUtils.ConvertToDbTypeString(DefaultProjectCameraID);
            newRow["DefaultProjectCameraName"] = StringUtils.ConvertToDbTypeString(DefaultProjectCameraName);
            newRow["CameraIP"] = StringUtils.ConvertToDbTypeString(CameraIP);
            newRow["CameraPort"] = CameraPort;
            newRow["ConnectMode"] = StringUtils.ConvertToDbTypeString(ConnectMode);
            newRow["MediaServerID"] = StringUtils.ConvertToDbTypeString(MediaServerID);
            newRow["MediaServerExtraParams"] = StringUtils.ConvertToDbTypeString(MediaServerExtraParams);
            newRow["RtspID"] = StringUtils.ConvertToDbTypeString(RtspID);
            newRow["RtspPassword"] = StringUtils.ConvertToDbTypeString(RtspPassword);
            newRow["ControlType"] = StringUtils.ConvertToDbTypeString(ControlType);
            newRow["CameraControlSettingID"] = StringUtils.ConvertToDbTypeString(CameraControlSettingID);
            newRow["PtzConnectionInfoID"] = StringUtils.ConvertToDbTypeString(PtzConnectionInfoID);
            newRow["ImageClippingArea"] = StringUtils.ConvertToDbTypeString(ImageClippingArea);
            /*
            if (string.IsNullOrWhiteSpace(imageClippingArea))
            {
                newRow["ImageClippingArea"] = StringUtils.ConvertToDbTypeString("0,0,0,0");
            }
            else
            {
                newRow["ImageClippingArea"] = StringUtils.ConvertToDbTypeString(ImageClippingArea);
            }*/

            if (string.IsNullOrWhiteSpace(RecordCameraNumber) || RecordCameraNumber.Equals("None"))
            {
                newRow["RecordCameraNumber"] = StringUtils.ConvertToDbTypeString(null);
            }
            else
            {
                newRow["RecordCameraNumber"] = StringUtils.ConvertToDbTypeString(RecordCameraNumber);
            }

            newRow["Enable"] = Enable;
            newRow["ShowAudioLevel"] = ShowAudioLevel;
            newRow["AnalogCameraMemo"] = StringUtils.ConvertToDbTypeString(AnalogCameraMemo);
            newRow["MediaServerPosition"] = StringUtils.ConvertToDbTypeString(MediaServerPosition);
            newRow["Vendor"] = StringUtils.ConvertToDbTypeString(Vendor);
            newRow["RdsHost"] = StringUtils.ConvertToDbTypeString(RdsHost);
            newRow["RdsPort"] = RdsPort;
            newRow["MultiCameraControllerId"] = StringUtils.ConvertToDbTypeString(MultiCameraControllerId);
            newRow["ResizingCodec"] = StringUtils.ConvertToDbTypeString(ResizingCodec);
            newRow["Channel"] = StringUtils.ConvertToDbTypeString(Channel);
            newRow["Latitude"] = StringUtils.ConvertToDbTypeString(Latitude);
            newRow["Longitude"] = StringUtils.ConvertToDbTypeString(Longitude);
            newRow["DemolitionDate"] = StringUtils.ConvertToDbTypeString(DemolitionDate);

            dataTable.Rows.Add(newRow);

            return dataTable;
        }
    }
}
