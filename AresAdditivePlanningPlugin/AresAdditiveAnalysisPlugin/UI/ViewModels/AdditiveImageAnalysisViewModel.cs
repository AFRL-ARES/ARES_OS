using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Results;
using ARESCore.Extensions;
using DynamicData.Binding;
using ReactiveUI;

namespace AresAdditiveAnalysisPlugin.UI.ViewModels
{
  public class AdditiveImageAnalysisViewModel : BasicReactiveObjectDisposable
  {
    private IObservableCollection<IExperimentExecutionSummary> _experimentSummaries = new ObservableCollectionExtended<IExperimentExecutionSummary>();
    private ICampaignExecutionSummary _campaignSummary;

    public AdditiveImageAnalysisViewModel(ICampaignExecutionSummary campaignSummary)
    {
      _campaignSummary = campaignSummary;
      //      _campaignSummary.ExperimentExecutionSummaries.CollectionChanged += ExperimentSummaryAdded;
      ExperimentSummaryCopies = _campaignSummary.ExperimentExecutionSummaries;
    }

    private void ExperimentSummaryAdded(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        Disposables.Add(e.NewItems.Cast<IExperimentExecutionSummary>().First()
          .WhenPropertyChanged(executionSummary => executionSummary.Result, false)
          .Take(1)
          .Subscribe(experimentSummary => ExperimentSummaryCopies.Add(experimentSummary.Sender)));

      }
    }

    //    public override void Dispose()
    //    {
    //      _campaignSummary.ExperimentExecutionSummaries.CollectionChanged -= ExperimentSummaryAdded;
    //      base.Dispose();
    //    }

    public IObservableCollection<IExperimentExecutionSummary> ExperimentSummaryCopies
    {
      get => _experimentSummaries;
      set => this.RaiseAndSetIfChanged(ref _experimentSummaries, value);
    }
  }
}
