using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;

namespace ARESCore.PlanningSupport
{
  public interface IPlanResults : IBasicReactiveObjectDisposable
  {
    IPlannedExperimentBatchInputs Results { get; set; }
  }
}
