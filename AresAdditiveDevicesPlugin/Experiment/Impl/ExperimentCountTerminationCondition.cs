using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class ExperimentCountTerminationCondition : BasicReactiveObjectDisposable, ITerminationCondition
  {
    private int _experimentTarget;
    private bool _targetisUpperBound;

    public ExperimentCountTerminationCondition()
    {
      TerminationType = TerminationConditionType.ExperimentCount;
    }

    public TerminationConditionType TerminationType { get; set; }

    public int ExperimentTarget
    {
      get => _experimentTarget;
      set => this.RaiseAndSetIfChanged(ref _experimentTarget, value);
    }

    /// <summary>
    /// Whether the target value is the upper bound of experiments to run. In other words,
    /// This is true if we want to run "no more than" the number. This is false if we
    /// want to run "at least" the number.
    /// </summary>
    public bool TargetisUpperBound
    {
      get => _targetisUpperBound;
      set => this.RaiseAndSetIfChanged(ref _targetisUpperBound, value);
    }
  }
}
