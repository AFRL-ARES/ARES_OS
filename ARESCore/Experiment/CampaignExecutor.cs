using ARESCore.DisposePatternHelpers;
using ARESCore.ErrorSupport;
using ARESCore.ErrorSupport.Impl;
using ARESCore.ErrorSupport.Impl.RetryInfos;
using ARESCore.Experiment.Results;
using ARESCore.Experiment.Scripting;
using ARESCore.Extensions;
using ARESCore.PlanningSupport;
using ARESCore.UI;
using ARESCore.UserSession;
using CommonServiceLocator;
using DynamicData.Binding;
using MoreLinq;
using Ninject;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ARESCore.Experiment
{
  public class CampaignExecutor : ReactiveSubscriber, ICampaignExecutor
  {
    private readonly List<CampaignRetryInfo> _campaignRetryInfos = new List<CampaignRetryInfo>();
    private readonly List<ExperimentRetryInfo> _experimentRetryInfos = new List<ExperimentRetryInfo>();
    private readonly ICampaign _campaign;
    private readonly ISelectedPlannersRepository _selectedPlanners;
    private readonly IPlanResults _planResults;
    private readonly IScriptExecutor _scriptExecutor;
    private readonly ICurrentConfig _currentConfig;
    private ICampaignExecutionSummary _campaignExecutionSummary;
    private IDisposable _campaignElapsedTimer;
    private IDisposable _campaignRemainingTimer;
    private bool _isComplete;
    private IErrorable _previousErrored = null;
    private bool _shouldExecute = true;
    private bool _shouldContinue = false;
    private bool _terminated;
    private string _newCampaignPath = String.Empty;

    public CampaignExecutor(ICampaign campaign, ISelectedPlannersRepository selectedPlanners,
      IPlanResults planResults, IScriptExecutor scriptExecutor,
      ICurrentConfig currentConfig, ICampaignExecutionSummary campaignExecutionSummary)
    {
      _campaign = campaign;
      _selectedPlanners = selectedPlanners;
      _planResults = planResults;
      _scriptExecutor = scriptExecutor;
      _currentConfig = currentConfig;
      CampaignExecutionSummary = campaignExecutionSummary;
    }

    public async Task Execute()
    {
      _campaignRetryInfos.Clear();
      _terminated = false;
      _shouldExecute = true;
      var campaignErrorSub = this.Subscribe(campaignExecutor => campaignExecutor.Error, campaign => OnFail(this, null, null)); // TODO: handle this weird logic

      CampaignExecutionSummary.ExperimentExecutionSummaries.Clear();
      if (!DoPreRunChecks())
      {
        return;
      }

      await CreateFiles();
      _campaignExecutionSummary.ETC = TimeSpan.Zero;

      for (var experimentIdx = 0; experimentIdx < _campaign.NumExperimentsToRun; experimentIdx++)
      {
        if (_terminated)
        {
          break;
        }
        if (!_shouldExecute)
        {
          continue;
        }
        await BuildAndExecuteExperiment(experimentIdx);
      }
      await _scriptExecutor.Run(_campaign.CampaignCloseScript, null);
      ShutdownCampaign();

      while (_campaignRetryInfos.Any() && !_terminated) // This might cause a deadlock since Retry is going to call this Execute() function
      {
        await Task.Delay(500);
      }
      campaignErrorSub.Dispose();
      _terminated = false;
      _shouldContinue = true;
      _shouldExecute = true;
      if (!_campaignRetryInfos.Any())
      {
        _previousErrored = null;
      }
    }

    private Task CreateFiles()
    {
      var now = DateTime.Now;
      var timeStamp = $"{now.Year:0000}{now.Month:00}{now.Day:00}{now.Hour:00}{now.Minute:00}{now.Second:00}";
      var currentDirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
      var omnibusRepo = currentDirInfo.Parent?.Parent?.Parent?.Parent;
      if (omnibusRepo == null)
      {
        ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine("Can't get to Omnibus directory");
      }

      var campaignsDir = omnibusRepo.GetDirectories()
        .FirstOrDefault(dir => dir.Name.Equals("Campaigns", StringComparison.CurrentCultureIgnoreCase))
        ?? Directory.CreateDirectory(omnibusRepo.FullName);
      var campaignsLocation =
        $@"{campaignsDir.FullName}";

      _newCampaignPath = $@"{campaignsLocation}\{timeStamp}";
      Directory.CreateDirectory(_newCampaignPath);
      _campaign.IsExecuting = true;
      //      _campaign.CanRun = false;
      // TODO: Set up the campaign. Turn on equipment, etc.
      _campaignExecutionSummary.Status = ExecutionStatus.EXECUTING;
      _campaignElapsedTimer = Observable.Interval(TimeSpan.FromSeconds(_currentConfig.TimerPrecision))
        .Subscribe(_ => _campaignExecutionSummary.ExecutionDuration = _campaignExecutionSummary.ExecutionDuration.Add(TimeSpan.FromSeconds(_currentConfig.TimerPrecision)));
      return Task.CompletedTask;
    }

    private async Task BuildAndExecuteExperiment(int experimentIdx)
    {
      Directory.CreateDirectory($@"{_newCampaignPath}\Experiment_{experimentIdx + 1:000}");
      if (experimentIdx > 0)
      {
        EstimateETC(experimentIdx);
      }


      var needsToExecuteSeedExperiment = _selectedPlanners
        .Select(planner => planner.Planner.RequiredNumberOfSeedExperiments).Any(requiredSeedCount => requiredSeedCount >= experimentIdx + 1);

      var batchExpNum = await SetPlanResults(experimentIdx, needsToExecuteSeedExperiment);

      if (!needsToExecuteSeedExperiment &&
                (_campaign.ReplanInterval != 0 && _planResults.Results.PlannedInputs.Count != _campaign.ReplanInterval)
                 || (_campaign.ReplanInterval == 0 && _planResults.Results.PlannedInputs.Count != _campaign.NumExperimentsToRun))
      {
        ShutdownCampaign();
        _campaignExecutionSummary.Status = ExecutionStatus.ERROR;
        // TODO: Handle with fail UI: "Incorrect number of experiments were generated by the planner"
        return;
      }

      var inputs = _planResults.Results.PlannedInputs[batchExpNum];

      await ShutdownIfNeeded(inputs);
      await ExecuteExperiment(experimentIdx, inputs);
    }


    private Task<int> SetPlanResults(int experimentIdx, bool needsToExecuteSeedExperiment)
    {
      var batchExpNum = experimentIdx;
      //      if (needsToExecuteSeedExperiment && _planResults.Results.HasInputs())
      //      {
      //        return Task.FromResult(batchExpNum);
      //      }

      _selectedPlanners.ForEach(selectedPlanner =>
      {
        if (needsToExecuteSeedExperiment)
        {
          if (!_planResults.Results.HasInputs())
          {
            _planResults.Results = selectedPlanner.DoPlanning().Result;
          }
          return;
        }
        var expNumber = experimentIdx + 1;
        if (expNumber <= selectedPlanner.Planner.RequiredNumberOfSeedExperiments || _campaign.ReplanInterval == 0)
        {
          batchExpNum = experimentIdx;
        }
        else
        {
          batchExpNum = (expNumber - selectedPlanner.Planner.RequiredNumberOfSeedExperiments) % _campaign.ReplanInterval;
        }
        if (batchExpNum == 0)
        {
          if (_campaign.ReplanInterval == 0)
          {
            selectedPlanner.NumExpsToPlan = _campaign.NumExperimentsToRun;
          }
          else
          {
            selectedPlanner.NumExpsToPlan = _campaign.ReplanInterval;
            //            if (!needsToExecuteSeedExperiment)
            //            if (!_planResults.Results.HasInputs())
            //            {
            _planResults.Results = selectedPlanner.DoPlanning().Result;
            //            }
          }
        }
      });
      return Task.FromResult(batchExpNum);
    }

    private async Task ExecuteExperiment(int experimentIdx, IPlannedExperimentInputs inputs)
    {
      var expExecutionSummary = AresKernel._kernel.Get<IExperimentExecutionSummary>();
      expExecutionSummary.ExperimentNumber = experimentIdx + 1;
      expExecutionSummary.Status = ExecutionStatus.EXECUTING;
      CampaignExecutionSummary.ExperimentExecutionSummaries.Add(expExecutionSummary);

      var expErrorSub = _campaign.Subscribe(campaign => campaign.Error, campaign => CreateAndAddErrorBundle(this, _campaign));
      try
      {
        await RunExperiment(inputs, expExecutionSummary);
        do
          await Task.Delay(500);
        while (_experimentRetryInfos.Any() && !_terminated);
        if (!_campaignRetryInfos.Any())
        {
          await _scriptExecutor.Run(_campaign.InterExpScript, inputs);
        }

      }
      catch (Exception)
      {
      }
    }

    private Task ShutdownIfNeeded(IPlannedExperimentInputs inputs)
    {
      if (!_scriptExecutor.Validate(_campaign.ExpScript, inputs))
      {
        ShutdownCampaign();
        _campaignExecutionSummary.Status = ExecutionStatus.ERROR;
        //TODO: Handle with fail UI: "Experiment Script Validation Failed"
        return Task.CompletedTask;
      }
      if (!_scriptExecutor.Validate(_campaign.InterExpScript, inputs))
      {
        ShutdownCampaign();
        _campaignExecutionSummary.Status = ExecutionStatus.ERROR;
        //TODO: Handle with fail UI: "Inter-Experiment Script Validation Failed"
        return Task.CompletedTask;
      }
      if (!_scriptExecutor.Validate(_campaign.CampaignCloseScript, inputs))
      {
        ShutdownCampaign();
        _campaignExecutionSummary.Status = ExecutionStatus.ERROR;
        //TODO: Handle with fail UI: "Campaign Closeout Script Validation Failed"
        return Task.CompletedTask;
      }
      return Task.CompletedTask;
    }

    private void OnFail(IErrorable errored, IExperimentExecutionSummary expExecutionSummary, IPlannedExperimentInputs experimentInputs) // expResult and experimentInputs are null for campaign to campaign handling
    {
      _previousErrored = errored;
      if (errored is ICampaignExecutor)
      {
        OnCampaignFail();
      }
      else if (errored is IScriptExecutor)
      {
        OnExpFail(experimentInputs, expExecutionSummary);
      }
    }

    private void OnExpFail(IPlannedExperimentInputs inputs, IExperimentExecutionSummary experimentExecutionSummary)
    {
      var expRetryInfo = new ExperimentRetryInfo { Inputs = inputs, ExperimentResult = experimentExecutionSummary };
      _experimentRetryInfos.Add(expRetryInfo);

      CreateAndAddErrorBundle(this, _scriptExecutor);
    }

    private void OnCampaignFail()
    {
      _campaignRemainingTimer?.Dispose();
      _campaignElapsedTimer.Dispose();
      CampaignExecutionSummary.ExecutionDuration = TimeSpan.Zero;
      CampaignExecutionSummary.ETC = TimeSpan.Zero;
      CampaignExecutionSummary.Status = ExecutionStatus.PENDING;
      var campaignRetryInfo = new CampaignRetryInfo { CampaignResult = CampaignExecutionSummary };
      _campaignRetryInfos.Add(campaignRetryInfo);

      CreateAndAddErrorBundle(this, this);
    }

    private async Task RunExperiment(IPlannedExperimentInputs inputs, IExperimentExecutionSummary expExecutionSummary)
    {
      //      var shouldContinue = false;
      var expTimer = Observable.Interval(TimeSpan.FromSeconds(_currentConfig.TimerPrecision))
        .Subscribe(_ => expExecutionSummary.ExecutionDuration = expExecutionSummary.ExecutionDuration.Add(TimeSpan.FromSeconds(_currentConfig.TimerPrecision)));
      _scriptExecutor.WhenPropertyChanged(expExec => expExec.IsComplete, false).Take(1).Subscribe(completion => ExperimentCompletionUpdated(ref _shouldContinue, completion.Value));
      var expErrorSub = _scriptExecutor.Subscribe(expExec => expExec.Error, expExec => OnFail(expExec, expExecutionSummary, inputs));
      try
      {
        await _scriptExecutor.Run(_campaign.ExpScript, inputs);
        do
          await Task.Delay(500);
        while (!_shouldContinue && _experimentRetryInfos.Any() && !_terminated); // IgnoreAndContinue causes a bug here
        expExecutionSummary.Status = ExecutionStatus.DONE;
      }
      catch (Exception)
      {
      }
      finally
      {
        expTimer.Dispose();
        expErrorSub.Dispose();
      }
    }

    private void ExperimentCompletionUpdated(ref bool shouldExpsContinue, bool completionValue)
    {
      shouldExpsContinue = completionValue;
    }

    private void EstimateETC(int experimentIdx)
    {
      _campaignRemainingTimer?.Dispose();
      var elapsedTime = _campaignExecutionSummary.ExecutionDuration;
      var experimentsRemaining = _campaign.NumExperimentsToRun - experimentIdx;
      var estTimePerExp = elapsedTime.Ticks / experimentIdx;
      var remainingTimeTicks = estTimePerExp * experimentsRemaining;
      var remainingTimeSpan = new TimeSpan(remainingTimeTicks);
      _campaignExecutionSummary.ETC = remainingTimeSpan;
      _campaignRemainingTimer = Observable.Interval(TimeSpan.FromSeconds(_currentConfig.TimerPrecision))
        .Subscribe(_ => _campaignExecutionSummary.ETC = _campaignExecutionSummary.ETC.Subtract(TimeSpan.FromSeconds(_currentConfig.TimerPrecision)));
    }

    private void ShutdownCampaign()
    {
      _campaignElapsedTimer.Dispose();
      _campaignRemainingTimer?.Dispose();
      _campaignExecutionSummary.ETC = TimeSpan.Zero;
      _campaignExecutionSummary.Status = ExecutionStatus.DONE;
      _campaign.IsExecuting = false;
    }

    private bool DoPreRunChecks()
    {
      if (_campaign.NumExperimentsToRun == 0)
      {
        AresKernel._kernel.Get<IAresConsole>().WriteLine("I was asked to run 0 experiments. Nothing to do.");
        return false;
      }
      if (_selectedPlanners.Count == 0)
      {
        AresKernel._kernel.Get<IAresConsole>().WriteLine("No Planners selected. Can't run experiments.");
        return false;
      }
      if (_campaign.ExpScript.Length == 0)
      {
        AresKernel._kernel.Get<IAresConsole>().WriteLine("Empty experiment script. Can't run experiments.");
        return false;
      }
      if (_campaign.InterExpScript.Length == 0)
      {
        AresKernel._kernel.Get<IAresConsole>().WriteLine("Empty inter-experiment script. Can't transition between experiments.");
        return false;
      }
      if (!_campaign.CanRun)
      {
        AresKernel._kernel.Get<IAresConsole>().WriteLine("Campaign run conditions are not satisfied.");
        return false;
      }
      return true;
    }

    public ICampaignExecutionSummary CampaignExecutionSummary
    {
      get => _campaignExecutionSummary;
      set => this.RaiseAndSetIfChanged(ref _campaignExecutionSummary, value);
    }

    public override async Task Handle(ErrorResponse errorResponse)
    {
      await base.Handle(errorResponse);
      if (!_experimentRetryInfos.Any() && !_campaignRetryInfos.Any())
      {
        IsComplete = true;
      }
    }

    protected override Task HandleStop()
    {
      if (_previousErrored is IScriptExecutor)
      {
        return HandleExperimentStop();
      }
      else if (_previousErrored is ICampaignExecutor)
      {
        return HandleCampaignStop();
      }
      return Task.CompletedTask;
    }

    private Task HandleExperimentStop()
    {
      _terminated = true;
      _experimentRetryInfos.Clear();
      Error = new AresError { Severity = ErrorSeverity.Error, Text = "Campaign Handling Campaign" };
      return Task.CompletedTask;
    }

    private Task HandleCampaignStop()
    {
      _terminated = true;
      ShouldExecute = false;
      ShutdownCampaign();
      //      _campaignRetryInfos.Clear();
      return Task.CompletedTask;
    }

    protected override Task HandleIgnoreAndContinue()
    {

      return _previousErrored is IScriptExecutor ? HandleExperimentIgnoreAndContinue() : HandleStop();
    }

    private Task HandleExperimentIgnoreAndContinue()
    {
      _experimentRetryInfos.RemoveAt(0);
      _shouldContinue = true;
      return Task.CompletedTask;
    }

    protected override Task HandleRetry()
    {
      if (_previousErrored is IScriptExecutor)
      {
        return HandleExperimentRetry();
      }
      else if (_previousErrored is ICampaignExecutor)
      {
        return HandleCampaignRetry();
      }
      return Task.CompletedTask;
    }

    private async Task HandleExperimentRetry()
    {
      var expRetryInfo = _experimentRetryInfos.FirstOrDefault();

      expRetryInfo.ExperimentResult.StepExecutionSummaries.ForEach(stepResult => stepResult.CommandExecutionSummaries.Clear());
      expRetryInfo.ExperimentResult.StepExecutionSummaries.Clear();
      try
      {
        await RunExperiment(expRetryInfo.Inputs, expRetryInfo.ExperimentResult);
      }
      catch (Exception)
      {

      }
      if (_experimentRetryInfos.Any())
      {
        _experimentRetryInfos.RemoveAt(0);
      }
    }

    private async Task HandleCampaignRetry()
    {
      await HandleStop();
      // Isn't retrying a campaign the same as letting the campaign end, then immediately hitting execute again? That works nicely, this retry logic is getting messy and buggy

      //      var retryInfo = _campaignRetryInfos.FirstOrDefault();
      //      _campaign.CanRun = true; // TODO Figure out why this is false and replace this cheat with logic
      //      try
      //      {
      //        await Execute(); // This might not work as expected. But in that case, isn't it not working as expected... expected?
      //      }
      //      catch ( Exception e )
      //      {
      //      }
      //
      //      if ( _campaignRetryInfos.Any() )
      //      {
      //        _campaignRetryInfos.RemoveAt( 0 );
      //      }
    }

    public bool IsComplete
    {
      get => _isComplete;
      set
      {
        _isComplete = value;
        this.RaisePropertyChanged();
      }
    }

    public bool ShouldExecute
    {
      get => _shouldExecute;
      set => this.RaiseAndSetIfChanged(ref _shouldExecute, value);
    }
  }
}