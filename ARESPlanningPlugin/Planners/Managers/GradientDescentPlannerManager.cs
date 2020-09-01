using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using AresPlanningPlugin.Planners.GradientDescent.Views;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ARESCore.PlanningSupport;
using CommonServiceLocator;
using ReactiveUI;

namespace AresPlanningPlugin.Planners.PlannerManagers
{
   public class GradientDescentPlannerManager : BasicReactiveObjectDisposable, IAresPlannerManager
   {
      private IAresPlanner _planner;
      private int _numExpsToPlan;


      public GradientDescentPlannerManager(IAresPlanner[] planners)
      {
         Planner = planners.FirstOrDefault(planner => planner is GradientDescentPlanner);
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
      public UserControl PlannerTile { get; set; } = ServiceLocator.Current.GetInstance<GradientDescentPlannerView>();
      public IPlanningParameters PlanningParameters { get; set; } = new GradientDescentPlanningParameters();

      public int NumExpsToPlan
      {
         get => _numExpsToPlan;
         set => this.RaiseAndSetIfChanged(ref _numExpsToPlan, value);
      }
   }
}
