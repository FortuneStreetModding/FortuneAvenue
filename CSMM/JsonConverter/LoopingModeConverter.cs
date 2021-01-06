using FSEditor.FSData;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomStreetMapManager
{
    public class LoopingModeConverter : JsonConverter<LoopingMode>
    {
        public override LoopingMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            LoopingMode loopingMode;
            if (!Enum.TryParse(reader.GetString(), out loopingMode))
                throw new JsonException("Could not parse loopingMode");
            return loopingMode;
        }

        public override void Write(Utf8JsonWriter writer, LoopingMode loopingMode, JsonSerializerOptions options)
        {
            writer.WriteStringValue(loopingMode.ToString());
        }

    }

}
