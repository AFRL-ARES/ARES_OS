using ARESCore.DisposePatternHelpers;

namespace ARESCore.Experiment
{
  public interface ICampaign : IReactiveSubscriber
  {
    int NumExperimentsToRun { get; set; }
    int ReplanInterval { get; set; }
    string ExpScript { get; set; }
    bool CanRun { get; set; }
    bool IsExecuting { get; set; }
    string InterExpScript { get; set; }
    string CampaignCloseScript { get; set; }
    byte CanRunMask { get; set; }
    bool InitiatingEStop { get; set; }
  }
}