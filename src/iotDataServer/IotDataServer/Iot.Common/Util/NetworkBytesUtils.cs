namespace Iot.Common.Util
{
    public class NetworkBytesUtils
    {
        public static byte[] GetBytes(uint value)
        {
            try
            {
                byte[] bytes = new byte[4];
                bytes[0] = (byte)(value >> 24);
                bytes[1] = (byte)(value >> 16);
                bytes[2] = (byte)(value >> 8);
                bytes[3] = (byte)(value);
                return bytes;
            }
            catch
            {
                //Todo: log 남기는 문제는 생각 해보자
            }

            return null;
        }

        public static uint GetInt(byte[] dataBytes, int offset)
        {
            try
            {
                if (dataBytes.Length <= (offset + 3))
                {
                    return 0xffffffff;
                }
                uint returnData = (uint)(dataBytes[offset] << 24 | dataBytes[offset + 1] << 16 | dataBytes[offset + 2] << 8 | dataBytes[offset + 3]);
                return returnData;
            }
            catch
            {
                //Todo: log 남기는 문제는 생각 해보자
            }
            return 0;
        }
        public static uint GetLittleEndianInt(byte[] dataBytes, int offset)
        {
            try
            {
                if (dataBytes.Length <= (offset + 3))
                {
                    return 0xffffffff;
                }
                uint returnData = (uint)(dataBytes[offset]| dataBytes[offset + 1] << 8 | dataBytes[offset + 2] << 16 | dataBytes[offset + 3] << 24);
                return returnData;
            }
            catch
            {
                //Todo: log 남기는 문제는 생각 해보자
            }
            return 0;
        }
    }
}
