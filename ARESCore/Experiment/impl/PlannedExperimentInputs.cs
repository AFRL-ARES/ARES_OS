using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.Experiment.impl
{
  public class PlannedExperimentInputs : ReactiveSubscriber, IPlannedExperimentInputs
  {
    private IList<ExperimentParameter> _inputs = new List<ExperimentParameter>();

    public PlannedExperimentInputs()
    {
    }

    public PlannedExperimentInputs(List<string> variableStrings, List<double> inputs)
    {
      SetInputs(variableStrings, inputs);
    }

    public IList<ExperimentParameter> Inputs
    {
      get => _inputs;
      set => this.RaiseAndSetIfChanged(ref _inputs, value);
    }

    public void SetInputs(List<string> variableStrings, List<double> inputs)
    {
      Inputs.Clear();
      if (variableStrings == null || variableStrings.Count == 0)
        throw new Exception("Inputs descriptions cannot be null or empty!");
      if (inputs == null || inputs.Count == 0)
        throw new Exception("Inputs cannot be null or empty!");
      if (inputs.Count != inputs.Count)
        throw new Exception("Inputs cannot be null or empty!");
      if (inputs.Any(aData => (double.IsInfinity(aData) || double.IsNaN(aData))))
        throw new Exception("Cannot have NaN or an Infinity inputs value!");

      for (var index = 0; index < inputs.Count; index++)
      {
        var expParam = new ExperimentParameter {Value = inputs[index], Name = variableStrings[index]};
        Inputs.Add(expParam);

      }
    }

  }
}