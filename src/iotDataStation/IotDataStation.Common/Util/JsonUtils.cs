using Newtonsoft.Json.Linq;

namespace IotDataStation.Common.Util
{
    internal static class JsonUtils
    {
        public static T GetValueUsePath<T>(JObject jObject, string path, T defaultValue = default (T))
        {
            T result = defaultValue;

            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return defaultValue;
                }

                JToken lightValueJToken = jObject.SelectToken(@path);

                if (lightValueJToken != null)
                {
                    T lightStatusValue = lightValueJToken.Value<T>();
                    return lightStatusValue;
                }
            }
            catch
            {
                result = defaultValue;
            }

            return result;
        }

        public static string GetStringValue(JObject jObject, string key, string defaultValue = "")
        {
            string retValue = defaultValue;

            try
            {
                if (jObject != null)
                {
                    JToken jsonToken;
                    if (jObject.TryGetValue(key, out jsonToken))
                    {
                        retValue = jsonToken.Value<string>() ?? "";
                        retValue = retValue.Trim();
                    }
                }
            }
            catch
            {
                retValue = defaultValue;
            }

            return retValue;
        }

        public static int GetIntValue(JObject jObject, string key, int defaultValue = 0)
        {
            int retValue = defaultValue;

            try
            {
                if (jObject != null)
                {
                    JToken jsonToken;
                    if (jObject.TryGetValue(key, out jsonToken))
                    {
                        retValue = jsonToken.Value<int>();
                    }
                }

            }
            catch
            {
                retValue = defaultValue;
            }

            return retValue;
        }

        public static double GetDoubleValue(JObject jObject, string key, double defaultValue = 0)
        {
            double retValue = defaultValue;

            try
            {
                if (jObject != null)
                {
                    JToken jsonToken;
                    if (jObject.TryGetValue(key, out jsonToken))
                    {
                        if (jsonToken.Type != JTokenType.Null)
                        {
                            retValue = jsonToken.Value<double>();
                        }
                    }
                }

            }
            catch
            {
                retValue = defaultValue;
            }

            return retValue;
        }

        public static bool GetBoolValue(JObject jObject, string key, bool defaultValue = false)
        {
            bool retValue = defaultValue;

            try
            {
                if (jObject != null)
                {
                    JToken jsonToken;
                    if (jObject.TryGetValue(key, out jsonToken))
                    {
                        if (jsonToken.Type != JTokenType.Null)
                        {
                            retValue = jsonToken.Value<bool>();
                        }
                    }
                }

            }
            catch
            {
                retValue = defaultValue;
            }

            return retValue;
        }
    }
}
