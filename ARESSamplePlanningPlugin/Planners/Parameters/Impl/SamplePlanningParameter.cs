using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresSamplePlanningPlugin.Planners.Parameters.Impl
{
  public class SamplePlanningParameter : ReactiveSubscriber, ISamplePlanningParameter
  {
    private double _value;
    private bool _isPlanned;
    private double _max = 6.0;
    private double _min = 0.1;
    public string ScriptLabel { get; protected set; } = "VAL_SAMPLE";

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
