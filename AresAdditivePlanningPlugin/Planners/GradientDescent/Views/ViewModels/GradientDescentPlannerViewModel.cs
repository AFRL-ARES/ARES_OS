using AresAdditivePlanningPlugin.Planners.Parameters;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using CommonServiceLocator;
using MoreLinq;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace AresAdditivePlanningPlugin.Planners.GradientDescent.Views.ViewModels
{
  public class GradientDescentPlannerViewModel : BasicReactiveObjectDisposable
  {
    private GradientDescentPlanner _planner;

    public GradientDescentPlannerViewModel()
    {
      var planners = ServiceLocator.Current.GetAllInstances<IAresPlanner>();
      Planner = (GradientDescentPlanner)planners.First(planner => planner is GradientDescentPlanner);
      TogglePlannedCommand = ReactiveCommand.Create<IAdditivePlanningParameter>(TogglePlanned);
    }

    private void TogglePlanned(IAdditivePlanningParameter parameter)
    {
      Planner.Parameters.ForEach(additiveParameters =>
        additiveParameters.Where(p => p.ScriptLabel == parameter.ScriptLabel).ForEach(p => p.IsPlanned = !p.IsPlanned));
    }

    public ReactiveCommand<IAdditivePlanningParameter, Unit> TogglePlannedCommand { get; }

    public GradientDescentPlanner Planner
    {
      get => _planner;
      set => this.RaiseAndSetIfChanged(ref _planner, value);
    }
  }
}
