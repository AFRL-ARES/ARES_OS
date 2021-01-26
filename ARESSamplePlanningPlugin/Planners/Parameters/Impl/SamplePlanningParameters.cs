using DynamicData.Binding;

namespace AresSamplePlanningPlugin.Planners.Parameters.Impl
{
  public class SamplePlanningParameters : ObservableCollectionExtended<SamplePlanningParameter>
  {
    public SamplePlanningParameters()
    {
      Add(new SamplePlanningParameter());
    }

  }
}
