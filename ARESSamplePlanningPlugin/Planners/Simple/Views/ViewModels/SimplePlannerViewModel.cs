using ARESCore.Database.Filtering;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using AresSamplePlanningPlugin.Planners.Parameters;
using CommonServiceLocator;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace AresSamplePlanningPlugin.Planners.Simple.Views.ViewModels
{
  public class SimplePlannerViewModel : ReactiveSubscriber
  {
    private SimplePlanner _planner;

    public SimplePlannerViewModel(IDbFilterManager filterManager)
    {
      var planners = ServiceLocator.Current.GetAllInstances<IAresPlanner>();
      Planner = (SimplePlanner)planners.First(planner => planner is SimplePlanner);

      TogglePlannedCommand = ReactiveCommand.Create<ISamplePlanningParameter>(TogglePlanned);
    }

    private void TogglePlanned(ISamplePlanningParameter parameter)
    {
      parameter.IsPlanned = !parameter.IsPlanned;
    }

    public ReactiveCommand<ISamplePlanningParameter, Unit> TogglePlannedCommand { get; }

    public SimplePlanner Planner
    {
      get => _planner;
      private set => this.RaiseAndSetIfChanged(ref _planner, value);
    }
  }
}
