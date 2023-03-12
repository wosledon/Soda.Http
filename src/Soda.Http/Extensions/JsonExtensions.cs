using Newtonsoft.Json;

namespace Soda.Http.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T? ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T?>(json);
        }
    }
}