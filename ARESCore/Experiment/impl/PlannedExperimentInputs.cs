using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.Experiment.impl
{
  public class PlannedExperimentInputs : ReactiveSubscriber, IPlannedExperimentInputs
  {
    private IDictionary<string, double> _inputs;

    public PlannedExperimentInputs()
    {
      _inputs = new Dictionary<string, double>();
    }
    public PlannedExperimentInputs( List<string> variableStrings, List<double> inputs)
    {
      _inputs = new Dictionary<string, double>();

      SetInputs(variableStrings, inputs);
    }

    public IDictionary<string, double> Inputs
    {
      get => _inputs;
      set => this.RaiseAndSetIfChanged(ref _inputs, value);
    }

    public bool HasInputs()
    {
      return (_inputs != null && Inputs.Any());
    }

    public void SetInputs(List<string> variableStrings, List<double> inputs)
    {
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
            Inputs[variableStrings[index]] = inputs[index]; 
        }
      
    }

  }
}