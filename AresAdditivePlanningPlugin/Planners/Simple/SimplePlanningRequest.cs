using System.Collections.Generic;
using AresAdditivePlanningPlugin.Planners.Parameters.Impl;
using ARESCore.PlanningSupport;

namespace AresAdditivePlanningPlugin.Planners
{
  public class SimplePlanningRequest : IAresPlanningRequest
  {
    public List<AdditivePlanningParameters> Experiments { get; set; } = new List<AdditivePlanningParameters>();
  }
}
