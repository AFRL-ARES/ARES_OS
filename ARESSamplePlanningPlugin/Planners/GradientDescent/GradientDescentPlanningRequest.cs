using System.Collections.Generic;
using ARESCore.PlanningSupport;
using AresSamplePlanningPlugin.Planners.Parameters.Impl;

namespace AresSamplePlanningPlugin.Planners.GradientDescent
{
  public class GradientDescentPlanningRequest : IAresPlanningRequest
  {
    public List<SamplePlanningParameters> Experiments { get; set; } = new List<SamplePlanningParameters>();
  }
}
