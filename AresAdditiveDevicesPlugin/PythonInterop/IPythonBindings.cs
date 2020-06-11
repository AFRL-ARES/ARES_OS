using System.Collections.Generic;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonInterop
{
  public interface IPythonBindings
  {
    Task<string> Init();
    List<string> GetDefNames(string classname);
    List<string> GetClassNames();
    object LastResult { get; set; }
    bool ConfigurationNeeded { get; set; }
    Task<string> RunPythonDef(string className, string def, List<object> argsList);
    Task WriteDict(string dictName, string dictKey, double value);
    Task WriteDict(string dictName, string dictKey, string value);
    Task<double> GetDict(string dictName, string dictKey, int index = 0);

    Task<double> GetDict(string dictName, string dictKey);
  }
}
