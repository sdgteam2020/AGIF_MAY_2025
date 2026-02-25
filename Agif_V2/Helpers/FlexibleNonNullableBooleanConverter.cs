using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agif_V2.Helpers
{
    public class FlexibleNonNullableBooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True) return true;
            if (reader.TokenType == JsonTokenType.False) return false;

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() == 1;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string? value = reader.GetString();

                if (string.IsNullOrWhiteSpace(value))
                {
                    return false; // Default for non-nullable bool if the string is empty
                }

                value = value.Trim().ToLowerInvariant();

                if (value == "true" || value == "on" || value == "yes" || value == "1")
                {
                    return true;
                }

                if (value == "false" || value == "off" || value == "no" || value == "0")
                {
                    return false;
                }
            }

            // If the JSON explicitly sent 'null' for a non-nullable bool, default to false
            if (reader.TokenType == JsonTokenType.Null)
            {
                return false;
            }

            throw new JsonException("Unable to parse the value as a valid boolean.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}