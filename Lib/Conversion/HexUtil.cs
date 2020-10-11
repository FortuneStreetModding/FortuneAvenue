using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiscUtil.Conversion
{
    public class HexUtil
    {

        public static byte[] hexStringToByteArray(string hex)
        {
            hex = hex.Replace(" ", "");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string byteArrayToString(byte[] data)
        {
            int inx = Array.FindIndex(data, 0, (x) => x == 0);
            if (inx >= 0)
                return (Encoding.ASCII.GetString(data, 0, inx));
            else
                return (Encoding.ASCII.GetString(data));
        }

        public static string byteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public static string byteArrayToStringOrHex(byte[] bytes)
        {
            var str = Encoding.UTF8.GetString(bytes);
            bool constainsAnsiCharsOnlyOrZero = str.All(c => (c >= 32 && c <= 255) || c == 0 );
            bool containsAtLeastOneAnsiChar = str.Any(c => (c >= 32 && c <= 255));
            if (constainsAnsiCharsOnlyOrZero && containsAtLeastOneAnsiChar)
            {
                return byteArrayToString(bytes);
            }
            else
            {
                return byteArrayToHex(bytes);
            }
        }

    }
}
