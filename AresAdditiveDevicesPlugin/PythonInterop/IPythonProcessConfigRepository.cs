using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.PythonInterop
{
    public interface IPythonProcessConfigRepository : IObservableCollection<PythonProcessConfiguration>
    {
    }
}
