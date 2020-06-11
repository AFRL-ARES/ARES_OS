using AresAdditivePlanningPlugin.Planners.Parameters.Impl;
using ARESCore.PlanningSupport;

namespace AresAdditivePlanningPlugin.Planners
{
  public class SimplePlanningParameters : IPlanningParameters
  {
    public IAresPlanningRequest GenerateRequest()
    {
      var request = new SimplePlanningRequest();
      // for each experiment to plan for
      var parameters = new AdditivePlanningParameters();
      request.Experiments.Add(parameters);
      return request;
    }

  }
}
