using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class PrimeDelayPlanningParameter : AdditivePlanningParameter
  {
    public PrimeDelayPlanningParameter()
    {
      ScriptLabel = "VAL_PRIME_DELAY";
      PythonLabel = "dispenser.prime_delay";
    }


    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey(PythonLabel))
      {
        var parameter = toolpathParams[PythonLabel];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
