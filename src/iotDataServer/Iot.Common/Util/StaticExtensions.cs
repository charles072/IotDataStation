﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Iot.Common.Util
{
    public static class StaticExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] SubArrayDeepClone<T>(this T[] data, int index, int length)
        {
            T[] arrCopy = new T[length];
            Array.Copy(data, index, arrCopy, 0, length);
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, arrCopy);
                ms.Position = 0;
                return (T[])bf.Deserialize(ms);
            }
        }
    }
}
