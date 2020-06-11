using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Experiment.Impl;
//using AresAdditiveDevicesPlugin.Planners;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using ARESCore.DisposePatternHelpers;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.Experiment
{
  public interface IExperimentCampaign : IBasicReactiveObjectDisposable
  {
    ObservableCollectionExtended<ITerminationCondition> TerminationConditions { get; }

    //    IPlanner Planner { get; set; }

    BasicUserDefinedComponent AnalysisComponent { get; set; }

    ICampaignSetup CampaignSetup { get; set; }

    bool CanRun { get; set; }

    bool Running { get; set; }

    Task RunCampaign();
    Task StopCampaign();

    ObservableCollection<ExperimentStatus> StatusText { get; set; }
    int CurrentExperimentIdx { get; set; }
    string LastStepResults { get; set; }
    ExperimentStatus CurrentStatus { get; set; }

  }
}
