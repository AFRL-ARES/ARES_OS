using ARESCore.Experiment;
using ARESCore.Experiment.Results;

namespace ARESCore.ErrorSupport.Impl.RetryInfos
{
  public class ExperimentRetryInfo
  {
    public IPlannedExperimentInputs Inputs { get; set; }
    public IExperimentExecutionSummary ExperimentResult { get; set; }
  }
}
