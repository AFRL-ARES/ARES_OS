using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Results;
using ReactiveUI;

namespace ARESCore.Experiment.UI.ViewModels
{
  public class BatchResultsViewModel : BasicReactiveObjectDisposable
  {
    private ICampaignExecutionSummary _campaignExecutionSummary;

    public BatchResultsViewModel( ICampaignExecutionSummary campaignExecutionSummary )
    {
      CampaignExecutionSummary = campaignExecutionSummary;
    }

    public ICampaignExecutionSummary CampaignExecutionSummary
    {
      get => _campaignExecutionSummary;
      set => this.RaiseAndSetIfChanged( ref _campaignExecutionSummary, value );
    }
  }
}
