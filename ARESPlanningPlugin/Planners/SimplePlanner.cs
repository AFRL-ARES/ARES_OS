using System.Linq;
using System.Threading.Tasks;
using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using ARESCore.PlanningSupport.Impl;
using CommonServiceLocator;
using ReactiveUI;

namespace AresPlanningPlugin.Planners.Simple
{
  public class SimplePlanner : AresPlanner
  {
    private PlanningParameters _parameters = new PlanningParameters();
    private double _targetResult;

    public SimplePlanner()
    {
      CanPlan = true;
    }

    public override Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      return Task.Run(async () =>
      {
        var plannedParameters = Parameters.Where(parameter => parameter.IsPlanned);
          string.Join(",", plannedParameters.Select(parameter => parameter.ScriptLabel)));

        var inputs = new PlannedExperimentBatchInputs();
        var varNames = Parameters.Select(parameter => parameter.ScriptLabel).ToList();
        var varVals = Parameters.Select(parameter => parameter.Value).ToList();
        inputs.PlannedInputs.Add(new PlannedExperimentInputs(varNames, varVals));
        return (IPlannedExperimentBatchInputs)inputs;
      });
    }

    public PlanningParameters Parameters
    {
      get => _parameters;
      set => this.RaiseAndSetIfChanged(ref _parameters, value);
    }

    public double TargetResult
    {
      get => _targetResult;
      set => this.RaiseAndSetIfChanged(ref _targetResult, value);
    }
  }
}
