using AresAdditiveDevicesPlugin.Processing;
using AresAdditiveDevicesPlugin.Processing.Components.Base;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Json;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using Castle.Components.DictionaryAdapter;
using MoreLinq;
using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonInterop.Impl
{
  [JsonConverter(typeof(PythonProcessJsonConverter))]
  public class PythonProcess : BasicComponent, IPythonProcess
  {
    private readonly IPythonBindings _bindings;
    private readonly IPythonProcessConfigRepository _configRepo;
    private readonly IConfigurationWriter _configWriter;
    private bool _configured;
    private PythonVariableConfiguration _outputVariable;

    public PythonProcess()
    {
      // Required for injecting IEnumerable<IComponent> due to binding of IComponent to PythonProcess (ComponentService)
      LoadConfig();
    }


    public PythonProcess(IPythonBindings bindings, string name, KeyValuePair<string, List<object>> processDefinition, IPythonProcessConfigRepository _configRepo, IConfigurationWriter configWriter)
    {
      _bindings = bindings;
      this._configRepo = _configRepo;
      _configWriter = configWriter;
      ClassName = name;
      //      ComponentName = $"{ClassName}.{processDefinition.Key}";
      ComponentName = $"{processDefinition.Key}";
      ProcessDefinition = processDefinition;
      Config = _configRepo.FirstOrDefault(f => f.ProcessName.Equals(ComponentName));
      if (Config?.InputConfigurations == null || Config?.InputConfigurations.Count == 0 &&
          processDefinition.Key.ToLower().Contains("line") && processDefinition.Value.Any())
      {

        Config?.InputConfigurations.Add(new PythonVariableConfiguration() { VariableName = "File", SelectedVariableType = 4 });
      }
      LoadConfig();
      //      processDefinition.Value.ForEach( parameter => DefaultInputs.Add( new ProcessData<object>( parameter.ToString(), parameter ) ) );
      InputVariables.ForEach(pyVarCfg =>
     DefaultInputs.Add(new ProcessData<object>(pyVarCfg.VariableName, pyVarCfg.VariableType)));
      if (OutputVariable != null)
      {
        DefaultInputs.Add(new ProcessData<object>(OutputVariable.VariableName, OutputVariable.VariableType));
      }


      string defname = ComponentName.Substring(ComponentName.LastIndexOf(".") + 1);
      //      StartComponent = Task.Run( () =>
      //        _bindings.RunPythonDef( ClassName, defname, ProcessDefinition.Value )
      //      )
      //      .ContinueWith( res =>
      //      {
      //        if ( res.IsFaulted )
      //          throw res.Exception.InnerException;
      //        return res;
      //      } );
      //      return Task.CompletedTask;
    }

    private void LoadConfig()
    {
      OutputCheckedCommand = ReactiveCommand.Create<bool, Unit>(p => OutputChecked(p));
      InputVariables = new EditableList<PythonVariableConfiguration>();
      if (_configRepo == null)
        return;
      Config = _configRepo.FirstOrDefault(f => f.ProcessName.Equals(ComponentName));
      if (Config != null)
      {
        InputVariables = Config.InputConfigurations;
        OutputVariable = Config.OutputConfiguration;
        Configured = true;
      }
      else
      {
        DefaultInputs.ForEach(p =>
        {
          var iv = new PythonVariableConfiguration
          {
            VariableName = p.Name,
            VariableType = p.Type
          };
          InputVariables.Add(iv);
        });
        Configured = false;
      }
    }

    private Unit OutputChecked(bool isChecked)
    {
      if (isChecked)
      {
        OutputVariable = new PythonVariableConfiguration
        {
          VariableType = typeof(object),
          VariableName = "Output"
        };
      }
      else
      {
        OutputVariable = null;
      }

      return new Unit();
    }

    [JsonIgnore]
    [JsonProperty("Config")]
    public PythonProcessConfiguration Config { get; set; }

    public bool Configured
    {
      get => _configured;
      set
      {
        if (value)
        {
          WriteConfiguration();
        }
        this.RaiseAndSetIfChanged(ref _configured, value);
      }
    }

    private void WriteConfiguration()
    {
      _configWriter.Write(this);
    }

    public ReactiveCommand<bool, Unit> OutputCheckedCommand { get; set; }

    [JsonProperty("ProcessDefinition")]
    public KeyValuePair<string, List<object>> ProcessDefinition { get; set; }

    [JsonIgnore]
    [JsonProperty("InputVariables")]
    public List<PythonVariableConfiguration> InputVariables { get; set; }

    [JsonIgnore]
    [JsonProperty("OutputVariable")]
    public PythonVariableConfiguration OutputVariable
    {
      get => _outputVariable;
      set => this.RaiseAndSetIfChanged(ref _outputVariable, value);
    }

    // assumes void inputs

    public override async Task StartComponent(IList<IProcessData> inputs)
    {
      string defname = ComponentName.Substring(ComponentName.LastIndexOf(".") + 1);

      //TODO: Fix the way things are called
      //      var res = await _bindings.RunPythonDef( ClassName, defname, ProcessDefinition.Value );
      var populatedProcessDefinitionValue = inputs.Select(input => input.Data).ToList();
      populatedProcessDefinitionValue.RemoveAt(populatedProcessDefinitionValue.Count - 1); // Remove the output
      await _bindings.RunPythonDef(ClassName, defname, populatedProcessDefinitionValue);
      var result = _bindings.LastResult;
      var output = inputs.LastOrDefault();
      if (result.GetType() != output.Type)
      {
        if (result is float fResult && output.Type == typeof(double))
        {
          var dResult = (double)fResult;
          output.Data = dResult;
        }
      }
      else
      {
        output.Data = result;
      }
    }

    public override string ComponentName
    {
      get => $"{_name}";
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    [JsonProperty("ClassName")]
    public string ClassName { get; }

    [JsonIgnore]
    [JsonProperty("Bindings")]
    public IPythonBindings Bindings => _bindings;

    public static string HumanReadableTypeString = "Python";

    public override string ToString()
    {
      return ComponentName;
    }
  }
}
