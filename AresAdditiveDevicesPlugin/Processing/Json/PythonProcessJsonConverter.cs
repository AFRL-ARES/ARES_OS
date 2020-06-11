using System;
using System.Collections.Generic;
using AresAdditiveDevicesPlugin.PythonInterop;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using AresAdditiveDevicesPlugin.PythonInterop.Impl;
using CommonServiceLocator;
using DynamicData.Binding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    [JsonConverter(typeof(PythonProcess))]
    public class PythonProcessJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            PythonProcess component = null;
            var jPyProcess = JObject.Load(reader);
            var jId = jPyProcess["Id"].ToObject<int>();
            var defaultInputs = new ObservableCollectionExtended<IProcessData>();
            var inArray = JArray.Load(jPyProcess["DefaultInputs"].CreateReader());
            foreach (var inObject in inArray)
            {
                IProcessData processData = null;
                var processDataString = inObject.ToString();
                processData = JsonConvert.DeserializeObject<IProcessData>(processDataString); // Cannot use serializer
                defaultInputs.Add(processData);
            }

            var jProcessDef = jPyProcess["ProcessDefinition"];
            var jClassName = jPyProcess["ClassName"];

            var bindings = ServiceLocator.Current.GetInstance<IPythonBindings>();
            var configRepo = ServiceLocator.Current.GetInstance<IPythonProcessConfigRepository>();
            var configWriter = ServiceLocator.Current.GetInstance<IConfigurationWriter>();

            var className = jClassName.ToObject<string>();
            var processDefinition = jProcessDef.ToObject<KeyValuePair<string, List<object>>>();

            component = new PythonProcess(bindings, className, processDefinition, configRepo, configWriter);
            component.DefaultInputs = defaultInputs;
            return component;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override bool CanWrite => false;
    }
}
