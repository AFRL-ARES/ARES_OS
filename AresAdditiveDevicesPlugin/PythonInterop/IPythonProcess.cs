using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using AresAdditiveDevicesPlugin.Processing;

namespace AresAdditiveDevicesPlugin.PythonInterop
{
    public interface IPythonProcess : IComponent
    {
        KeyValuePair<string, List<object>> ProcessDefinition { get; set; }

        List<PythonVariableConfiguration> InputVariables { get; }

        PythonVariableConfiguration OutputVariable { get; set; }

        string ClassName { get; }

        bool Configured { get; set; }
        ReactiveCommand<bool, Unit> OutputCheckedCommand { get; }
    }
}
