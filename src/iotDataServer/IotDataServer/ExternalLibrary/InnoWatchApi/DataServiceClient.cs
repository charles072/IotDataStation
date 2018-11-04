using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using InnoWatchApi.DataModel;
using InnoWatchApi.DataService;
using Iot.Common.ClassLogger;
using Iot.Common.Util;


namespace InnoWatchApi
{
    public class DataServiceClient
    {
        private static readonly ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        private readonly XmlHttpClient _xmlHttpClient;
        public string HostUrl { get; private set; }
        public string LoginUser { get; private set; }

        public bool IsLogon => LoginUser != "";

        public delegate void DataServiceApiRequestCompletedHandler(object sender, object userTag, DataServiceApiResult result);
        public event DataServiceApiRequestCompletedHandler RequestCompleted;


        public DataServiceClient(string hostUrl)
        {
            HostUrl = hostUrl;
            if (!HostUrl.EndsWith("/"))
            {
                HostUrl += "/";
            }
            _xmlHttpClient = new XmlHttpClient();
            //_xmlHttpClient.SetHearder(HttpRequestHeader.CacheControl, "no-store,no-cache");
            _xmlHttpClient.RequestCompleted += XmlHttpClientOnRequestCompleted;
        }

        //Login
        public DataServiceApiResult Login(string userId, string userPassword)
        {
            DataServiceApiRequest request = new DataServiceApiRequest(DataServiceApi.Login, new Dictionary<string, string>()
            {
                ["userId"] = userId,
                ["password"] = userPassword
            }, null, null);

            return RunApi(request);
        }

        public DataServiceApiRequest LoginAsync(string userId, string userPassword, object userTag = null, Action<DataServiceApiResult> postAction = null)
        {
            DataServiceApiRequest request = new DataServiceApiRequest(DataServiceApi.Login, new Dictionary<string, string>()
            {
                ["userId"] = userId,
                ["password"] = userPassword
            }, userTag, postAction);
            RunApiAsync(request);
            return request;
        }



        //GetVideoInformation
        public DataServiceApiResult GetVideoInformation(string projectId, string clientId)
        {
            DataServiceApiRequest request = new DataServiceApiRequest(DataServiceApi.GetVideoInformation, new Dictionary<string, string>()
            {
                ["projectId"] = projectId,
                ["clientId"] = clientId
            }, null, null);

            return RunApi(request);
        }

        //GetCameraGroupInfo
        public DataServiceApiResult GetCameraGroupInfo(string projectId)
        {
            DataServiceApiRequest request = new DataServiceApiRequest(DataServiceApi.GetCameraGroupInfo, new Dictionary<string, string>()
            {
                ["projectId"] = projectId,
                ["userId"] = LoginUser,
                ["type"] = "command"
            }, null, null);
            if (!IsLogon)
            {
                return new DataServiceApiResult(request, DataServiceApiResultCode.ErrorAuthorization, null, "", null);
            }
            return RunApi(request);
        }


