using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Processing.Json;
using Newtonsoft.Json;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base
{
    [JsonConverter(typeof(BasicOpenCvComponentJsonConverter))]
    public class BasicOpenCvComponent : BasicComponent
    {

        public BasicOpenCvComponent(MethodInfo cvInvokeMethod)
        {
            if (cvInvokeMethod == null)
                return;
            CvInvokeMethod = cvInvokeMethod;
            ComponentName = cvInvokeMethod.Name;
        }

        [JsonProperty("CvInvokeMethod")]
        public MethodInfo CvInvokeMethod { get; set; }

        public override string ComponentName
        {
            get => $"{_name}";
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public override Task StartComponent(IList<IProcessData> inputs)
        {
            var invokableArgs = inputs.Select(input => input.Data).ToArray();
            CvInvokeMethod.Invoke(null, invokableArgs);
            for (var i = 0; i < inputs.Count; i++)
            {
                inputs[i].Data = invokableArgs[i]; // update the source inputs with the post invocation inputs
            }

            return Task.CompletedTask;
        }

        [JsonIgnore]
        public static string HumanReadableTypeString = "OpenCv";
    }
}
