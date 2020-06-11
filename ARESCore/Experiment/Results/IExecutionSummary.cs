using System;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Experiment.Results
{
  public interface IExecutionSummary : IBasicReactiveObjectDisposable
  {
    TimeSpan ExecutionDuration { get; set; }
    ExecutionStatus Status { get; set; }
  }
}
