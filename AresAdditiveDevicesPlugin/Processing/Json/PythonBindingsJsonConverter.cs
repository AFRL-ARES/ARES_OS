using System;
using AresAdditiveDevicesPlugin.PythonInterop.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    [JsonConverter(typeof(PythonBindings))]
    public class PythonBindingsJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            PythonBindings bindings = null;
            var jBindings = JObject.Load(reader);


            return bindings;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override bool CanWrite => false;
    }
}
