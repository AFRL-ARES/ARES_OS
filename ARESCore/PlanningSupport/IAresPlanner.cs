using System.Collections.Generic;
using System.Threading.Tasks;
using ARESCore.Database.Tables;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;

namespace ARESCore.PlanningSupport
{
  public interface IAresPlanner : IReactiveSubscriber
  {
    int NumExperimentsToPlan { get; set; }
    Task<IPlannedExperimentBatchInputs> DoPlanning();

    List<ExperimentEntity> PlanningDatabase { get; set; }

    IPlannerStatus PlannerStatus { get; set; }
    bool CanPlan { get; set; }
    IPlannedExperimentBatchInputs SeedExperimentBatchInputs { get; set; }
    int RequiredNumberOfSeedExperiments { get; set; }
  }
}
