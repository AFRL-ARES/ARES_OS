using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController;
using AresAdditivePlanningPlugin.Planners.Parameters.Impl;
using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using ARESCore.PlanningSupport.Impl;
using CommonServiceLocator;
using ReactiveUI;

namespace AresAdditivePlanningPlugin.Planners
{
  public class SimplePlanner : AresPlanner
  {
    private AdditivePlanningParameters _parameters = new AdditivePlanningParameters();
    private double _targetResult;

    public SimplePlanner()
    {
      CanPlan = true;
    }

    public override Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      return Task.Run(async () =>
      {
        var stageController = ServiceLocator.Current.GetInstance<IStageController>();
        await stageController.WritePyDict("data.Planner", "Simple");
        await stageController.WritePyDict("data.Target", TargetResult.ToString());
        var plannedParameters = Parameters.Where(parameter => parameter.IsPlanned);
        await stageController.WritePyDict("data.PlannedParams",
          string.Join(",", plannedParameters.Select(parameter => parameter.ScriptLabel)));

        var inputs = new PlannedExperimentBatchInputs();
        var varNames = Parameters.Select(parameter => parameter.ScriptLabel).ToList();
        var varVals = Parameters.Select(parameter => parameter.Value).ToList();
        inputs.PlannedInputs.Add(new PlannedExperimentInputs(varNames, varVals));
        return (IPlannedExperimentBatchInputs)inputs;
      });
    }

    public AdditivePlanningParameters Parameters
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
