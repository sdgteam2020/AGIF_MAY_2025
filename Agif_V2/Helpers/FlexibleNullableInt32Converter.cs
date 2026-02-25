using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agif_V2.Helpers
{
    public class FlexibleNullableInt32Converter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string? value = reader.GetString();

                // If the form field was left blank, safely return null for the int?
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                // Strip any commas just in case
                value = value.Replace(",", "").Trim();

                if (int.TryParse(value, out int result))
                {
                    return result;
                }
            }

            throw new JsonException("Unable to parse the value as a valid nullable Int32.");
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}