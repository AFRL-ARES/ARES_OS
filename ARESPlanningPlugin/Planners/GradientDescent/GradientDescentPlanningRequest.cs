using System.Collections.Generic;
using AresAdditivePlanningPlugin.Planners.Parameters.Impl;
using ARESCore.PlanningSupport;

namespace AresAdditivePlanningPlugin.Planners.GradientDescent
{
  public class GradientDescentPlanningRequest : IAresPlanningRequest
  {
    public List<AdditivePlanningParameters> Experiments { get; set; } = new List<AdditivePlanningParameters>();
  }
}
