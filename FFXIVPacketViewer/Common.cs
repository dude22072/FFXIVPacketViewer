using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFXIVPacketViewer
{
    class Common
    {
        public static DateTime UnixTimeStampToDateTimeMiliseconds(double unixTimeStamp)
        {
            // Unix timestamp is Miliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        public static string endianInterpreter(byte[] input, int size, int firstByte)
        {
            string data = "";
            for(int I = 0;I<size;I++)
            {
                data += BitConverter.ToString(input, firstByte-I, 1);
            }
            return data;
        }
        public static UInt16 HexToUInt16(string input)
        {
            return Convert.ToUInt16(long.Parse(input, System.Globalization.NumberStyles.HexNumber));
        }
        public static UInt32 HexToUInt32(string input)
        {
            return Convert.ToUInt32(long.Parse(input, System.Globalization.NumberStyles.HexNumber));
        }
        public static UInt64 HexToUInt64(string input)
        {
            return Convert.ToUInt64(long.Parse(input, System.Globalization.NumberStyles.HexNumber));
        }
    }
}
