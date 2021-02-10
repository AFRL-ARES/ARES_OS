using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using ARESCore.Database.Tables;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ARESCore.Experiment.impl;

namespace ARESCore.PlanningSupport.Impl
{
  public class ManualPlanner : ReactiveSubscriber, IAresPlanner
  {
    public string PlannerName { get; set; } = "Manual";
    public UserControl PlannerTile { get; set; } = new ManualPlanningView();

    public List<ExperimentEntity> PlanningDatabase { get; set; } = new List<ExperimentEntity>();
    public IPlannerStatus PlannerStatus { get; set; }

    public bool CanPlan { get; set; } = true;
    public IPlannedExperimentBatchInputs SeedExperimentBatchInputs { get; set; } = new PlannedExperimentBatchInputs();
    public int RequiredNumberOfSeedExperiments { get; set; } = 0;

    public int NumExperimentsToPlan { get; set; }

    public Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      throw new NotImplementedException();
    }
  }
}
