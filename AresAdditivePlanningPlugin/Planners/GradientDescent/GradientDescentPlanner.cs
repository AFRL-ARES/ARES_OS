using System;
using System.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController;
using AresAdditivePlanningPlugin.Planners.Parameters.Impl;
using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using ARESCore.Experiment.Results;
using ARESCore.PlanningSupport.Impl;
using CommonServiceLocator;
using DynamicData.Binding;
using MoreLinq;
using ReactiveUI;

namespace AresAdditivePlanningPlugin.Planners.GradientDescent
{
  public class GradientDescentPlanner : AresPlanner
  {
    // Instantiates 2 sets of Additive parameters by default. One for experiment 1 manual planning, and another for experiment 2
    private IObservableCollection<AdditivePlanningParameters> _parameters =
      new ObservableCollectionExtended<AdditivePlanningParameters>();

    private readonly IPlannedExperimentBatchInputs _experimentBatchInputs = new PlannedExperimentBatchInputs();

    private ICampaignExecutionSummary _campaignSummary;
    private readonly ICampaign _campaign;
    private double _targetResult;

    public GradientDescentPlanner(ICampaignExecutionSummary campaignSummary, ICampaign campaign)
    {
      for (var i = 0; i < RequiredNumberOfSeedExperiments; i++)
      {
        var seedExperiment = new AdditivePlanningParameters();
        seedExperiment.ForEach(parameter => parameter.ResetToToolpathValue());
        Parameters.Add(seedExperiment);
      }
      _campaignSummary = campaignSummary;
      _campaign = campaign;
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
      else if (_campaignSummary.ExperimentExecutionSummaries.Count < 2
      ) // Shouldn't be planning unless theres 2 seeded experiments
      {
        CanPlan = false;
      }
    }

    // Either returns a set of 2 experiment parameters when the campaign is being setup, or returns a single set of parameters based on the 2 seeded experiment results
    public override Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      return Task.Run(async () =>
      {
        var stageController = ServiceLocator.Current.GetInstance<IStageController>();
        await stageController.WritePyDict("data.Planner", "GradientDescent");
        await stageController.WritePyDict("data.Target", TargetResult.ToString());
        var plannedParameters = Parameters.First().Where(parameter => parameter.IsPlanned);
        await stageController.WritePyDict("data.PlannedParams",
          string.Join(",", plannedParameters.Select(parameter => parameter.ScriptLabel)));
        // TODO: Verify the values above match the post integration version on the developer drive

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

        // TODO: Plan based on the last 2 results
        //        _experimentBatchInputs.PlannedInputs.Clear();
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
        GenerateAdditiveParameters(
          _experimentBatchInputs.PlannedInputs[_experimentBatchInputs.PlannedInputs.Count - 2]);
      var secondLastResult =
        _campaignSummary.ExperimentExecutionSummaries[_campaignSummary.ExperimentExecutionSummaries.Count - 2].Result;
      var lastPlan =
        GenerateAdditiveParameters(
          _experimentBatchInputs.PlannedInputs.Last());
      var lastResult =
        _campaignSummary.ExperimentExecutionSummaries.Last().Result;

      var newPLan = new AdditivePlanningParameters();
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

    private void EvaluateParameters(ref AdditivePlanningParameters expParams)
    {
      var lastPlan =
        GenerateAdditiveParameters(_experimentBatchInputs.PlannedInputs.Last());

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
          return; //TODO: Should behave like a "continue"
        }
        var coeff = Math.Abs(expParam.Value - expParam.Min) >= Math.Abs(expParam.Max - expParam.Value)
          ? .9
          : 1.1;
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

    private AdditivePlanningParameters GenerateAdditiveParameters(IPlannedExperimentInputs experimentInputs)
    {
      var additiveParameters = new AdditivePlanningParameters();
      experimentInputs.Inputs.Keys.ForEach(varName =>
      {
        additiveParameters.First(parameter =>
            parameter.ScriptLabel.Equals(varName, StringComparison.CurrentCultureIgnoreCase)).Value =
          experimentInputs.Inputs[varName];
        var uiParam = Parameters.First().First(parameter => parameter.ScriptLabel.Equals(varName, StringComparison.CurrentCultureIgnoreCase));
        var refParam = additiveParameters.First(parameter =>
          parameter.ScriptLabel.Equals(varName, StringComparison.CurrentCultureIgnoreCase));
        refParam.IsPlanned = uiParam.IsPlanned;
      });
      return additiveParameters;
    }

    private PlannedExperimentInputs GeneratePlannedInputs(AdditivePlanningParameters experimentParameters)
    {
      var varNames = experimentParameters.Select(parameter => parameter.ScriptLabel).ToList();
      var varVals = experimentParameters.Select(parameter => parameter.Value).ToList();
      var experimentInputs = new PlannedExperimentInputs(varNames, varVals);
      return experimentInputs;
    }

    public IObservableCollection<AdditivePlanningParameters> Parameters
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
