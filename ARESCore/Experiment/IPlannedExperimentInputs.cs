using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.impl;


namespace ARESCore.Experiment
{
  public interface IPlannedExperimentInputs : IReactiveSubscriber
  {
   IList<ExperimentParameter> Inputs { get; }
   void SetInputs( List<string> variableStrings, List<double> inputs);
  }
}
