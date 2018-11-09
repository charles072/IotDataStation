using System;
using System.Reflection;

namespace IotDataServer.Common.Util
{
    public static class ReflectionUtils
    {
        public static bool HasParameterlessConstructor(this Type type)
        {
            return (type.GetConstructor(Type.EmptyTypes) != null);
        }

        public static bool CanInvoke(this MethodInfo method)
        {
            if (method.IsStatic) return true;
            if (method.IsAbstract) return false;

            var type = method.ReflectedType;
            if (type == null) return false;
            if (!type.IsClass) return false;
            if (type.IsAbstract) return false;

            return (true);
        }

        public static string GetPropertyStringValue(Type type, object instance, string name, string defaultValue = "")
        {
            string resValue = defaultValue;

            var propertyInfo = type.GetProperty(name);
            if (propertyInfo != null)
            {
                resValue = propertyInfo.GetValue(instance, null)?.ToString() ?? "";
            }
            return resValue;
        }
        public static int GetPropertyIntValue(Type type, object instance, string name, int defaultValue = 0)
        {
            int resValue = defaultValue;

            var propertyInfo = type.GetProperty(name);
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(instance, null);
                try
                {
                    resValue = Convert.ToInt32(value);
                }
                catch
                {
                    resValue = defaultValue;
                }
            }
            return resValue;
        }

        public static bool IsInteger(object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong;
        }
        public static bool IsNumber(object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }

    }
}
