using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agif_V2.Helpers
{
    public class FlexibleDecimalConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // If it's explicitly null, return null
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            // If it's a string, clean it up before parsing
            if (reader.TokenType == JsonTokenType.String)
            {
                string? value = reader.GetString();

                // Handle empty strings gracefully
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                // Strip commas out in case the user typed "1,00,000"
                value = value.Replace(",", "").Trim();

                if (decimal.TryParse(value, out decimal result))
                {
                    return result;
                }
            }
            // If it comes through as a native number, just read it
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }

            throw new JsonException($"Unable to parse the value as a valid decimal.");
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}