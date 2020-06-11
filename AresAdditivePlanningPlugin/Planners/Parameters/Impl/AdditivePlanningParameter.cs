using System;
using System.Linq;
using AresAdditiveDevicesPlugin.Experiment;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using MoreLinq;
using ReactiveUI;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class AdditivePlanningParameter : BasicReactiveObjectDisposable, IAdditivePlanningParameter
  {
    private double _value;
    private bool _isPlanned;
    private double _max;
    private double _min;
    public string ScriptLabel { get; protected set; }
    public string PythonLabel { get; protected set; }

    public virtual void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      Value = toolpathParams.First(varEntry => varEntry.Key.Equals(PythonLabel, StringComparison.CurrentCultureIgnoreCase))
        .Value.Value;
    }

    public double Value
    {
      get => _value;
      set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public bool IsPlanned
    {
      get => _isPlanned;
      set => this.RaiseAndSetIfChanged(ref _isPlanned, value);
    }

    public double Min
    {
      get => _min;
      set => this.RaiseAndSetIfChanged(ref _min, value);
    }

    public double Max
    {
      get => _max;
      set => this.RaiseAndSetIfChanged(ref _max, value);
    }
  }
}
