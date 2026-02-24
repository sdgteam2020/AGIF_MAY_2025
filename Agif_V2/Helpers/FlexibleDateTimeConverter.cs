using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agif_V2.Helpers
{
    public class FlexibleDateTimeConverter : JsonConverter<DateTime?>
    {
        // Add all the formats your front-end might send
        private readonly string[] _formats = {
            "dd-MM-yyyy",
            "dd/MM/yyyy",
            "yyyy-MM-dd",
            "MM/dd/yyyy"
        };

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // Try to parse using your exact formats
            if (DateTime.TryParseExact(value, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            // Fallback to standard C# parsing if the exact formats fail
            if (DateTime.TryParse(value, out date))
            {
                return date;
            }

            throw new JsonException($"Unable to parse '{value}' as a valid DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
