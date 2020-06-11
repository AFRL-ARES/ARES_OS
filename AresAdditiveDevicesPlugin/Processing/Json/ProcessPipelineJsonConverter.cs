using System;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Staging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    public class ProcessPipelineJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var pipeline = new ProcessPipeline();
            var jarray = JArray.Load(reader);
            foreach (var jObj in jarray)
            {
                var stageEntry = serializer.Deserialize<IStageEntry>(jObj.CreateReader());
                pipeline.Add(stageEntry);
            }
            return pipeline;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override bool CanWrite => false;
    }
}
