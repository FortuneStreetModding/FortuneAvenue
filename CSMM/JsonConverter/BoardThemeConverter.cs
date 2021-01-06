using FSEditor.FSData;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomStreetMapManager
{
    public class BoardThemeConverter : JsonConverter<BoardTheme>
    {
        public override BoardTheme Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            BoardTheme boardTheme;
            if (!Enum.TryParse(reader.GetString(), out boardTheme))
                throw new JsonException("Could not parse boardTheme");
            return boardTheme;
        }

        public override void Write(Utf8JsonWriter writer, BoardTheme boardTheme, JsonSerializerOptions options)
        {
            writer.WriteStringValue(boardTheme.ToString());
        }

    }

}
