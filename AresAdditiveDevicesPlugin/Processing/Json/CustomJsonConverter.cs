using System;
using DynamicData.Binding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    public class CustomJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var repo = new ObservableCollectionExtended<IProcessData>();
            var pdArray = JArray.Load(reader);
            foreach (var pdObject in pdArray)
            {
                IProcessData processData = null;
                var processDataString = pdObject.ToString();
                processData = JsonConvert.DeserializeObject<IProcessData>(processDataString);
                repo.Add(processData);
            }
            return repo;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override bool CanWrite => false;
    }
}
