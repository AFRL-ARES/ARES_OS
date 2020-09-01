using ARESCore.AnalysisSupport;
using ARESCore.PlanningSupport;

namespace ARESCore.Experiment.impl
{
  public class ARESExperimentBatch : IExperimentBatch
  {
    public IPlannedExperimentBatchInputs Inputs { get { return BatchInputs; } set { } }
    public IAresAnalyzer Analyzer { get; set; }
    public IPlannedExperimentBatchInputs BatchInputs { get; set; } = new PlannedExperimentBatchInputs();
    public IAresPlannerManager PlannerManager { get; set; } = null;


    public ARESExperimentBatch()
    {
      Inputs = null;
    }


  }
}
