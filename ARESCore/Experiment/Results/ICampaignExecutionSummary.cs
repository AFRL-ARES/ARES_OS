using System;
using DynamicData.Binding;

namespace ARESCore.Experiment.Results
{
  public interface ICampaignExecutionSummary : IExecutionSummary
  {
    TimeSpan ETC { get; set; }
    ObservableCollectionExtended<IExperimentExecutionSummary> ExperimentExecutionSummaries { get; set; }
  }
}
