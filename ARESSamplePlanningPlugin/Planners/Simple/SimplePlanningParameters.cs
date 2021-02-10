using ARESCore.PlanningSupport;
using AresSamplePlanningPlugin.Planners.Parameters.Impl;

namespace AresSamplePlanningPlugin.Planners.Simple
{
  public class SimplePlanningParameters : IPlanningParameters
  {
    public IAresPlanningRequest GenerateRequest()
    {
      var request = new SimplePlanningRequest();
      var parameters = new SamplePlanningParameters();
      request.Experiments.Add(parameters);
      return request;
    }

  }
}
