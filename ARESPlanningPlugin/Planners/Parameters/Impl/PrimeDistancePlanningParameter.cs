using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class PrimeDistancePlanningParameter : AdditivePlanningParameter
  {
    public PrimeDistancePlanningParameter()
    {
      ScriptLabel = "VAL_PRIME_DISTANCE";
      PythonLabel = "dispenser.prime";
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
