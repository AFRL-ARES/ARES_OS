using AresAdditivePlanningPlugin.Planners;
using AresAdditivePlanningPlugin.Planners.Views;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ARESCore.PlanningSupport;
using CommonServiceLocator;
using ReactiveUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AresAdditivePlanningPlugin.PlannerManagers
{
  public class SimplePlannerManager : BasicReactiveObjectDisposable, IAresPlannerManager
  {
    private int _numExpsToPlan;
    private IAresPlanner _planner;

    public SimplePlannerManager()
    {
      var planners = ServiceLocator.Current.GetAllInstances<IAresPlanner>().ToArray();
      Planner = planners.First(planner => planner is SimplePlanner);
      PlannerTile = ServiceLocator.Current.GetInstance<SimplePlannerView>();
    }

    public string PlannerName { get; set; } = "Simple";
    public UserControl PlannerTile { get; set; }
    public IPlanningParameters PlanningParameters { get; set; } = new SimplePlanningParameters();

    public int NumExpsToPlan
    {
      get => _numExpsToPlan;
      set => this.RaiseAndSetIfChanged(ref _numExpsToPlan, value);
    }
    public Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      return Planner.DoPlanning();
    }

    public IPlanningParameters TryParse(string currentDesc, string lineToken, bool tokenHasValue)
    {
      throw new NotImplementedException();
    }

    public IAresPlanner Planner
    {
      get => _planner;
      private set => this.RaiseAndSetIfChanged(ref _planner, value);
    }
  }
}
