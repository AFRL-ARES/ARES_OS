using AresAdditiveDevicesPlugin.Experiment;
using AresAdditivePlanningPlugin.Planners.Parameters;
using ARESCore.Database.Filtering;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using CommonServiceLocator;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace AresAdditivePlanningPlugin.Planners.Views.ViewModels
{
  public class SimplePlannerViewModel : BasicReactiveObjectDisposable
  {
    private SimplePlanner _planner;
    //    private IExperimentParameters _experimentParameters;

    public SimplePlannerViewModel(IDbFilterManager filterManager, IToolpathParameters toolpathParameters)
    {
      var planners = ServiceLocator.Current.GetAllInstances<IAresPlanner>();
      Planner = (SimplePlanner)planners.First(planner => planner is SimplePlanner);
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
