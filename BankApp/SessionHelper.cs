using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankApp
{
    public class SessionHelper
    {
        // henter et objekt af typen T fra session
        public static T? Get<T>(T type, HttpContext context) where T : class
        {
            string sessionName = nameof(type);
            string? s = context.Session.GetString(sessionName);
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return JsonSerializer.Deserialize<T>(s, options);
        }
        // gemmer et objekt af typen T i session
        public static void Set<T>(T type, HttpContext context) where T : class
        {
            string sessionName = nameof(type);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string s = JsonSerializer.Serialize(type, options);
            context.Session.SetString(sessionName, s);
        }
        // fjerner et objekt af type T fra session
        public static void Clear<T>(T type, HttpContext context)
        {
            context.Session.Remove(nameof(type));
        }
    }
}
