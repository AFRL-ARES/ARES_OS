using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
    public class PythonProcessConfiguration
    {
        public string ProcessName { get; set; }

        public List<PythonVariableConfiguration> InputConfigurations { get; set; }

        public PythonVariableConfiguration OutputConfiguration { get; set; }
    }
}
