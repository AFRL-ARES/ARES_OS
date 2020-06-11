using System;
using DynamicData.Binding;
using ReactiveUI;

namespace ARESCore.Experiment.Results.impl
{
  public class CampaignExecutionSummary : ExecutionSummary, ICampaignExecutionSummary
  {
    private TimeSpan _etc;
    private ObservableCollectionExtended<IExperimentExecutionSummary> _experimentResults = new ObservableCollectionExtended<IExperimentExecutionSummary>();

    public TimeSpan ETC
    {
      get => _etc;
      set => this.RaiseAndSetIfChanged( ref _etc, value );
    }

    public ObservableCollectionExtended<IExperimentExecutionSummary> ExperimentExecutionSummaries
    {
      get => _experimentResults;
      set => this.RaiseAndSetIfChanged( ref _experimentResults, value );
    }
  }
}
