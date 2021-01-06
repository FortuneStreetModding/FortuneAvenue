using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomStreetMapManager
{
    public class CharacterConverter : JsonConverter<Character>
    {
        public override Character Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Character character;
            if (!Enum.TryParse(reader.GetString(), out character))
                throw new JsonException("Could not parse character");
            return character;
        }

        public override void Write(Utf8JsonWriter writer, Character character, JsonSerializerOptions options)
        {
            writer.WriteStringValue(character.ToString());
        }

    }

}
