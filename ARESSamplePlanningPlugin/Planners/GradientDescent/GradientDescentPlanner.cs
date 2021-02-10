using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using ARESCore.Experiment.Results;
using ARESCore.PlanningSupport.Impl;
using ARESCore.UI;
using AresSamplePlanningPlugin.Planners.Parameters.Impl;
using DynamicData.Binding;
using MoreLinq;
using ReactiveUI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AresSamplePlanningPlugin.Planners.GradientDescent
{
  public class GradientDescentPlanner : AresPlanner
  {
    private IObservableCollection<SamplePlanningParameters> _parameters = new ObservableCollectionExtended<SamplePlanningParameters>();

    private readonly IPlannedExperimentBatchInputs _experimentBatchInputs = new PlannedExperimentBatchInputs();

    private ICampaignExecutionSummary _campaignSummary;
    private readonly ICampaign _campaign;
    private readonly IAresConsole _console;
    private double _targetResult;

    public GradientDescentPlanner(ICampaignExecutionSummary campaignSummary, ICampaign campaign, IAresConsole console)
    {
      for (var i = 0; i < RequiredNumberOfSeedExperiments; i++)
      {
        var seedExperiment = new SamplePlanningParameters();
        Parameters.Add(seedExperiment);
      }
      _campaignSummary = campaignSummary;
      _campaign = campaign;
      _console = console;
      Disposables.Add(campaign.WhenPropertyChanged(cmp => cmp.IsExecuting, false).Subscribe(executingProperty =>
      {
        if (!executingProperty.Value)
        {
          _experimentBatchInputs.PlannedInputs.Clear();
        }
      }));
      if (!_campaign.IsExecuting)
      {
        CanPlan = true;
      }
      else if (_campaignSummary.ExperimentExecutionSummaries.Count < 2)
      {
        CanPlan = false;
      }
    }

    // Either returns a set of 2 experiment parameters when the campaign is being setup, or returns a single set of parameters based on the 2 seeded experiment results
    public override Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      _console.WriteLine("Gradient Descent Planner, trying for " + TargetResult);
      var plannedFields = string.Join(",", Parameters.First().Select(parameter => parameter.ScriptLabel));
      _console.WriteLine(" Using arguments " + plannedFields);
      return Task.Run(async () =>
      {
        if (!_campaign.IsExecuting)
        {
          SeedExperimentBatchInputs.PlannedInputs.Clear();
          SeedExperimentBatchInputs.PlannedInputs.AddRange(Parameters.Select(GeneratePlannedInputs));
          _experimentBatchInputs.PlannedInputs.Clear();
          _experimentBatchInputs.PlannedInputs.AddRange(SeedExperimentBatchInputs.PlannedInputs);
          return SeedExperimentBatchInputs;

        }
        if (_campaignSummary.ExperimentExecutionSummaries.Count < RequiredNumberOfSeedExperiments)
        {
          return SeedExperimentBatchInputs;
        }
        var newInputStartIndex = _experimentBatchInputs.Count();
        for (var i = 0; i < NumExperimentsToPlan; i++)
        {
          _experimentBatchInputs.PlannedInputs.Add(PlanGradientDescent());
        }
        var newInputs = new PlannedExperimentBatchInputs();
        newInputs.PlannedInputs.AddRange(
          _experimentBatchInputs.PlannedInputs.GetRange(newInputStartIndex, NumExperimentsToPlan));
        return newInputs;
      });
    }

    private PlannedExperimentInputs PlanGradientDescent()
    {
      var secondLastPlan =
        GenerateParameters(
          _experimentBatchInputs.PlannedInputs[_experimentBatchInputs.PlannedInputs.Count - 2]);
      var secondLastResult =
        _campaignSummary.ExperimentExecutionSummaries[_campaignSummary.ExperimentExecutionSummaries.Count - 2].Result;
      var lastPlan =
        GenerateParameters(
          _experimentBatchInputs.PlannedInputs.Last());
      var lastResult =
        _campaignSummary.ExperimentExecutionSummaries.Last().Result;

      var newPLan = new SamplePlanningParameters();
      Parameters.First().ForEach(parameter =>
      {
        var newParameter = newPLan.First(newParam =>
          newParam.ScriptLabel.Equals(parameter.ScriptLabel, StringComparison.CurrentCultureIgnoreCase));
        newParameter.Min = parameter.Min;
        newParameter.Max = parameter.Max;
        newParameter.IsPlanned = parameter.IsPlanned;
      });


      var desiredChange = TargetResult - secondLastResult;
      var actualChange = lastResult - secondLastResult;
      var modRate = desiredChange / actualChange;

      if (Math.Abs(actualChange) > .00001)
      {
        secondLastPlan.ForEach(olderParam =>
        {

          var newerParam = lastPlan.First(parameter => parameter.ScriptLabel.Equals(olderParam.ScriptLabel, StringComparison.CurrentCultureIgnoreCase));
          var newParam = newPLan.First(parameter =>
            parameter.ScriptLabel.Equals(olderParam.ScriptLabel, StringComparison.CurrentCultureIgnoreCase));
          if (!olderParam.IsPlanned)
          {
            newParam.Value = newerParam.Value;
            return;
          }
          newParam.IsPlanned = true;
          var paramDiff = newerParam.Value - olderParam.Value;
          newParam.Value = olderParam.Value + paramDiff * modRate;
        });
      }
      else
      {
        secondLastPlan.ForEach(olderParam =>
        {
          var newerParam = lastPlan.First(parameter => parameter.ScriptLabel.Equals(olderParam.ScriptLabel, StringComparison.CurrentCultureIgnoreCase));
          var newParam = newPLan.First(parameter =>
            parameter.ScriptLabel.Equals(olderParam.ScriptLabel, StringComparison.CurrentCultureIgnoreCase));
          if (!olderParam.IsPlanned)
          {
            newParam.Value = newerParam.Value;
            return;
          }

          newParam.IsPlanned = true;
          var randomPercentage = new Random().NextDouble();
          // TODO: Determine a better way of changing values?
          if (randomPercentage > .5)
          {
            newParam.Value = olderParam.Value * randomPercentage; // Decreases value
          }
          else
          {
            newParam.Value = olderParam.Value * (1 + randomPercentage); // Increases value
          }
        });
      }

      EvaluateParameters(ref newPLan);
      var experimentInputs = GeneratePlannedInputs(newPLan);
      return experimentInputs;
    }

    private void EvaluateParameters(ref SamplePlanningParameters expParams)
    {
      var lastPlan =
        GenerateParameters(_experimentBatchInputs.PlannedInputs.Last());

      expParams.ForEach(expParam =>
      {
        if (expParam.IsPlanned && expParam.Value < expParam.Min)
        {
          expParam.Value = expParam.Min;
        }
        if (expParam.IsPlanned && expParam.Value > expParam.Max)
        {
          expParam.Value = expParam.Max;
        }
      });

      var calculatedVals = expParams.Select(param => param.Value);
      var previousVals = lastPlan.Select(param => param.Value);

      if (!calculatedVals.SequenceEqual(previousVals))
      {
        return;
      }

      expParams.ForEach(expParam =>
      {

        if (!expParam.IsPlanned)
        {
          return;
        }
        var coeff = Math.Abs(expParam.Value - expParam.Min) >= Math.Abs(expParam.Max - expParam.Value) ? .9 : 1.1;
        var newVal = expParam.Value * coeff;
        var min = expParam.Min;
        if (Math.Abs(min) < .00001 && newVal <= min)
        {
          newVal = .1 * expParam.Max;
        }
        expParam.Value = newVal;
        if (expParam.Value < expParam.Min)
        {
          expParam.Value = expParam.Min;
        }
        if (expParam.Value > expParam.Max)
        {
          expParam.Value = expParam.Max;
        }
      });
    }

    private SamplePlanningParameters GenerateParameters(IPlannedExperimentInputs experimentInputs)
    {
      var additiveParameters = new SamplePlanningParameters();
      experimentInputs.Inputs.Select(param => param.Name).ForEach(varName =>
      {
        additiveParameters.First(parameter =>
            parameter.ScriptLabel.Equals(varName, StringComparison.CurrentCultureIgnoreCase)).Value =
          experimentInputs.Inputs.First(param => param.Name.Equals(varName)).Value;
        var uiParam = Parameters.First().First(parameter => parameter.ScriptLabel.Equals(varName, StringComparison.CurrentCultureIgnoreCase));
        var refParam = additiveParameters.First(parameter =>
          parameter.ScriptLabel.Equals(varName, StringComparison.CurrentCultureIgnoreCase));
        refParam.IsPlanned = uiParam.IsPlanned;
      });
      return additiveParameters;
    }

    private PlannedExperimentInputs GeneratePlannedInputs(SamplePlanningParameters experimentParameters)
    {
      var varNames = experimentParameters.Select(parameter => parameter.ScriptLabel).ToList();
      var varVals = experimentParameters.Select(parameter => parameter.Value).ToList();
      var experimentInputs = new PlannedExperimentInputs(varNames, varVals);
      return experimentInputs;
    }

    public IObservableCollection<SamplePlanningParameters> Parameters
    {
      get => _parameters;
      set => this.RaiseAndSetIfChanged(ref _parameters, value);
    }

    public override int RequiredNumberOfSeedExperiments { get; set; } = 2;

    public double TargetResult
    {
      get => _targetResult;
      set => this.RaiseAndSetIfChanged(ref _targetResult, value);
    }
  }
}
