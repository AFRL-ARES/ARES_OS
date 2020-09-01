using ARESCore.AnalysisSupport;
using ARESCore.PlanningSupport;

namespace ARESCore.Experiment
{
  public interface IExperimentBatch
  {
    IPlannedExperimentBatchInputs Inputs { get; set; }
    IAresAnalyzer Analyzer { get; set; }
    IAresPlannerManager PlannerManager { get; set; }
  }
}
