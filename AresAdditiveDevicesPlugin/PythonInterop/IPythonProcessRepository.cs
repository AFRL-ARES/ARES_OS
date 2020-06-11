using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.PythonInterop
{
    public interface IPythonProcessRepository : IObservableCollection<IPythonProcess>
    {
    }
}
