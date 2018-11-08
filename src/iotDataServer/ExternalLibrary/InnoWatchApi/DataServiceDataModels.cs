using System.Data;
using Iot.Common.Util;

namespace InnoWatchApi
{
    public class DataServiceDataModels
    {
        public static DataTable DataTableToCameraInfo(
            string cameraId,
            string cameraDescription,
            string defaultProjectCameraID,
            string defaultProjectCameraName,
            string cameraIp,
            int? cameraPort,
            string connectMode,
            string mediaServerId,
            string mediaServerExtraParams,
            string rtspId,
            string rtspPassword,
            string controlType,
            string cameraControlSettingId,
            string ptzConnectionInfoId,
            string imageClippingArea,
            bool enable,
            bool showAudioLevel,
            string analogCameraMemo,
            int? mediaServerPosition,
            int? vendor,
            string recordCameraNumber,
            string rdsHost,
            int? rdsPort,
            string multiCameraControllerId,
            string resizingCodec,
            string channel,
            string latitude,
            string longitude,
            string demolitionDate)
        {
            var dataTable = new DataTable();
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
            newRow["CameraID"] = StringUtils.ConvertToDbTypeString(cameraId);
            newRow["CameraDescription"] = StringUtils.ConvertToDbTypeString(cameraDescription);
            newRow["DefaultProjectCameraID"] = StringUtils.ConvertToDbTypeString(defaultProjectCameraID);
            newRow["DefaultProjectCameraName"] = StringUtils.ConvertToDbTypeString(defaultProjectCameraName);
            newRow["CameraIP"] = StringUtils.ConvertToDbTypeString(cameraIp);
            newRow["CameraPort"] = cameraPort;
            newRow["ConnectMode"] = StringUtils.ConvertToDbTypeString(connectMode);
            newRow["MediaServerID"] = StringUtils.ConvertToDbTypeString(mediaServerId);
            newRow["MediaServerExtraParams"] = StringUtils.ConvertToDbTypeString(mediaServerExtraParams);
            newRow["RtspID"] = StringUtils.ConvertToDbTypeString(rtspId);
            newRow["RtspPassword"] = StringUtils.ConvertToDbTypeString(rtspPassword);
            newRow["ControlType"] = StringUtils.ConvertToDbTypeString(controlType);
            newRow["CameraControlSettingID"] = StringUtils.ConvertToDbTypeString(cameraControlSettingId);
            newRow["PtzConnectionInfoID"] = StringUtils.ConvertToDbTypeString(ptzConnectionInfoId);

            if (string.IsNullOrWhiteSpace(imageClippingArea))
            {
                newRow["ImageClippingArea"] = StringUtils.ConvertToDbTypeString("0,0,0,0");
            }
            else
            {
                newRow["ImageClippingArea"] = StringUtils.ConvertToDbTypeString(imageClippingArea);
            }

            if (string.IsNullOrWhiteSpace(recordCameraNumber) || recordCameraNumber.Equals("None"))
            {
                newRow["RecordCameraNumber"] = StringUtils.ConvertToDbTypeString(null);
            }
            else
            {
                newRow["RecordCameraNumber"] = StringUtils.ConvertToDbTypeString(recordCameraNumber);
            }

            newRow["Enable"] = enable;
            newRow["ShowAudioLevel"] = showAudioLevel;
            newRow["AnalogCameraMemo"] = StringUtils.ConvertToDbTypeString(analogCameraMemo);
            newRow["MediaServerPosition"] = StringUtils.ConvertToDbTypeString(mediaServerPosition.ToString());
            newRow["Vendor"] = StringUtils.ConvertToDbTypeString(vendor.ToString());
            newRow["RdsHost"] = StringUtils.ConvertToDbTypeString(rdsHost);
            newRow["RdsPort"] = StringUtils.ConvertToDbTypeString(rdsPort.ToString());
            newRow["MultiCameraControllerId"] = StringUtils.ConvertToDbTypeString(multiCameraControllerId);
            newRow["ResizingCodec"] = StringUtils.ConvertToDbTypeString(resizingCodec);
            newRow["Channel"] = StringUtils.ConvertToDbTypeString(channel);
            newRow["Latitude"] = StringUtils.ConvertToDbTypeString(latitude);
            newRow["Longitude"] = StringUtils.ConvertToDbTypeString(longitude);
            newRow["DemolitionDate"] = StringUtils.ConvertToDbTypeString(demolitionDate);

            dataTable.Rows.Add(newRow);

            return dataTable;
        }
    }
}
