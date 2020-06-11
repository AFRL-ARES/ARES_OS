using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.PythonInterop.Impl
{
  public class PythonInvoker : BasicReactiveObjectDisposable, IPythonInvoker
  {
    private readonly IPythonProcessRepository _pythonProcessRepo;
    private readonly IPythonBindings _bindings;
    private string _invocationResponse;

    public PythonInvoker(IPythonProcessRepository pyProcessRepo, IPythonBindings bindings)
    {
      _pythonProcessRepo = pyProcessRepo;
      _bindings = bindings;
    }

    public string InvocationResponse
    {
      get => _invocationResponse;
      set => this.RaiseAndSetIfChanged(ref _invocationResponse, value);
    }

    public async Task WriteDict(string dictName, string dictKey, double value)
    {
      await _bindings.WriteDict(dictName, dictKey, value);
    }

    public async Task WriteDict(string dictName, string dictKey, string value)
    {
      await _bindings.WriteDict(dictName, dictKey, value);
    }

    public async Task<string> InvokePython(string command, List<object> args = null)
    {
      var split = command.Split('.');
      if (split.Length != 2)
      {
        return "Invalid command " + command + ".Should be something like<classname>.< defname >.";
      }
      var className = split[0];
      var funcName = split[1];
      var process = _pythonProcessRepo.FirstOrDefault(p => p.ClassName.Equals(className) && p.ComponentName.Equals(funcName));
      if (process == null)
      {
        return "Could not find a valid python call of " + command;
      }
      if (args == null)
        args = new List<object>();
      try
      {
        var str = await _bindings.RunPythonDef(className, funcName, args);
        InvocationResponse = str;
      }
      catch (Exception e)
      {
        InvocationResponse = "Python Error: " + e.Message;
      }
      return "";
    }

    public async Task<string> InvokePythonDirect(string command, List<object> args = null)
    {
      var split = command.Split('.');
      if (split.Length != 2)
      {
        return "Invalid command " + command + ". Should be something like <classname>.<defname>.";
      }
      var className = split[0];
      var funcName = split[1];
      var process = _pythonProcessRepo.FirstOrDefault(p => p.ClassName.Equals(className) && p.ComponentName.Equals(funcName));
      if (process == null)
      {
        return "Could not find a valid python call of " + command;
      }
      if (args == null)
        args = new List<object>();
      try
      {
        var str = await _bindings.RunPythonDef(className, funcName, args);
        return str;

      }
      catch (Exception)
      {
        return "";
      }
    }
  }
}
