using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonInterop.Impl;

namespace AresAdditiveDevicesPlugin.PythonInterop
{
    public interface IPythonProcessFactory
    {
        List<IPythonProcess> Create(IPythonBindings bindings, PythonDef definition);
    }
}
