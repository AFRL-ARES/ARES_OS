using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Scripting;
using ARESCore.PlanningSupport;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;

namespace ARESCore.Experiment
{
  public class Campaign : ReactiveSubscriber, ICampaign
  {
    private string _expScript;
    private string _interExpScript;
    private string _campaignCloseScript;
    private int _numExpToRun;
    private int _replanInterval;
    private IPlannedExperimentBatchInputs _batchInputs;
    private bool _canRun = true;
    private bool _isExecuting;
    private bool _initiatingEStop;
    private readonly ISelectedPlannersRepository _selectedPlanners;
    private byte _canRunMask = 0b10000000; // Assign 1 to bits that are not checked. Yes I know its better to just inverse everything but I already started it this way
    private readonly IScriptExecutor _scriptExecutor;
    private readonly IPlanResults _planResults;

    public Campaign(ISelectedPlannersRepository selectedPlanners, IPlanResults planResults, IScriptExecutor scriptExecutor)
    {
      NumExperimentsToRun = 1; // Assign the default here for CanRun validation check
      IsExecuting = false;
      _planResults = planResults;
      _scriptExecutor = scriptExecutor;
      _selectedPlanners = selectedPlanners;
      _selectedPlanners.PropertyChanged += SelectedPlannersChanged;
      planResults.WhenAnyValue(pResults => pResults.Results).Subscribe(plans => PlanResultsChanged(planResults));

    }

    private void PlanResultsChanged(IPlanResults planResults)
    {
      var planResultsValid = planResults.Results != null && planResults.Results.PlannedInputs.Any();
      CanRunMask = (byte)(planResultsValid ? CanRunMask | (byte)CampaignCanRunMask.PlanResults : CanRunMask & (byte)CampaignCanRunMask.NoPlanResults);
      if (planResultsValid)
      {
        var expScriptValid = _scriptExecutor.Validate(ExpScript, planResults.Results.PlannedInputs.FirstOrDefault()); // FirstOrDefault may not be the best choice
        CanRunMask = (byte)(expScriptValid ? CanRunMask | (byte)CampaignCanRunMask.ValidExperimentScript : CanRunMask & (byte)CampaignCanRunMask.InvalidExperimentScript);

        var interScriptValid = _scriptExecutor.Validate(InterExpScript, planResults.Results.PlannedInputs.FirstOrDefault()); // FirstOrDefault may not be the best choice
        CanRunMask = (byte)(interScriptValid ? CanRunMask | (byte)CampaignCanRunMask.ValidInterScript : CanRunMask & (byte)CampaignCanRunMask.InvalidInterScript);

        var closeScriptValid = _scriptExecutor.Validate(CampaignCloseScript, planResults.Results.PlannedInputs.FirstOrDefault()); // FirstOrDefault may not be the best choice
        CanRunMask = (byte)(closeScriptValid ? CanRunMask | (byte)CampaignCanRunMask.ValidCloseScript : CanRunMask & (byte)CampaignCanRunMask.InvalidCloseScript);
      }
    }

    private void SelectedPlannersChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      CanRunMask = (byte)(_selectedPlanners.Any() ? CanRunMask | (byte)CampaignCanRunMask.Planner : CanRunMask & (byte)CampaignCanRunMask.NoPlanner);

    }

    public override void Dispose()
    {
      _selectedPlanners.PropertyChanged -= SelectedPlannersChanged;
    }

    public int NumExperimentsToRun
    {
      get => _numExpToRun;
      set
      {
        this.RaiseAndSetIfChanged(ref _numExpToRun, value);
        CanRunMask = (byte)(_numExpToRun > 0 ? CanRunMask | (byte)CampaignCanRunMask.Experiments : CanRunMask & (byte)CampaignCanRunMask.NoExperiments);
      }
    }

    public int ReplanInterval
    {
      get => _replanInterval;
      set => this.RaiseAndSetIfChanged(ref _replanInterval, value);
    }

    public IPlannedExperimentBatchInputs BatchInputs
    {
      get => _batchInputs;
      set => this.RaiseAndSetIfChanged(ref _batchInputs, value);
    }

    public byte CanRunMask
    {
      get => _canRunMask;
      set
      {
        this.RaiseAndSetIfChanged(ref _canRunMask, value);
        CanRun = (_canRunMask & byte.MaxValue) == byte.MaxValue;
      }
    }

    public bool CanRun
    {
      get => _canRun;
      set => this.RaiseAndSetIfChanged(ref _canRun, value);
    }

    public bool IsExecuting
    {
      get => _isExecuting;
      set
      {
        this.RaiseAndSetIfChanged(ref _isExecuting, value);
        CanRunMask = (byte)(_isExecuting ? CanRunMask & (byte)CampaignCanRunMask.CampaignRunning : CanRunMask | (byte)CampaignCanRunMask.CampaignPending);
      }
    }

    public string ExpScript
    {
      get => _expScript;
      set
      {
        this.RaiseAndSetIfChanged(ref _expScript, value);

        var expScriptValid = _scriptExecutor.Validate(ExpScript, _planResults.Results?.PlannedInputs?.FirstOrDefault()); // FirstOrDefault may not be the best choice
        CanRunMask = (byte)(expScriptValid ? CanRunMask | (byte)CampaignCanRunMask.ValidExperimentScript : CanRunMask & (byte)CampaignCanRunMask.InvalidExperimentScript);
      }
    }

    public string InterExpScript
    {
      get => _interExpScript;
      set
      {
        this.RaiseAndSetIfChanged(ref _interExpScript, value);

        var interScriptValid = _scriptExecutor.Validate(InterExpScript, _planResults.Results?.PlannedInputs?.FirstOrDefault()); // FirstOrDefault may not be the best choice
        CanRunMask = (byte)(interScriptValid ? CanRunMask | (byte)CampaignCanRunMask.ValidInterScript : CanRunMask & (byte)CampaignCanRunMask.InvalidInterScript);
      }
    }

    public string CampaignCloseScript
    {
      get => _campaignCloseScript;
      set
      {
        this.RaiseAndSetIfChanged(ref _campaignCloseScript, value);

        var closeScriptValid = _scriptExecutor.Validate(CampaignCloseScript, _planResults.Results?.PlannedInputs?.FirstOrDefault()); // FirstOrDefault may not be the best choice
        CanRunMask = (byte)(closeScriptValid ? CanRunMask | (byte)CampaignCanRunMask.ValidCloseScript : CanRunMask & (byte)CampaignCanRunMask.InvalidCloseScript);
      }
    }


    public bool InitiatingEStop
    {
      get => _initiatingEStop;
      set
      {
        _initiatingEStop = value;
        this.RaisePropertyChanged();
      }
    }

  }
}