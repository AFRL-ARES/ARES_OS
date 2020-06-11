using System;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Staging;
using AresAdditiveDevicesPlugin.Processing.Staging.Impl;
using DynamicData.Binding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    public class StageEntryJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IStageEntry stageEntry = null;
            var obj = JObject.Load(reader);
            var stageId = obj["StageId"].ToObject<int>();
            var uniqueId = obj["UniqueId"].ToObject<Guid>();
            var componentJson = obj["Process"].ToString();
            var inputs = new ObservableCollectionExtended<IProcessData>();
            var inArray = JArray.Load(obj["Inputs"].CreateReader());
            foreach (var inObject in inArray)
            {
                IProcessData processData = null;
                var processDataString = inObject.ToString();
                processData = JsonConvert.DeserializeObject<IProcessData>(processDataString);
                inputs.Add(processData);
            }

            var componentTypeToken = obj["Process"].SelectToken("$type");
            var componentType = typeof(IComponent);
            if (componentTypeToken != null)
            {
                componentType = componentTypeToken.ToObject<Type>();
            }

            var component = JsonConvert.DeserializeObject(componentJson, componentType);
            var inputMappings = serializer.Deserialize<StageEntryInputMappings>(obj["InputMappings"].CreateReader());

            stageEntry = new StageEntry(component as IComponent, inputs, stageId, uniqueId, inputMappings);
            return stageEntry;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override bool CanWrite => false;
    }
}
