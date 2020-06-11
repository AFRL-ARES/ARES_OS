using AresAdditiveDevicesPlugin.Processing.Json;
using ARESCore.DisposePatternHelpers;
using DynamicData.Binding;
using Humanizer;
using log4net;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base
{
  public abstract class BasicComponent : BasicReactiveObjectDisposable, IComponent
  {
    [JsonIgnore]
    public readonly ILog Log;

    protected IProcessData _output;
    protected IObservableCollection<IProcessData> _inputs = new ObservableCollectionExtended<IProcessData>();
    protected string _name = "";
    protected static string _typeString = "Basic";
    protected bool _isComplete = false;

    protected BasicComponent()
    {
      Log = LogManager.GetLogger(GetType());
    }

    protected async Task ActualStartComponent(Action process)
    {
      try
      {

        await Task.Run(process);
      }
      catch (Exception)
      {
        // ignored
      }
      IsComplete = true;
    }

    /// <inheritdoc />
    [JsonProperty("Id")]
    public int Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("ComponentName")]
    public virtual string ComponentName
    {
      get => _name;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    /// <inheritdoc />
    public bool CanAutoStart { get; private set; }

    public abstract Task StartComponent(IList<IProcessData> inputs);

    [JsonProperty("Output")]
    public IProcessData Output
    {
      get => _output;
      protected set => this.RaiseAndSetIfChanged(ref _output, value);
    }

    [JsonProperty("DefaultInputs")]
    [JsonConverter(typeof(CustomJsonConverter))]
    public IObservableCollection<IProcessData> DefaultInputs
    {
      get => _inputs;
      set => this.RaiseAndSetIfChanged(ref _inputs, value);
    }

    public bool IsComplete
    {
      get => _isComplete;
      set => this.RaiseAndSetIfChanged(ref _isComplete, value);
    }

    public override string ToString()
    {
      return ComponentName.Humanize();
    }
  }
}
