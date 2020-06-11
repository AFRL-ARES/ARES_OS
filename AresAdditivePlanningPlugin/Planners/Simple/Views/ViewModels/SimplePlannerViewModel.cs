using System.Linq;
using System.Reactive;
using AresAdditiveDevicesPlugin.Experiment;
using AresAdditivePlanningPlugin.Planners.Parameters;
using ARESCore.Database.Filtering;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using ReactiveUI;

namespace AresAdditivePlanningPlugin.Planners.Views.ViewModels
{
  public class SimplePlannerViewModel : BasicReactiveObjectDisposable
  {
    private SimplePlanner _planner;
    //    private IExperimentParameters _experimentParameters;

    public SimplePlannerViewModel(IAresPlanner[] planners, IDbFilterManager filterManager, IToolpathParameters toolpathParameters)
    {
      Planner = (SimplePlanner)planners.FirstOrDefault(planner => planner is SimplePlanner);
      //      ExperimentParameters = new ExperimentParameters();
      //      ExperimentParameters.AddRange(toolpathParameters.Select(toolpathParam => new ExperimentParameter(toolpathParam.Value)));

      TogglePlannedCommand = ReactiveCommand.Create<IAdditivePlanningParameter>(TogglePlanned);
    }

    private void TogglePlanned(IAdditivePlanningParameter parameter)
    {
      parameter.IsPlanned = !parameter.IsPlanned;
    }

    public ReactiveCommand<IAdditivePlanningParameter, Unit> TogglePlannedCommand { get; }

    public SimplePlanner Planner
    {
      get => _planner;
      private set => this.RaiseAndSetIfChanged(ref _planner, value);
    }

    //    public IExperimentParameters ExperimentParameters
    //    {
    //      get => _experimentParameters;
    //      set => this.RaiseAndSetIfChanged(ref _experimentParameters, value);
    //    }
  }
}
