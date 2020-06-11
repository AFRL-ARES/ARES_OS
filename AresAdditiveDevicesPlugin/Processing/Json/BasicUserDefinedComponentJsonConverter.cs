using System;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using AresAdditiveDevicesPlugin.Processing.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    [JsonConverter(typeof(BasicUserDefinedComponentJsonConverter))]
    public class BasicUserDefinedComponentJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jComponent = JObject.Load(reader);
            var jPipeline = jComponent["Pipeline"];
            var pipeLine = serializer.Deserialize<ProcessPipeline>(jPipeline.CreateReader());

            var jComponentName = jComponent["ComponentName"];
            var componentName = jComponentName.ToObject<string>();


            return new BasicUserDefinedComponent(pipeLine, componentName);
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override bool CanWrite => false;
    }
}
