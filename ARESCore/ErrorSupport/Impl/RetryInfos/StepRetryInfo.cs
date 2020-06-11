using System.Collections.Generic;
using ARESCore.Commands;
using ARESCore.Experiment.Results;

namespace ARESCore.ErrorSupport.Impl.RetryInfos
{
  public class StepRetryInfo
  {
    public IAresCommand Step { get; set; }
    public List<string> SubList { get; set; }
    public IStepExecutionSummary StepResult { get; set; }
  }
}
