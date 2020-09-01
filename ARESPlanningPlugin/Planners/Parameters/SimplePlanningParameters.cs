using ARESCore.PlanningSupport;

namespace AresPlanningPlugin.Planners.Simple
{
   public class SimplePlanningParameters : IPlanningParameters
   {
      public IAresPlanningRequest GenerateRequest()
      {
         var request = new SimplePlanningRequest();
         // for each experiment to plan for
         var parameters = new PlanningParameters();
         request.Experiments.Add(parameters);
         return request;
      }

   }
}
