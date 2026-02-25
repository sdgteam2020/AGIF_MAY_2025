using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agif_V2.Helpers
{
    public class FlexibleBooleanConverter : JsonConverter<bool?>
    {
        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 1. Handle native JSON null
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            // 2. Handle native JSON booleans
            if (reader.TokenType == JsonTokenType.True) return true;
            if (reader.TokenType == JsonTokenType.False) return false;

            // 3. Handle numbers (1 = true, 0 = false)
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() == 1;
            }

            // 4. Handle strings coming from the HTML form
            if (reader.TokenType == JsonTokenType.String)
            {
                string? value = reader.GetString();

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                value = value.Trim().ToLowerInvariant();

                // Check standard "truthy" string values
                if (value == "true" || value == "on" || value == "yes" || value == "1")
                {
                    return true;
                }

                // Check standard "falsy" string values
                if (value == "false" || value == "off" || value == "no" || value == "0")
                {
                    return false;
                }
            }

            throw new JsonException("Unable to parse the value as a valid boolean.");
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteBooleanValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}