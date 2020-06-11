using System;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.Experiment.Results.impl
{
  public abstract class ExecutionSummary : BasicReactiveObjectDisposable, IExecutionSummary
  {
    private TimeSpan _executionDuration;
    private ExecutionStatus _status = ExecutionStatus.PENDING;

    public TimeSpan ExecutionDuration
    {
      get => _executionDuration;
      set => this.RaiseAndSetIfChanged( ref _executionDuration, value );
    }

    public ExecutionStatus Status
    {
      get => _status;
      set => this.RaiseAndSetIfChanged( ref _status, value );
    }
  }
}
