using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using ARESCore.PlanningSupport.Impl;
using ARESCore.UI;
using AresSamplePlanningPlugin.Planners.Parameters.Impl;
using ReactiveUI;
using System.Linq;
using System.Threading.Tasks;

namespace AresSamplePlanningPlugin.Planners.Simple
{
  public class SimplePlanner : AresPlanner
  {
    private readonly IAresConsole _console;
    private SamplePlanningParameters _parameters = new SamplePlanningParameters();
    private double _targetResult;

    public SimplePlanner(IAresConsole console)
    {
      _console = console;
      CanPlan = true;
    }

    public override Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      _console.WriteLine("Simple planner, targeting " + TargetResult);
      var plannedvals = string.Join(",", Parameters.Select(parameter => parameter.ScriptLabel));
      _console.WriteLine("Planning " + plannedvals);
      return Task.Run(async () =>
      {
        var inputs = new PlannedExperimentBatchInputs();
        var varNames = Parameters.Select(parameter => parameter.ScriptLabel).ToList();
        var varVals = Parameters.Select(parameter => parameter.Value).ToList();
        inputs.PlannedInputs.Add(new PlannedExperimentInputs(varNames, varVals));
        return (IPlannedExperimentBatchInputs)inputs;
      });
    }

    public SamplePlanningParameters Parameters
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
