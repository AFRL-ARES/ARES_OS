using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
    public interface IConfigurationWriter
    {
        void Write(IPythonProcess process);
    }
}
