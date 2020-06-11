using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
    public class PythonProcessConfigRepository : ObservableCollectionExtended<PythonProcessConfiguration>, IPythonProcessConfigRepository
    {
    }
}
