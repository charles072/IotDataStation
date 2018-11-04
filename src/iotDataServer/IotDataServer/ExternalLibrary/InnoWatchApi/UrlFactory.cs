using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace InnoWatchApi
{
    public static class UrlFactory
    {

        public static string GetUrl(string url, Dictionary<string, string> dictParam)
        {
            return GetUrl(url, "", dictParam);
        }
        public static string GetUrl(string baseUrl, string function, Dictionary<string, string> dictParam)
        {
            StringBuilder urlStringBuilder = new StringBuilder();
            urlStringBuilder.Append(baseUrl);
            urlStringBuilder.Append(function);

            if (dictParam != null && dictParam.Count > 0)
            {

                urlStringBuilder.Append("?");
                bool isFirstParam = true;
                foreach (KeyValuePair<string, string> keyValuePair in dictParam)
                {
                    if (isFirstParam)
                    {
                        isFirstParam = false;
                    }
                    else{
                        urlStringBuilder.Append("&");
                    }

                    urlStringBuilder.Append($"{HttpUtility.UrlEncode(keyValuePair.Key)}={HttpUtility.UrlEncode(keyValuePair.Value)}");
                }
            }

            return urlStringBuilder.ToString();
        }
        public static Uri GetUri(string url, Dictionary<string, string> dictParam)
        {
            return new Uri(GetUrl(url, dictParam));
        }

        public static Uri GetUri(string baseUrl, string function, Dictionary<string, string> dictParam)
        {
            return new Uri(GetUrl(baseUrl, function, dictParam));
        }


        /// <summary>
        /// A 타입 추가 URL.
        /// command + userId + password.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// 문자열을 반환한다.
        /// </returns>
        public static string GetUrlA(string address, string command, string userId, string password)
        {
            return string.Format("{0}{1}userId={2}&password={3}", address, command, userId, password);
        }

        /// <summary>
        /// C 타입 추가 URL.
        /// command + projectId + userId + classType.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="projectId">
        /// The project id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="classType">
        /// The class Type.
        /// </param>
        /// <returns>
        /// URL 문자열.
        /// </returns>
        public static string GetUrlC(string address, string command, string projectId, string userId, string classType)
        {
            return string.Format("{0}{1}projectId={2}&userId={3}&type={4}", address, command, projectId, userId, classType);
        }

        /// <summary>
        /// E 타입 URL. 
        /// command + projctId.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="projectId">
        /// The project Id.
        /// </param>
        /// <returns>
        /// URL 문자열.
        /// </returns>
        public static string GetUrlE(string address, string command, string projectId)
        {
            return string.Format("{0}{1}projectId={2}", address, command, projectId);
        }

        /// <summary>
        /// F 타입 URL. 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="command"></param>
        /// <param name="projectId"></param>
        /// <param name="clientId"></param>
        /// <returns> URL 문자열. </returns>
        public static string GetUrlF(string address, string command, string projectId, string clientId)
        {
            return string.Format("{0}{1}projectId={2}&clientId={3}", address, command, projectId, clientId);
        }

        /// <summary>
        /// G 타입 URL.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="command"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static string GetUrlG(string address, string command, string clientId)
        {
            return string.Format("{0}{1}clientId={2}", address, command, clientId);
        }

        /// <summary>
        /// B 타입 URL.
        /// command + TargetName + projectId + userId + CM/PB type
        /// </summary>
        /// <param name="address"> CT 주소</param>
        /// <param name="command"> 요청할 명령</param>
        /// <param name="name"> 해당 명령을 실행할 타겟</param>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">User Id</param>
        /// <param name="classType">CM/PB Type</param>
        /// <returns></returns>
        public static string GetUrlB(string address, string command, string name, string projectId, string userId, string classType)
        {
            return string.Format("{0}{1}projectId={3}&userId={4}&type={5}&name={2}", address, command, name, projectId, userId, classType);
        }

        /// <summary>
        /// D 타입 추가 URL.
        /// command + projectId + groupType.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="projectId">
        /// The project id.
        /// </param>
        /// <param name="groupType"> </param>
        /// <returns>
        /// URL 문자열.
        /// </returns>
        public static string GetUrlD(string address, string command, string projectId, string groupType)
        {
            return $"{address}{command}projectId={projectId}&group={groupType}";
        }

        public static string CheckAndFixHttpUrl(string url)
        {
            string ret = url;

            if (!ret.StartsWith("http://"))
            {
                ret = "http://" + ret;
            }

            if (!ret.EndsWith("/"))
            {
                ret = ret + "/";
            }

            return ret;
        }
    }
}