        //GetCameraInfoList
        public CameraInfo[] GetCameraInfoList(string projectId = null)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return GetCameraInfoListAll();
            }
            return GetCameraInfoListByProject(projectId);
            
        }

        public CameraInfo[] GetCameraInfoListAll()
        {
            if (!IsLogon)
            {
                Logger.Error($"Error Authorization.");
                return null;
            }

            DataServiceApiRequest request = null;
            request = new DataServiceApiRequest(DataServiceApi.GetAllCameraInfo, new Dictionary<string, string>()
            {
                ["userId"] = LoginUser,
                ["type"] = "command"
            }, null, null);

            var res = RunApi(request);
            if (res.ResultCode != DataServiceApiResultCode.Success)
            {
                Logger.Error($"GetCameraInfo returns {res.ResultCode}");
                return null;
            }

            try
            {
                List<CameraInfo> cameraInfos = new List<CameraInfo>();
                XmlNodeList cameraInfoNodes = res.ResponseXml.SelectNodes("/CameraInfo/CameraInfo");
                if (cameraInfoNodes != null)
                {
                    foreach (XmlNode cameraInfoNode in cameraInfoNodes)
                    {
                        var cameraInfo = CameraInfo.CreateFrom(cameraInfoNode);
                        if (cameraInfo != null)
                        {
                            cameraInfos.Add(cameraInfo);
                        }
                    }
                }

                return cameraInfos.ToArray();
            }
            catch (Exception e)
            {
                Logger.Error(e, "GetCameraInfo Exception:");
                return null;
            }
        }


        public CameraInfo[] GetCameraInfoListByProject(string projectId)
        {
            if (!IsLogon)
            {
                Logger.Error($"Error Authorization.");
                return null;
            }
            DataServiceApiRequest request = null;
            request = new DataServiceApiRequest(DataServiceApi.GetCameraInfoList, new Dictionary<string, string>()
            {
                ["projectId"] = projectId,
                ["userId"] = LoginUser,
                ["type"] = "command"
            }, null, null);

            var res = RunApi(request);
            if (res.ResultCode != DataServiceApiResultCode.Success)
            {
                Logger.Error($"GetCameraInfo returns {res.ResultCode}");
                return null;
            }

            try
            {
                List<CameraInfo> cameraInfos = new List<CameraInfo>();
                XmlNodeList cameraInfoNodes = res.ResponseXml.SelectNodes("/NewDataSet/Table");
                if (cameraInfoNodes != null)
                {
                    foreach (XmlNode cameraInfoNode in cameraInfoNodes)
                    {
                        var cameraInfo = CameraInfo.CreateFrom(cameraInfoNode);
                        if (cameraInfo != null)
                        {
                            cameraInfos.Add(cameraInfo);
                        }
                    }
                }

                return cameraInfos.ToArray();
            }
            catch (Exception e)
            {
                Logger.Error(e, "GetCameraInfo Exception:");
                return null;
            }
        }

        public DataServiceApiResultCode UpdateCameraInfo(CameraInfo cameraInfo)
        {
            if (cameraInfo == null)
            {
                return DataServiceApiResultCode.ErrorBadRequest;
            }
            if (!IsLogon)
            {
                Logger.Error($"Error Authorization.");
                return DataServiceApiResultCode.ErrorAuthorization;
            }

            try
            {
                DataTable updateCameraInfoDataTable = cameraInfo.ToDataTable();
                DataSet updateCameraInfoDataSet = new DataSet("CameraInfo");
                updateCameraInfoDataSet.Tables.Add(updateCameraInfoDataTable);

                string postData = "";
                using (var stringWriteData = new StringWriter())
                {
                    updateCameraInfoDataSet.WriteXml(stringWriteData, XmlWriteMode.WriteSchema);

                    postData = stringWriteData.ToString();
                }
                var param = new RequestParameter
                {
                    Url = $"{HostUrl}{DataServiceApi.UpdateCameraInfo}",
                    EncodingOption = "UTF-8",
                    PostMessage = postData
                };
                return (DataServiceHandler.RequestToBoolResponse(param, true)) ? DataServiceApiResultCode.Success: DataServiceApiResultCode.Failed;
            }
            catch (Exception e)
            {
                Logger.Error(e, "GetCameraInfo Exception:");
                return DataServiceApiResultCode.ErrorInternal;
            }
        }

        #region base


        private DataServiceApiResult RunApi(DataServiceApiRequest request)
        {
            XmlHttpResult result = _xmlHttpClient.Get(HostUrl, request.Api, request.Params);

            return BuildApiResult(request, result);
        }

        private DataServiceApiResult BuildApiResult(DataServiceApiRequest apiRequest, XmlHttpResult httpResult)
        {
            DataServiceApiResult apiResult = null;
            if (apiRequest.Api == DataServiceApi.Login)
            {
                LoginUser = "";
            }
            if (httpResult.ResultCode == XmlHttpResultCode.Success)
            {
                DataServiceApiResultCode resultCode = DataServiceApiResultCode.Failed;
                if (apiRequest.Api == DataServiceApi.Login)
                {

                    var statusNode = httpResult.ResponseXmlRoot.SelectSingleNode("Status");
                    var resultvalue = XmlUtils.GetXmlAttributeValue(statusNode, "value");
                    switch (resultvalue)
                    {
                        case "success":
                            resultCode = DataServiceApiResultCode.Success;
                            LoginUser = apiRequest.Params["userId"];
                            break;
                        case "none userid":
                            resultCode = DataServiceApiResultCode.ErrorCannotFindUser;
                            break;
                        case "incorrect password":
                            resultCode = DataServiceApiResultCode.ErrorIncorrectPassword;
                            break;
                        default:
                            resultCode = DataServiceApiResultCode.Failed;
                            break;
                    }
                }
                else
                {
                    resultCode = DataServiceApiResultCode.Success;
                }
                apiResult = new DataServiceApiResult(apiRequest, resultCode, httpResult.Error, httpResult.ResponseString, httpResult.ResponseXmlRoot);
            }
            else
            {
                apiResult = new DataServiceApiResult(apiRequest, GetDataServiceApiResultCode(httpResult.ResultCode), httpResult.Error, httpResult.ResponseString, httpResult.ResponseXmlRoot);

            }

            return apiResult;
        }

        private DataServiceApiResultCode GetDataServiceApiResultCode(XmlHttpResultCode httpResultCode)
        {
            DataServiceApiResultCode resultCode;

            switch (httpResultCode)
            {
                case XmlHttpResultCode.Success:
                    resultCode = DataServiceApiResultCode.Success;
                    break;
                case XmlHttpResultCode.Canceled:
                    resultCode = DataServiceApiResultCode.Canceled;
                    break;


                case XmlHttpResultCode.ErrorXmlParsing:
                    resultCode = DataServiceApiResultCode.ErrorDataParsing;
                    break;

                default:
                    resultCode = DataServiceApiResultCode.ErrorProtocol;
                    break;
            }

            return resultCode;
        }

        public void RunApiAsync(DataServiceApiRequest request)
        {
            _xmlHttpClient.GetAsync(HostUrl, request.Api, request.Params, request);
        }

        private void XmlHttpClientOnRequestCompleted(object sender, object userToken, XmlHttpResult httpResult)
        {
            var apiRequest = userToken as DataServiceApiRequest;

            OnRequestCompleted(BuildApiResult(apiRequest, httpResult));
        }


        protected virtual void OnRequestCompleted(DataServiceApiResult response)
        {
            response.Request.PostAction?.Invoke(response);
            RequestCompleted?.Invoke(this, response.Request.UserTag, response);
        }


        #endregion
    }
}
