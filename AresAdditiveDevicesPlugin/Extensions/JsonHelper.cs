using AresAdditiveDevicesPlugin.Processing.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AresAdditiveDevicesPlugin.Extensions
{
    public static class JsonHelper
    {
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
            Converters = new JsonConverter[] { new StringEnumConverter() },
            ContractResolver = new CustomContractResolver()
        };

        public static string SerializeObject(this object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        public static T DeserializeObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
        }
    }
}
