using System.Collections.Generic;
using ARESCore.PlanningSupport;
using AresSamplePlanningPlugin.Planners.Parameters.Impl;

namespace AresSamplePlanningPlugin.Planners.Simple
{
  public class SimplePlanningRequest : IAresPlanningRequest
  {
    public List<SamplePlanningParameters> Experiments { get; set; } = new List<SamplePlanningParameters>();
  }
}
