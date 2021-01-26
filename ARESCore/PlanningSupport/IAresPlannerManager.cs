using System.Threading.Tasks;
using System.Windows.Controls;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ARESCore.PluginSupport;

namespace ARESCore.PlanningSupport
{
  public interface IAresPlannerManager : IAresPlugin, IReactiveSubscriber
  {
    string PlannerName { get; set; }
    UserControl PlannerTile { get; set; }

    IPlanningParameters PlanningParameters { get; set; }
    int NumExpsToPlan { get; set; }

    Task<IPlannedExperimentBatchInputs> DoPlanning();
    IPlanningParameters TryParse( string currentDesc, string lineToken, bool tokenHasValue );
    IAresPlanner Planner { get; }
  }
}
