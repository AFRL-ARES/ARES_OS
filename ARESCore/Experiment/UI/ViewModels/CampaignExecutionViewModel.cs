using System.Collections.Specialized;
using System.Reactive;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Results;
using ARESCore.UI;
using NationalInstruments.Restricted;
using ReactiveUI;

namespace ARESCore.Experiment.UI.ViewModels
{
  public class CampaignExecutionViewModel : BasicReactiveObjectDisposable
  {
    private ICampaign _campaign;
    private readonly ICampaignExecutor _campaignExecutor;
    private IExperimentExecutionSummary _currentExperimentExecutionSummary;
    private IStepExecutionSummary _currentStepExecutionSummary;
    private ICampaignExecutionSummary _campaignExecutionSummary;
    private bool _shouldDisplay;

    public CampaignExecutionViewModel( ICampaign campaign, ICampaignExecutor campaignExector, ICampaignExecutionSummary campaignExecutionSummary )
    {
      CampaignExecutionSummary = campaignExecutionSummary;
      Campaign = campaign;
      _campaignExecutor = campaignExector;
      ExecuteCampaignCommand = ReactiveCommand.CreateFromTask( ExecuteCampaign );
      CampaignExecutionSummary.ExperimentExecutionSummaries.CollectionChanged += ExperimentResultsCollectionChanged;
      InitializeEStopCommand = ReactiveCommand.CreateFromTask( InitializeEStop );
    }

    private Task InitializeEStop()
    {
      Campaign.InitiatingEStop = true;
      return Task.CompletedTask;
    }

    private void ExperimentResultsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
      if ( e.Action == NotifyCollectionChangedAction.Add )
      {
        if ( CurrentExperimentExecutionSummary != null )
        {
          CurrentExperimentExecutionSummary.StepExecutionSummaries.CollectionChanged -= StepResultsOnCollectionChanged;
        }
        CurrentExperimentExecutionSummary = CampaignExecutionSummary.ExperimentExecutionSummaries[e.NewStartingIndex];
        CurrentExperimentExecutionSummary.StepExecutionSummaries.CollectionChanged += StepResultsOnCollectionChanged;
        if ( !ShouldDisplay )
        {
          ShouldDisplay = true;
        }
      }
      else if ( e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset )
      {
        if ( CampaignExecutionSummary.ExperimentExecutionSummaries.IsEmpty() )
        {
          ShouldDisplay = false;
        }
      }
    }

    private void StepResultsOnCollectionChanged( object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs )
    {
      if ( notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add )
      {
        CurrentStepExecutionSummary = CurrentExperimentExecutionSummary.StepExecutionSummaries[notifyCollectionChangedEventArgs.NewStartingIndex];
      }
    }

    public override void Dispose()
    {
      base.Dispose();
      CampaignExecutionSummary.ExperimentExecutionSummaries.CollectionChanged -= ExperimentResultsCollectionChanged;
      CurrentExperimentExecutionSummary.StepExecutionSummaries.CollectionChanged -= StepResultsOnCollectionChanged;
    }

    private Task ExecuteCampaign()
    {
      var task = _campaignExecutor.Execute();
      if ( task.IsCanceled )
      {
        // canceled....
      }
      if ( task.IsFaulted )
      {
        CommonServiceLocator.ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine("Campaign Execution Faulted. Try checking number of experiments to run and replan interval");
      }
      if ( task.IsCompleted )
      {
        // yay!
      }
      _campaignExecutor.ShouldExecute = true; // reset after stop behavior
      return Task.CompletedTask;
    }

    public ICampaign Campaign
    {
      get => _campaign;
      set => this.RaiseAndSetIfChanged( ref _campaign, value );
    }

    public IExperimentExecutionSummary CurrentExperimentExecutionSummary
    {
      get => _currentExperimentExecutionSummary;
      set => this.RaiseAndSetIfChanged( ref _currentExperimentExecutionSummary, value );
    }

    public IStepExecutionSummary CurrentStepExecutionSummary
    {
      get => _currentStepExecutionSummary;
      set => this.RaiseAndSetIfChanged( ref _currentStepExecutionSummary, value );
    }

    public ICampaignExecutionSummary CampaignExecutionSummary
    {
      get => _campaignExecutionSummary;
      set => this.RaiseAndSetIfChanged( ref _campaignExecutionSummary, value );
    }

    public bool ShouldDisplay
    {
      get => _shouldDisplay;
      set => this.RaiseAndSetIfChanged( ref _shouldDisplay, value );
    }

    public ReactiveCommand<Unit, Unit> ExecuteCampaignCommand { get; }
    public ReactiveCommand<Unit, Unit> InitializeEStopCommand { get; }
  }
}
