using System.Collections.Generic;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonInterop
{
  public interface IPythonInvoker
  {
    Task<string> InvokePythonDirect(string command, List<object> args = null);

    string InvocationResponse { get; set; }
    Task WriteDict(string dictName, string dictKey, double value);
    Task WriteDict(string dictName, string dictKey, string value);
  }
}
