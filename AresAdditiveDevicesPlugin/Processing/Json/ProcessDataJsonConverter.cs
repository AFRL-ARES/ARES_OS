using System;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.Processing.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
  [JsonConverter(typeof(IProcessData))]
  public class ProcessDataJasonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException("Don't implement this, it'll destroy the serializer");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      IProcessData processData = null;
      var obj = JObject.Load(reader);
      var type = obj["Type"].ToObject<Type>();

      object data = null;

      var concreteTypeToken = obj["Data"].SelectToken("$type");
      if (concreteTypeToken != null)
      {
        data = obj["Data"].ToObject(concreteTypeToken.ToObject<Type>());
      }
      else
      {
        data = obj["Data"].ToObject(type);
      }
      var name = obj["Name"].ToObject<string>();
      processData = typeof(ProcessData<>).MakeGenericType(type).GenerateInstance(name, data) as IProcessData;

      processData.RequiresSetup = obj["RequiresSetup"].Value<bool>();
      processData.IsResult = obj["IsResult"].Value<bool>();
      processData.IsAnalysisImage = obj["IsAnalysisImage"].Value<bool>();

      return processData;
    }

    public override bool CanConvert(Type objectType)
    {
      return false;
    }

    public override bool CanWrite => false;
  }
}
