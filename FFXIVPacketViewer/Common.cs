using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFXIVPacketViewer
{
    class Common
    {
        /**
         * <summary>Turns a unix timestamp (in miliseconds since epoch) into a localized DateTime object.</summary>
         * <param name="unixTimeStamp">Miliseconds since Unix Epoch</param>
         * <returns>Localized DateTime</returns>
         */
        public static DateTime UnixTimeStampToDateTimeMiliseconds(double unixTimeStamp)
        {
            // Unix timestamp is Miliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /**
         * <summary>Turns a unix timestamp (in seconds since epoch) into a localized DateTime object.</summary>
         * <param name="unixTimeStamp">Seconds since Unix Epoch</param>
         * <returns>Localized DateTime</returns>
         */ 
        public static DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /**
         * <summary>Turns a HesString into a Byte[].</summary>
         * <param name="hex">HexString to read. Will remove hyphens.</param>
         * <returns>Byte[]</returns>
         */
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
        /**
         * <summary>Reads a byte array backwards into a string.</summary>
         * <param name="input">The byte array to read from.</param>
         * <param name="size">The number of bytes to read</param>
         * <param name="firstByte">The first byte to read and start decrementing from.</param>
         * <returns>HexString (No speration)</returns>
         */ 
        public static string endianInterpreter(byte[] input, int size, int firstByte)
        {
            string data = "";
            for(int I = 0;I<size;I++)
            {
                data += BitConverter.ToString(input, firstByte-I, 1);
            }
            return data;
        }
        /**
         * <summary>Converts a HexString into a UInt16.</summary>
         * <param name="input">HexString to read.</param>
         * <returns>UInt16</returns>
         */ 
        public static UInt16 HexToUInt16(string input)
        {
            return Convert.ToUInt16(long.Parse(input, System.Globalization.NumberStyles.HexNumber));
        }
        /**
         * <summary>Converts a HexString into a UInt32.</summary>
         * <param name="input">HexString to read.</param>
         * <returns>UInt32</returns>
         */
        public static UInt32 HexToUInt32(string input)
        {
            return Convert.ToUInt32(long.Parse(input, System.Globalization.NumberStyles.HexNumber));
        }
        /**
         * <summary>Converts a HexString into a UInt64.</summary>
         * <param name="input">HexString to read.</param>
         * <returns>UInt64</returns>
         */
        public static UInt64 HexToUInt64(string input)
        {
            return Convert.ToUInt64(long.Parse(input, System.Globalization.NumberStyles.HexNumber));
        }
        /**
         * <summary>Converts a HexString into a Float.</summary>
         * <param name="input">HexString to read.</param>
         * <returns>Single</returns>
         */
        public static Single HexToFloat(String input)
        {
            uint num = uint.Parse(input, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] floatVals = BitConverter.GetBytes(num);
            float f = BitConverter.ToSingle(floatVals, 0);
            return f;
        }
    }
}
