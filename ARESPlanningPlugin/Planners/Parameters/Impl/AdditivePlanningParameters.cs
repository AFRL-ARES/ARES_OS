using DynamicData.Binding;
using MoreLinq.Extensions;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class AdditivePlanningParameters : ObservableCollectionExtended<AdditivePlanningParameter>
  {
    public AdditivePlanningParameters()
    {
      Add(new NozzleDiameterPlanningParameter());
      Add(new ExtrusionMultiplierPlanningParameter());
      Add(new TipHeightPlanningParameter());
      Add(new PrimeDistancePlanningParameter());
      Add(new PrimeDelayPlanningParameter());
      Add(new PrimeRatePlanningParameter());
      Add(new RetractDistancePlanningParameter());
      Add(new RetractDelayPlanningParameter());
      Add(new RetractRatePlanningParameter());
      Add(new DispenserSpeedPlanningParameter());
      Add(new BedTemperaturePlanningParameter());
      Add(new Var1PlanningParameter());
      Add(new Var2PlanningParameter());
      Add(new Var3PlanningParameter());
      Add(new Var4PlanningParameter());
      Add(new Var5PlanningParameter());
      Add(new Var6PlanningParameter());
    }

    public void ResetToToolpathParameters()
    {
      this.ForEach(param => param.ResetToToolpathValue());
    }
  }
}
