using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomStreetMapManager
{
    public class VentureCardConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str.Length != 128)
            {
                throw new JsonException("Venture Card string must be of length 128");
            }
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '1')
                {
                    bytes[i] = 1;
                }
                else if (str[i] == '0')
                {
                    bytes[i] = 0;
                }
                else
                {
                    throw new JsonException("Cannot parse venture cards");
                }
            }
            return bytes;
        }

        public override void Write(Utf8JsonWriter writer, byte[] ventureCards, JsonSerializerOptions options)
        {
            var str = "";
            foreach (byte ventureCard in ventureCards)
            {
                if (ventureCard == 0)
                    str += "0";
                else
                    str += "1";
            }
            writer.WriteStringValue(str);
        }

    }

}
