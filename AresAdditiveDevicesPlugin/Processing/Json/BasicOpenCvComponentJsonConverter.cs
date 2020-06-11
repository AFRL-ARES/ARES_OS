using System;
using System.Linq;
using AresAdditiveDevicesPlugin.Processing.Components.Base;
using DynamicData.Binding;
using Emgu.CV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    [JsonConverter(typeof(BasicOpenCvComponent))]
    public class BasicOpenCvComponentJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            BasicOpenCvComponent component = null;
            var obj = JObject.Load(reader);
            var id = obj["Id"].ToObject<int>();
            var defaultInputs = new ObservableCollectionExtended<IProcessData>();
            var inArray = JArray.Load(obj["DefaultInputs"].CreateReader());
            foreach (var inObject in inArray)
            {
                IProcessData processData = null;
                var processDataString = inObject.ToString();
                processData = JsonConvert.DeserializeObject<IProcessData>(processDataString);
                defaultInputs.Add(processData);
            }
            var methodObject = JObject.Load(obj["CvInvokeMethod"].CreateReader());


            var cvInvokeMethods = typeof(CvInvoke).GetMethods();
            var signature = methodObject["Signature"].ToObject<string>();
            var matchingMethod = cvInvokeMethods.FirstOrDefault(method => method.ToString() == signature);

            //    var methodInfo = serializer.Deserialize<MethodInfo>( methodObject.CreateReader() );
            //    component = new BasicOpenCvComponent( methodInfo ) { DefaultInputs = defaultInputs, Id = id };
            component = new BasicOpenCvComponent(matchingMethod) { DefaultInputs = defaultInputs, Id = id };
            return component;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override bool CanWrite => false;
    }
}
