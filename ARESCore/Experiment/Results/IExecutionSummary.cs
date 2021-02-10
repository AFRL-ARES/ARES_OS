using System;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Experiment.Results
{
  public interface IExecutionSummary : IReactiveSubscriber
  {
    TimeSpan ExecutionDuration { get; set; }
    ExecutionStatus Status { get; set; }
  }
}
