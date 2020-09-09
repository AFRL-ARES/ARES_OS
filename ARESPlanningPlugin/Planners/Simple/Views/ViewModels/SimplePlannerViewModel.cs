using System.Linq;
using System.Reactive;
using ARESCore.Database.Filtering;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using AresPlanningPlugin.Planners;
using ReactiveUI;

namespace AresPlanningPlugin.Planners.Views.ViewModels
{
  public class SimplePlannerViewModel : BasicReactiveObjectDisposable
  {
    private SimplePlanner _planner;
    //    private IExperimentParameters _experimentParameters;

    public SimplePlannerViewModel(IAresPlanner[] planners, IDbFilterManager filterManager)
    {
      Planner = (SimplePlanner)planners.FirstOrDefault(planner => planner is SimplePlanner);
      //      ExperimentParameters = new ExperimentParameters();
      //      ExperimentParameters.AddRange(toolpathParameters.Select(toolpathParam => new ExperimentParameter(toolpathParam.Value)));

    }


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
