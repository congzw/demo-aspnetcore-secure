using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Common
{
    public static class JsonExtensions
    {
        public static T LogJson<T>(this T value, string prefix = null, bool indented = false)
        {
            value.GetJson(true, prefix, indented);
            return value;
        }

        public static string GetJson<T>(this T value, bool logJson = false, string prefix = null, bool indented = false)
        {
            var json = JsonConvert.SerializeObject(value, indented ? Formatting.Indented : Formatting.None);
            if (logJson)
            {
                json.Log(prefix);
            }
            return json;
        }
    }
}
