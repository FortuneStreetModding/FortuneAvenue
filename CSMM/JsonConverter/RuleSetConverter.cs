using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomStreetMapManager
{
    public class RuleSetConverter : JsonConverter<RuleSet>
    {
        public override RuleSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            RuleSet ruleSet;
            if (!Enum.TryParse(reader.GetString(), out ruleSet))
                throw new JsonException("Could not parse ruleSet");
            return ruleSet;
        }

        public override void Write(Utf8JsonWriter writer, RuleSet ruleSet, JsonSerializerOptions options)
        {
            writer.WriteStringValue(ruleSet.ToString());
        }

    }

}
