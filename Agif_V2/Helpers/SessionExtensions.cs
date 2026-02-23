using System.Text.Json;

namespace Agif_V2.Helpers
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            if (value == null)
            {
                session.Remove(key); // Optionally remove key if null
                return;
            }

            var jsonString = JsonSerializer.Serialize(value);
            session.SetString(key, jsonString);
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var jsonString = session.GetString(key);
            if (string.IsNullOrEmpty(jsonString))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }

}
