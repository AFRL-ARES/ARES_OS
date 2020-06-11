using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.DisposePatternHelpers;
using Ninject;
using ReactiveUI;

namespace ARESCore.Experiment.UI.ViewModels
{
  public class BatchScriptReviewViewModel : BasicReactiveObjectDisposable
  {
    private ICampaign _campaign;
    private int _numEperimentsToRun;
    private int _replanInterval;
    private string _experimentScript;
    private string _closeoutScript;
    private string _interScript;

    public BatchScriptReviewViewModel()
    {
      CurrentCampaign = AresKernel._kernel.Get<ICampaign>();
      ExperimentScript = CurrentCampaign.ExpScript;
      CloseoutScript = CurrentCampaign.CampaignCloseScript;
      InterScript = CurrentCampaign.InterExpScript;
      SaveCloseoutScriptChangesCommand = ReactiveCommand.Create<Unit, Unit>( u => SaveCloseoutScriptChanges() );
      SaveExperimentScriptChangesCommand = ReactiveCommand.Create<Unit, Unit>( u => SaveExperimentScriptChanges() );
      SaveInterScriptChangesCommand = ReactiveCommand.Create<Unit, Unit>( u => SaveInterScriptChanges() );
    }

    public string InterScript
    {
      get => _interScript;
      set => this.RaiseAndSetIfChanged(ref _interScript, value);
    }

    public string CloseoutScript
    {
      get => _closeoutScript;
      set => this.RaiseAndSetIfChanged(ref _closeoutScript, value);
    }

    public string ExperimentScript
    {
      get => _experimentScript;
      set => this.RaiseAndSetIfChanged(ref _experimentScript, value);
    }

    public Unit SaveCloseoutScriptChanges()
    {
      _campaign.CampaignCloseScript = CloseoutScript;
      return new Unit();
    }

    public Unit SaveExperimentScriptChanges()
    {
      _campaign.ExpScript = ExperimentScript;
      return new Unit();
    }

    public Unit SaveInterScriptChanges()
    {
      _campaign.InterExpScript = InterScript;
      return new Unit();
    }

    public int NumEperimentsToRun
    {
      get => _numEperimentsToRun;
      set => this.RaiseAndSetIfChanged( ref _numEperimentsToRun, value );
    }

    public int ReplanInterval
    {
      get => _replanInterval;
      set => this.RaiseAndSetIfChanged(ref _replanInterval, value);
    }

    public ICampaign CurrentCampaign
    {
      get => _campaign;
      set => this.RaiseAndSetIfChanged(ref _campaign, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCloseoutScriptChangesCommand { get; set; }

    public ReactiveCommand<Unit, Unit> SaveInterScriptChangesCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SaveExperimentScriptChangesCommand { get; set; }
  }
}