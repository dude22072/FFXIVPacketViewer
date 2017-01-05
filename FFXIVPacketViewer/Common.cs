using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFXIVPacketViewer
{
    class Common
    {
        public static string[] zoneIDs = {
        /*0->*/"--", "1", "2", "3", "4", "5", "6", "7", "8", "9",
        /*10->*/"", "", "", "", "", "", "", "", "", "",
        /*20->*/"", "", "", "", "", "", "", "", "", "",
        /*30->*/"", "", "", "", "", "", "", "", "", "",
        /*40->*/"", "", "", "", "", "", "", "", "", "",
        /*50->*/"", "", "", "", "", "", "", "", "", "",
        /*60->*/"", "", "", "", "", "", "", "", "", "",
        /*70->*/"", "", "", "", "", "", "", "", "", "",
        /*80->*/"", "", "", "", "", "", "", "", "", "",
        /*90->*/"", "", "", "", "", "", "", "", "", "",
        /*100->*/"", "", "", "", "", "", "", "", "", "",
        /*110->*/"", "", "", "", "", "", "", "", "", "",
        /*120->*/"120", "121", "122", "123", "124", "125", "126", "127", "Lower La Noscea", "Western La Noscea",
        /*130->*/"Eastern La Noscea", "Mistbeard Cove", "Cassiopeia Hollow", "Limsa Lominsa", "Market Wards", "Upper La Noscea", "136", "U'Ghamaro Mines", "La Noscea", "The Cieldalaes",
        /*140->*/"Sailors Ward", "Lower La Noscea", "141", "Coerthas Central Highlands", "Coerthas Eastern Highlands", "Coerthas Eastern Lowlands", "Coerthas", "	Coerthas Central Lowlands", "Coerthas Western Highlands", "149",
        /*150->*/"Central Shroud", "East Shroud", "North Shroud", "West Shroud", "South Shroud", "Gridania", "The Black Shroud", "The Mun-Tuy Cellars", "The Tam-Tara Deepcroft", "The Thousand Maws of Toto-Rak",
        /*160->*/"Market Wards", "Peasants Ward", "Central Shroud", "163", "Central Shroud", "Central Shroud", "Central Shroud", "Central Shroud", "Central Shroud", "169",
        /*170->*/"Central Thanalan", "Eastern Thanalan", "Western Thanalan", "Northern Thanalan", "Southern Thanalan", "Ul'dah", "Nanawa Mines", "--", "Copperbell Mines", "Thanalan",
        /*180->*/"Market Wards", "Merchants Ward", "Central Thanalan", "183", "Ul'dah", "Ul'dah", "Ul'dah", "Ul'dah", "Ul'dah", "189",
        /*190->*/"Mor Dhona", "191", "Rhotano Sea", "Rhotano Sea", "Rhotano Sea", "Rhotano Sea", "Rhotano Sea", "197", "Rhotano Sea", "199",
        /*200->*/"Strait of Merlthor", "--", "202", "203", "Western La Noscea", "Eastern La Noscea", "Gridania", "North Shroud", "South Shroud", "Ul'dah",
        /*210->*/"Eastern Thanalan", "Western Thanalan", "212", "213", "214", "215", "216", "217", "218", "219",
        /*220->*/"220", "221", "222", "223", "224", "225", "226", "227", "228", "229",
        /*230->*/"Limsa Lominsa", "Dzemael Darkhold", "Maelstrom Command", "Hall of Flames", "Adders' Nest", "Shposhae", "Locke's Lie", "Turtleback Island", "Thornmarch", "The Howling Eye",
        /*240->*/"The Bowl of Embers", "241", "242", "243", "Inn Room", "The Aurum Vale", "Cutter's Cry", "North Shroud", "Western La Noscea", "Eastern Thanalan",
        /*250->*/"The Howling Eye", "Transmission Tower", "The Aurum Vale", "The Aurum Vale", "Cutter's Cry", "Cutter's Cry", "The Howling Eye", "Rivenroad", "North Shroud", "North Shroud",
        /*260->*/"Western La Noscea", "Western La Noscea", "Eastern Thanalan", "Eastern Thanalan", "Transmission Tower", "The Bowl of Embers", "Mor Dhona", "Rivenroad", "Rivenroad", "Locke's Lie",
        /*270->*/"Turtleback Island", "271", "272", "273", "274", "275", "276", "277", "278", "279"
        };
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
