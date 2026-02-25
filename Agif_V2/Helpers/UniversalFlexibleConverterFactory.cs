using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agif_V2.Helpers
{
    public class UniversalFlexibleConverterFactory : JsonConverterFactory
    {
        // 1. Tell the factory which data types it is allowed to intercept
        public override bool CanConvert(Type typeToConvert)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
            return underlyingType == typeof(int) ||
                   underlyingType == typeof(decimal) ||
                   underlyingType == typeof(double) ||
                   underlyingType == typeof(bool) ||
                   underlyingType == typeof(DateTime);
        }

        // 2. Create the generic converter for the specific type being processed
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(FlexibleUniversalConverter<>).MakeGenericType(typeToConvert))!;
        }

        // 3. The actual conversion logic
        private class FlexibleUniversalConverter<T> : JsonConverter<T>
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Type underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                bool isNullable = Nullable.GetUnderlyingType(typeof(T)) != null;

                // Handle native nulls
                if (reader.TokenType == JsonTokenType.Null)
                {
                    if (isNullable || !typeof(T).IsValueType) return default;
                    if (underlyingType == typeof(bool)) return (T)(object)false; // Non-nullable bool defaults to false
                    return default;
                }

                string? value = null;

                // Extract the value as a string, regardless of how it arrived
                if (reader.TokenType == JsonTokenType.String)
                    value = reader.GetString()?.Trim();
                else if (reader.TokenType == JsonTokenType.Number)
                    value = reader.GetDouble().ToString(CultureInfo.InvariantCulture);
                else if (reader.TokenType == JsonTokenType.True)
                    value = "true";
                else if (reader.TokenType == JsonTokenType.False)
                    value = "false";

                // Handle Empty Strings from HTML Forms
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (isNullable) return default;
                    if (underlyingType == typeof(bool)) return (T)(object)false;
                    if (underlyingType == typeof(int) || underlyingType == typeof(decimal) || underlyingType == typeof(double))
                        return (T)Convert.ChangeType(0, underlyingType); // Non-nullable numbers default to 0
                    return default;
                }

                // Strip commas from financial inputs (e.g., "50,000.50" -> "50000.50")
                if (underlyingType == typeof(int) || underlyingType == typeof(decimal) || underlyingType == typeof(double))
                {
                    value = value.Replace(",", "");
                }

                try
                {
                    // Parse Numbers
                    if (underlyingType == typeof(int))
                        return (T)(object)Convert.ToInt32(Math.Round(Convert.ToDouble(value, CultureInfo.InvariantCulture)));
                    if (underlyingType == typeof(decimal))
                        return (T)(object)Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    if (underlyingType == typeof(double))
                        return (T)(object)Convert.ToDouble(value, CultureInfo.InvariantCulture);

                    // Parse Booleans
                    if (underlyingType == typeof(bool))
                    {
                        value = value.ToLowerInvariant();
                        bool isTrue = value == "true" || value == "on" || value == "yes" || value == "1";
                        return (T)(object)isTrue;
                    }

                    // Parse Dates
                    if (underlyingType == typeof(DateTime))
                    {
                        string[] formats = { "dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" };
                        if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime exactDate))
                            return (T)(object)exactDate;
                        if (DateTime.TryParse(value, out DateTime parsedDate))
                            return (T)(object)parsedDate;
                    }
                }
                catch
                {
                    // If parsing completely fails, fall through to the default return
                }

                return isNullable ? default : default!;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNullValue();
                    return;
                }

                Type underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

                if (underlyingType == typeof(int)) writer.WriteNumberValue((int)(object)value);
                else if (underlyingType == typeof(decimal)) writer.WriteNumberValue((decimal)(object)value);
                else if (underlyingType == typeof(double)) writer.WriteNumberValue((double)(object)value);
                else if (underlyingType == typeof(bool)) writer.WriteBooleanValue((bool)(object)value);
                else if (underlyingType == typeof(DateTime)) writer.WriteStringValue(((DateTime)(object)value).ToString("yyyy-MM-dd"));
                else writer.WriteStringValue(value.ToString() ?? "");
            }
        }
    }
}
