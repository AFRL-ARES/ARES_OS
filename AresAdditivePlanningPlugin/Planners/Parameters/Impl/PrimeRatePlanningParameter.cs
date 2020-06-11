using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class PrimeRatePlanningParameter : AdditivePlanningParameter
  {
    public PrimeRatePlanningParameter()
    {
      ScriptLabel = "VAL_PRIME_RATE";
      PythonLabel = "dispenser.prime_rate";
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
