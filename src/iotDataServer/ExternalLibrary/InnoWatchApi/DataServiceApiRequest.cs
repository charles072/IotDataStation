using System;
using System.Collections.Generic;

namespace InnoWatchApi
{
    public class DataServiceApiRequest
    {
        public String Api { get; private set; }
        public Dictionary<string, string> Params { get; private set; }

        public object UserTag { get; private set; }
        public Action<DataServiceApiResult> PostAction { get; private set; }

        public DataServiceApiRequest(String api, Dictionary<string, string> dicParam = null, object userTag = null, Action<DataServiceApiResult> postAction=null)
        {
            Api = api;
            Params = dicParam ?? new Dictionary<string, string>();
            UserTag = userTag;
            PostAction = postAction;
        }
        public DataServiceApiRequest(String api, object userTag) : this (api, null, userTag)
        {
        }
        public DataServiceApiRequest(String api, Dictionary<string, string> dicParam , Action<DataServiceApiResult> postAction) : this(api, dicParam, null, postAction)
        {
        }
        public DataServiceApiRequest(String api, Action<DataServiceApiResult> postAction) : this(api, null, null, postAction)
        {
        }
    }
}