using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
    public class ParameterLimitConfigurations
    {
        [JsonProperty]
        public List<ParameterLimitConfiguration> Limits { get; set; } = new List<ParameterLimitConfiguration>();

    }
}
