using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
    public class ParameterLimitConfigurationsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var config = value as ParameterLimitConfigurations;
            config.Limits.ForEach(limit => serializer.Serialize(writer, limit));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var jLimits = jObject["Limits"];
            var configs = new ParameterLimitConfigurations();
            foreach (var jLimit in jLimits)
            {
                var limit = serializer.Deserialize<ParameterLimitConfiguration>(jLimit.CreateReader());
                configs.Limits.Add(limit);
            }
            return configs;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ParameterLimitConfigurations);
        }
    }
}
