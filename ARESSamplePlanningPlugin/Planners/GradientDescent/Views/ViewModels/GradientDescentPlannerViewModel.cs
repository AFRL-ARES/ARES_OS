using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using AresSamplePlanningPlugin.Planners.Parameters;
using CommonServiceLocator;
using MoreLinq;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace AresSamplePlanningPlugin.Planners.GradientDescent.Views.ViewModels
{
  public class GradientDescentPlannerViewModel : ReactiveSubscriber
  {
    private GradientDescentPlanner _planner;

    public GradientDescentPlannerViewModel()
    {
      var planners = ServiceLocator.Current.GetAllInstances<IAresPlanner>();
      Planner = (GradientDescentPlanner)planners.First(planner => planner is GradientDescentPlanner);
      TogglePlannedCommand = ReactiveCommand.Create<ISamplePlanningParameter>(TogglePlanned);
    }

    private void TogglePlanned(ISamplePlanningParameter parameter)
    {
      Planner.Parameters.ForEach(additiveParameters =>
        additiveParameters.Where(p => p.ScriptLabel == parameter.ScriptLabel).ForEach(p => p.IsPlanned = !p.IsPlanned));
    }

    public ReactiveCommand<ISamplePlanningParameter, Unit> TogglePlannedCommand { get; }

    public GradientDescentPlanner Planner
    {
      get => _planner;
      set => this.RaiseAndSetIfChanged(ref _planner, value);
    }
  }
}
