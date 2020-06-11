using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class TargetValueTerminationCondition : BasicReactiveObjectDisposable, ITerminationCondition
  {
    private double _targetValue;
    private bool _terminateWhenLessThanTarget;
    private bool _terminateWhenGreaterThanTarget;
    private bool _terminateWhenEqualToTarget;
    private double _targetPrecision;

    public TargetValueTerminationCondition()
    {
      TerminationType = TerminationConditionType.TargetValue;
      TerminateWhenGreaterThanTarget = true;
    }

    public TerminationConditionType TerminationType { get; set; }

    public double TargetValue
    {
      get => _targetValue;
      set => this.RaiseAndSetIfChanged(ref _targetValue, value);
    }

    public bool TerminateWhenLessThanTarget
    {
      get => _terminateWhenLessThanTarget;
      set => this.RaiseAndSetIfChanged(ref _terminateWhenLessThanTarget, value);
    }

    public bool TerminateWhenGreaterThanTarget
    {
      get => _terminateWhenGreaterThanTarget;
      set => this.RaiseAndSetIfChanged(ref _terminateWhenGreaterThanTarget, value);
    }

    public bool TerminateWhenEqualToTarget
    {
      get => _terminateWhenEqualToTarget;
      set => this.RaiseAndSetIfChanged(ref _terminateWhenEqualToTarget, value);
    }

    public double TargetPrecision
    {
      get => _targetPrecision;
      set => this.RaiseAndSetIfChanged(ref _targetPrecision, value);
    }
  }
}
