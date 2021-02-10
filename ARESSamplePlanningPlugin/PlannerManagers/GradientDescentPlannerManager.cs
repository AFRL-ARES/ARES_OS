using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ARESCore.PlanningSupport;
using AresSamplePlanningPlugin.Planners.GradientDescent;
using AresSamplePlanningPlugin.Planners.GradientDescent.Views;
using CommonServiceLocator;
using ReactiveUI;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AresSamplePlanningPlugin.PlannerManagers
{
  public class GradientDescentPlannerManager : ReactiveSubscriber, IAresPlannerManager
  {
    private IAresPlanner _planner;
    private int _numExpsToPlan;


    public GradientDescentPlannerManager()
    {
      var planners = ServiceLocator.Current.GetAllInstances<IAresPlanner>().ToArray();
      Planner = planners.FirstOrDefault(planner => planner is GradientDescentPlanner);
      PlannerTile = ServiceLocator.Current.GetInstance<GradientDescentPlannerView>();
    }
    public Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      Planner.NumExperimentsToPlan = NumExpsToPlan;
      return Planner.DoPlanning();
    }

    public IPlanningParameters TryParse(string currentDesc, string lineToken, bool tokenHasValue)
    {
      throw new System.NotImplementedException();
    }

    public IAresPlanner Planner
    {
      get => _planner;
      set => this.RaiseAndSetIfChanged(ref _planner, value);
    }

    public string PlannerName { get; set; } = "Gradient Descent";
    public UserControl PlannerTile { get; set; }
    public IPlanningParameters PlanningParameters { get; set; } = new GradientDescentPlanningParameters();

    public int NumExpsToPlan
    {
      get => _numExpsToPlan;
      set => this.RaiseAndSetIfChanged(ref _numExpsToPlan, value);
    }
  }
}
