using AresAdditiveDevicesPlugin.Planners;
using AresAdditiveDevicesPlugin.Processing;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using ARESCore.DisposePatternHelpers;
using ARESCore.Extensions;
using CommonServiceLocator;
using DynamicData.Binding;
using Humanizer;
using MoreLinq;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using AresAdditiveDevicesPlugin.PythonStageController;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class ExperimentCampaign : BasicReactiveObjectDisposable, IExperimentCampaign
  {
    private IPlanner _planner;
    private ICampaignSetup _campaignSetup;
    private bool _canRun;
    private bool _running;
    private ObservableCollection<ExperimentStatus> _statusText = new ObservableCollection<ExperimentStatus>();
    private int _currentExperimentIdx = 0;
    private string _lastStepResults = "No step results. First Experiment has not completed.";
    private IExperiment _currentExperiment;
    private BasicUserDefinedComponent _analysisComponent;
    private int _availableBefore;
    private int _numberRan;
    private ExperimentStatus _currentStatus;

    public ExperimentCampaign(ICampaignSetup campaignSetup)
    {
      _campaignSetup = campaignSetup;
      TerminationConditions = new ObservableCollectionExtended<ITerminationCondition>();
      TerminationConditions.SubscribeAndInvoke(async added => await TermCondsChanged(added), async removed => await TermCondsChanged(removed));
      Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(async _ => await CheckCanRun());
    }

    private Task TermCondsChanged(ITerminationCondition obj)
    {
      return CheckCanRun();
    }

    private Task CheckCanRun()
    {
      if (CampaignSetup.Grid == null || AnalysisComponent == null)
      {
        CanRun = false;
        return Task.CompletedTask;
      }

      if (!CampaignSetup.Grid.Any(validLocation => validLocation))
      {
        CanRun = false;
        return Task.CompletedTask;
      }


      if (!AnalysisComponent.Pipeline.Select(entry => entry.Inputs).Flatten().Cast<IProcessData>().Any(input => input.IsAnalysisImage))
      {
        CanRun = false;
        return Task.CompletedTask;
      }

      _availableBefore = CampaignSetup.Grid.Count(availabilty => availabilty);
      if (!Running && TerminationConditions.Count > 0 && Planner != null && CampaignSetup != null && CampaignSetup.VarEntries.Count > 0 && _availableBefore > 0)
        CanRun = true;
      else
      {
        CanRun = false;
      }
      return Task.CompletedTask;
    }

    public async Task RunCampaign()
    {
      await CheckCanRun();
      Running = CanRun;
      //      if ( !File.Exists( @"..\..\..\py\ParameterLimits.json" ) )
      //      {
      //        var configs = new ParameterLimitConfigurations();
      //        CampaignSetup.VarEntries.Map( entry =>
      //        {
      //          var config = new ParameterLimitConfiguration { Name = entry.Value.Name };
      //          configs.Limits.Add( config );
      //        } );
      //
      //        var json = JsonConvert.SerializeObject( configs );
      //        File.WriteAllText( @"..\..\..\py\ParamterLimits.json", json );
      //      }
      await Execute();
      Planner.Reset();
    }

    private Task UpdateStatus(ExperimentStatus expStatus, string status)
    {
      Application.Current.Dispatcher.Invoke(() => expStatus.Status = status);
      CurrentStatus = expStatus;
      return Task.CompletedTask;
    }

    private async Task Execute()
    {
      CurrentExperimentIdx = 1;
      Planner.Reset();
      StatusText.Clear();
      await MoveToCampaignStart();
      await Task.Run(async () =>
      {
        while (ShouldntTerminate())
        {
          var expStatus = new ExperimentStatus { ExpNum = CurrentExperimentIdx, Status = "Validating Inputs..." };
          Application.Current.Dispatcher.Invoke(() => StatusText.Add(expStatus));
          var ok = ValidateInputs();
          if (!ok)
          {
            await UpdateStatus(expStatus, "Invalid Termination Parameters. Please Click \"Stop Campaign\", review and edit.");
            return;
          }

          await UpdateStatus(expStatus, "Planning...");
          var expParams = Planner.Plan(CurrentExperimentIdx - 1, AnalysisComponent.Result);
          await UpdateStatus(expStatus, "Running Experiment:");
          foreach (var experimentParameter in expParams)
          {
            await UpdateStatus(expStatus, expStatus.Status + "\n" + experimentParameter.VarEntry.Name.Split('.').LastOrDefault().Humanize() + " : " + experimentParameter.VarEntry.Value);
          }
          var currExperiment = ServiceLocator.Current.GetInstance<IExperiment>();
          currExperiment.Number = CurrentExperimentIdx;
          CurrentExperiment = currExperiment;
          var ran = await currExperiment.Run(expParams, AnalysisComponent);
          if (ran)
          {
            if (Planner.Result == AnalysisComponent.Result)
            {
              await Task.Delay(400);
            }
            Planner.Result = AnalysisComponent.Result;
            await UpdateStatus(expStatus, expStatus.Status.Replace("Running", "Completed"));
            await UpdateStatus(expStatus, expStatus.Status + "\n\nResult: " + currExperiment.Results);
            expStatus.ExpNum = CurrentExperiment.Number; // Number may have changed while looking for an available experiment location
            _numberRan++;
          }
          else
          {
            await UpdateStatus(expStatus, "Experiment could not be run at this grid location");
          }
          LastStepResults = currExperiment.Results.ToString();
          if (CurrentExperiment == null)
          {
            await UpdateStatus(expStatus, "Something weird happened here. The campaign may have been running after telling it to stop, so this experiment is null");
            CurrentExperimentIdx = currExperiment.Number + 1;
          }
          else
          {
            CurrentExperimentIdx = CurrentExperiment.Number + 1;
          }
        }
        CurrentExperiment = null;
        _availableBefore = 0;
        _numberRan = 0;
      });
    }

    private async Task MoveToCampaignStart()
    {
      var stageController = ServiceLocator.Current.GetInstance<IStageController>();
      await stageController.GetPositions();
      await stageController.MoveTo(stageController.XPosition, stageController.YPosition, stageController.ZPosition + 2);
      await stageController.MoveTo(CampaignSetup.Grid.InitXPosition, CampaignSetup.Grid.InitYPosition, CampaignSetup.Grid.InitZPosition);
    }


    public BasicUserDefinedComponent AnalysisComponent
    {
      get => _analysisComponent;
      set
      {
        value.Result = -1;
        this.RaiseAndSetIfChanged(ref _analysisComponent, value);
      }
    }

    public IExperiment CurrentExperiment
    {
      get => _currentExperiment;
      set => this.RaiseAndSetIfChanged(ref _currentExperiment, value);
    }

    private bool ShouldntTerminate()
    {
      if (!Running)
      {
        return false;
      }
      if (CurrentExperiment == null)
        return true;

      // Check if there is any space for another experiment
      var maxExpIndex = CampaignSetup.Grid.Rows * CampaignSetup.Grid.Columns;
      if (CurrentExperimentIdx > maxExpIndex)
      {
        return false;
      }

      var expcountconds = TerminationConditions.Where((t => t is ExperimentCountTerminationCondition)).Cast<ExperimentCountTerminationCondition>().ToList();
      var upperBounds = expcountconds.Where(t => t.TargetisUpperBound);
      var lowerBounds = expcountconds.Where(t => !t.TargetisUpperBound);
      var tvalconds = TerminationConditions.Where(t => t.TerminationType == TerminationConditionType.TargetValue).Cast<TargetValueTerminationCondition>().ToList();

      var upperConditions = upperBounds as ExperimentCountTerminationCondition[] ?? upperBounds.ToArray();
      var hasUpperBound = upperConditions.Any();
      var lowerConditions = lowerBounds as ExperimentCountTerminationCondition[] ?? lowerBounds.ToArray();
      var hasLowerBound = lowerConditions.Any();
      var hasTarget = tvalconds.Any();

      // Run the minimum required number of experiments
      if (hasLowerBound)
      {
        var lower = lowerConditions.Max(condition => condition.ExperimentTarget);
        lower = lower == 0 ? 1 : lower;
        if (Planner.SeedCount >= lower)
        {
          lower += Planner.SeedCount;
        }
        var lowerMet = _numberRan >= lower;
        if (!lowerMet)
        {
          return true;
        }
        if (!hasUpperBound && !hasTarget)
        {
          return false;
        }

      }

      // Determine the experiment limit
      if (hasUpperBound)
      {
        var upperMet = _numberRan >= upperConditions.Min(condition => condition.ExperimentTarget);
        if (upperMet)
        {
          return false;
        }
      }

      // Check if there are target conditions
      if (!hasTarget)
        return true;

      var hasGreater = tvalconds.Any(t => t.TerminateWhenGreaterThanTarget);
      if (hasGreater)
      {
        var greaterRes = tvalconds.FirstOrDefault(t => t.TerminateWhenGreaterThanTarget && t.TargetValue < CurrentExperiment.Results);
        var greaterMet = greaterRes != null;
        if (greaterMet)
        {
          return false;
        }
      }
      var hasLess = tvalconds.Any(t => t.TerminateWhenLessThanTarget);
      if (hasLess)
      {
        var lessRes = tvalconds.FirstOrDefault(t => t.TerminateWhenLessThanTarget && t.TargetValue > CurrentExperiment.Results);
        var lessMet = lessRes != null;
        if (lessMet)
        {
          return false;
        }
      }

      var hasEqual = tvalconds.Any(t => t.TerminateWhenEqualToTarget);
      if (!hasEqual)
        return true;

      var res = tvalconds.FirstOrDefault(t => t.TerminateWhenEqualToTarget && Math.Abs(t.TargetValue - CurrentExperiment.Results) < t.TargetPrecision);
      var equalMet = res != null;
      return !equalMet;
    }

    private bool ValidateInputs()
    {
      return TerminationConditions.Any();
    }

    public ObservableCollection<ExperimentStatus> StatusText
    {
      get => _statusText;
      set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }

    public int CurrentExperimentIdx
    {
      get => _currentExperimentIdx;
      set => this.RaiseAndSetIfChanged(ref _currentExperimentIdx, value);
    }

    public string LastStepResults
    {
      get => _lastStepResults;
      set => this.RaiseAndSetIfChanged(ref _lastStepResults, value);
    }

    public ExperimentStatus CurrentStatus
    {
      get => _currentStatus;
      set => this.RaiseAndSetIfChanged(ref _currentStatus, value);
    }

    public async Task StopCampaign()
    {
      Running = false;
      await CheckCanRun();
    }


    public ObservableCollectionExtended<ITerminationCondition> TerminationConditions { get; }


    public IPlanner Planner
    {
      get => _planner;
      set => this.RaiseAndSetIfChanged(ref _planner, value);
    }

    public ICampaignSetup CampaignSetup
    {
      get => _campaignSetup;
      set => this.RaiseAndSetIfChanged(ref _campaignSetup, value);
    }

    public bool CanRun
    {
      get => _canRun;
      set => this.RaiseAndSetIfChanged(ref _canRun, value);
    }

    public bool Running
    {
      get => _running;
      set => this.RaiseAndSetIfChanged(ref _running, value);
    }
  }
}
