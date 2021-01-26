using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using ARESCore.DisposePatternHelpers;


namespace ARESCore.Experiment
{
  public interface IPlannedExperimentInputs : IReactiveSubscriber
  {
   IDictionary<string, double> Inputs { get; set; }
   bool HasInputs();
   void SetInputs( List<string> variableStrings, List<double> inputs);
  }
}
