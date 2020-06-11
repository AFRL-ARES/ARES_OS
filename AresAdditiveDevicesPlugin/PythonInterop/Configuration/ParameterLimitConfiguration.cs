using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
    public class ParameterLimitConfiguration
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public double Min { get; set; }
        [JsonProperty]
        public double Max { get; set; }
    }
}
