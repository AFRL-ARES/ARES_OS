using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using CommonServiceLocator;

namespace ARESCore.PlanningSupport.Impl
{
  public class ManualPlannerManager : ReactiveSubscriber, IAresPlannerManager
  {
    public string PlannerName { get; set; } = "Manual";
    public UserControl PlannerTile { get; set; } = new ManualPlanningView();
    public IPlanningParameters PlanningParameters { get; set; } // null
    public int NumExpsToPlan { get; set; }

    public ManualPlannerManager()
    {
      Planner = ServiceLocator.Current.GetAllInstances<IAresPlanner>().First( planner => planner is ManualPlanner );
    }

    public Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      // nothing to do here.
      return null;
    }

    public IPlanningParameters TryParse( string currentDesc, string lineToken, bool tokenHasValue )
    {
      // nothing to do here.
      return null;
    }

    public IAresPlanner Planner { get; } // TODO Assign this?
  }
}
