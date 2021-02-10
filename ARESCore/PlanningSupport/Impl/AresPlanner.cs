using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARESCore.Database.Filtering;
using ARESCore.Database.Tables;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using DynamicData.Binding;
using Ninject;
using ReactiveUI;

namespace ARESCore.PlanningSupport.Impl
{
  public abstract class AresPlanner : ReactiveSubscriber, IAresPlanner
  {
    private IPlannerStatus _plannerStatus;
    private int _numExperimentsToPlan;
    private bool _canPlan;

    protected AresPlanner()
    {
      SubscribeAndValidateCanPlan();
    }

    protected virtual void SubscribeAndValidateCanPlan()
    {
      AresKernel._kernel.Get<IDbFilterManager>()
          .WhenPropertyChanged(dbFilterManager => dbFilterManager.LastFilterResult)
          .Subscribe(filterManager =>
          {
            CanPlan = filterManager.Value != null && filterManager.Value.Cast<object>().Any();
          });
    }

    public abstract Task<IPlannedExperimentBatchInputs> DoPlanning();

    public List<ExperimentEntity> PlanningDatabase { get; set; } = new List<ExperimentEntity>();

    public int NumExperimentsToPlan
    {
      get => _numExperimentsToPlan;
      set => this.RaiseAndSetIfChanged(ref _numExperimentsToPlan, value);
    }

    public IPlannerStatus PlannerStatus
    {
      get => _plannerStatus;
      set => this.RaiseAndSetIfChanged(ref _plannerStatus, value);
    }
    public virtual bool CanPlan
    {
      get => _canPlan;
      set => this.RaiseAndSetIfChanged(ref _canPlan, value);
    }

    public IPlannedExperimentBatchInputs SeedExperimentBatchInputs { get; set; } = new PlannedExperimentBatchInputs();

    public virtual int RequiredNumberOfSeedExperiments { get; set; } = 0;
  }
}
