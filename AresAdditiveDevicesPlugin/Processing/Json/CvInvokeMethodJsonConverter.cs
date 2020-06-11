using Emgu.CV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
  [JsonConverter(typeof(MethodInfo))]
  public class CvInvokeMethodJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var obj = JObject.Load(reader);
      var cvInvokeMethods = typeof(CvInvoke).GetMethods();
      var signature = obj["Signature"].ToObject<string>();
      var matchingMethod = cvInvokeMethods.FirstOrDefault(method => method.ToString() == signature);
      return matchingMethod;
    }

    public override bool CanConvert(Type objectType)
    {
      return false;
    }

    public override bool CanWrite => false;
  }
}
